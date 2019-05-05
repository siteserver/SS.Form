using System.Collections.Generic;
using System.Linq;
using Datory;
using SiteServer.Plugin;
using SS.Form.Core.Model;
using SS.Form.Core.Utils;

namespace SS.Form.Core.Repositories
{
    public class FieldRepository
    {
        private readonly Repository<FieldInfo> _repository;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public FieldRepository()
        {
            _repository = new Repository<FieldInfo>(Context.Environment.DatabaseType, Context.Environment.ConnectionString);
        }

        private static class Attr
        {
            public const string Id = nameof(FieldInfo.Id);

            public const string FormId = nameof(FieldInfo.FormId);

            public const string Title = nameof(FieldInfo.Title);

            public const string Taxis = nameof(FieldInfo.Taxis);
        }

        public void Insert(int siteId, FieldInfo fieldInfo)
        {
            fieldInfo.Taxis = GetMaxTaxis(fieldInfo.FormId) + 1;

            fieldInfo.Id = _repository.Insert(fieldInfo);

            FieldManager.ItemRepository.InsertItems(fieldInfo.FormId, fieldInfo.Id, fieldInfo.Items);

            var formInfo = FormManager.GetFormInfo(siteId, fieldInfo.FormId);
            var list = FormUtils.StringCollectionToStringList(formInfo.ListAttributeNames);
            list.Add(fieldInfo.Title);
            formInfo.ListAttributeNames = FormUtils.ObjectCollectionToString(list);
            FormManager.Repository.Update(formInfo);

            FieldManager.ClearCache(fieldInfo.FormId);
        }

        public void Update(FieldInfo info, bool updateItems)
        {
            _repository.Update(info);

            if (updateItems)
            {
                FieldManager.ItemRepository.DeleteByFieldId(info.Id);
                FieldManager.ItemRepository.InsertItems(info.FormId, info.Id, info.Items);
            }

            FieldManager.ClearCache(info.FormId);
        }

        public void Delete(int formId, int fieldId)
        {
            if (fieldId == 0) return;

            _repository.Delete(fieldId);

            FieldManager.ItemRepository.DeleteByFieldId(fieldId);

            FieldManager.ClearCache(formId);
        }

        public void Delete(int formId, string title)
        {
            var fieldId = _repository.Get<int>(Q
                .Select(Attr.Id)
                .Where(Attr.FormId, formId)
                .Where(Attr.Title, title)
            );
            Delete(formId, fieldId);
        }

        public void DeleteByFormId(int formId)
        {
            _repository.Delete(Q.Where(Attr.FormId, formId));

            FieldManager.ItemRepository.DeleteByFormId(formId);

            FieldManager.ClearCache(formId);
        }

        public bool IsTitleExists(int formId, string title)
        {
            return _repository.Exists(Q
                .Where(Attr.FormId, formId)
                .Where(Attr.Title, title)
            );
        }

        private int GetMaxTaxis(int formId)
        {
            return _repository.Max(Attr.Taxis, Q
                       .Where(Attr.FormId, formId)
                   ) ?? 0;
        }

        public List<KeyValuePair<string, FieldInfo>> GetAllFieldInfoList(int formId)
        {
            var pairs = new List<KeyValuePair<string, FieldInfo>>();

            var allItemsDict = FieldManager.ItemRepository.GetAllItems(formId);

            var fieldInfoList = _repository.GetAll(Q
                .Where(Attr.FormId, formId)
                .OrderByDesc(Attr.Taxis, Attr.Id)
            );

            foreach (var fieldInfo in fieldInfoList)
            {
                fieldInfo.Validate = string.IsNullOrEmpty(fieldInfo.Validate) ? string.Empty : fieldInfo.Validate;
                fieldInfo.Value = fieldInfo.Value ?? string.Empty;

                allItemsDict.TryGetValue(fieldInfo.Id, out var items);
                if (items == null)
                {
                    items = new List<FieldItemInfo>();
                }
                fieldInfo.Items = items;

                var key = FieldManager.GetKey(fieldInfo.FormId, fieldInfo.Title);

                if (pairs.All(pair => pair.Key != key))
                {
                    var pair = new KeyValuePair<string, FieldInfo>(key, fieldInfo);
                    pairs.Add(pair);
                }
            }

            return pairs;
        }
    }
}
