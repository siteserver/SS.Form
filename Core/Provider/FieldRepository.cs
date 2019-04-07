using System.Collections.Generic;
using System.Linq;
using Datory;
using SiteServer.Plugin;
using SS.Form.Core.Model;
using SS.Form.Core.Utils;

namespace SS.Form.Core.Provider
{
    public class FieldRepository : Repository<FieldInfo>
    {
        public FieldRepository() : base(Context.DatabaseType, Context.ConnectionString)
        {

        }

        public int Insert(int siteId, FieldInfo fieldInfo)
        {
            fieldInfo.Taxis = GetMaxTaxis(fieldInfo.FormId) + 1;
            
            fieldInfo.Id = base.Insert(fieldInfo);

            FieldManager.ItemRepository.InsertItems(fieldInfo.FormId, fieldInfo.Id, fieldInfo.Items);

            var formInfo = FormManager.GetFormInfo(siteId, fieldInfo.FormId);
            var list = FormUtils.StringCollectionToStringList(formInfo.ListAttributeNames);
            list.Add(fieldInfo.Title);
            formInfo.ListAttributeNames = FormUtils.ObjectCollectionToString(list);
            FormManager.Repository.Update(formInfo);

            FieldManager.ClearCache(fieldInfo.FormId);
            return fieldInfo.Id;
        }

        public void Update(FieldInfo info, bool updateItems)
        {
            base.Update(info);

            if (updateItems)
            {
                FieldManager.ItemRepository.DeleteByFieldId(info.Id);
                FieldManager.ItemRepository.InsertItems(info.FormId, info.Id, info.Items);
            }

            FieldManager.ClearCache(info.FormId);
        }

        public bool Delete(int formId, int fieldId)
        {
            var deleted = base.Delete(fieldId);

            FieldManager.ItemRepository.DeleteByFieldId(fieldId);

            FieldManager.ClearCache(formId);

            return deleted;
        }

        public void DeleteByFormId(int formId)
        {
            base.Delete(Q.Where("FormId", formId));

            FieldManager.ItemRepository.DeleteByFormId(formId);

            FieldManager.ClearCache(formId);
        }

        public bool IsTitleExists(int formId, string title)
        {
            return Exists(Q.Where("FormId", formId).Where("Title", title));
        }

        private int GetMaxTaxis(int formId)
        {
            return Max(Q.Where("FormId", formId)) ?? 0;
        }

        public void TaxisDown(int formId, int id)
        {
            var fieldInfo = FieldManager.GetFieldInfo(formId, id);
            if (fieldInfo == null) return;

            var dataInfo = Get(Q
                .Where("FormId", fieldInfo.FormId)
                .Where("Taxis", ">", fieldInfo.Taxis)
                .OrderBy("Taxis")
            );

            if (dataInfo == null) return;

            var higherId = dataInfo.Id;
            var higherTaxis = dataInfo.Taxis;

            SetTaxis(formId, id, higherTaxis);
            SetTaxis(formId, higherId, fieldInfo.Taxis);
        }

        public void TaxisUp(int formId, int id)
        {
            var fieldInfo = FieldManager.GetFieldInfo(formId, id);
            if (fieldInfo == null) return;

            var dataInfo = Get(Q
                .Where("FormId", fieldInfo.FormId)
                .Where("Taxis", "<", fieldInfo.Taxis)
                .OrderByDesc("Taxis")
            );

            if (dataInfo == null) return;

            var lowerId = dataInfo.Id;
            var lowerTaxis = dataInfo.Taxis;

            SetTaxis(formId, id, lowerTaxis);
            SetTaxis(formId, lowerId, fieldInfo.Taxis);
        }

        private void SetTaxis(int formId, int id, int taxis)
        {
            Update(Q.Set("Taxis", taxis).Where("Id", id));

            FieldManager.ClearCache(formId);
        }
        
        public List<KeyValuePair<string, FieldInfo>> GetAllFieldInfoList(int formId)
        {
            var pairs = new List<KeyValuePair<string, FieldInfo>>();

            var allItemsDict = FieldManager.ItemRepository.GetAllItems(formId);

            var fieldInfoList = GetAll(Q.Where("FormId", formId).OrderByDesc("Taxis", "Id"));

            foreach (var fieldInfo in fieldInfoList)
            {
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
