using System.Collections.Generic;
using Datory;
using SiteServer.Plugin;
using SS.Form.Core.Model;

namespace SS.Form.Core.Provider
{
    public class FieldItemRepository : Repository<FieldItemInfo>
    {
        public FieldItemRepository() : base(Context.DatabaseType, Context.ConnectionString)
        {

        }

        public void InsertItems(int formId, int fieldId, List<FieldItemInfo> items)
        {
            if (formId <= 0 || fieldId <= 0 || items == null || items.Count == 0) return;

            foreach (var itemInfo in items)
            {
                itemInfo.FormId = formId;
                itemInfo.FieldId = fieldId;
                base.Insert(itemInfo);
            }
        }

        public void DeleteByFormId(int formId)
        {
            if (formId == 0) return;

            base.Delete(Q.Where("FormId", formId));
        }

        public void DeleteByFieldId(int fieldId)
        {
            if (fieldId == 0) return;

            base.Delete(Q.Where("FieldId", fieldId));
        }

        public IList<FieldItemInfo> GetItemInfoList(int fieldId)
        {
            return GetAll(Q.Where("FieldId", fieldId).OrderBy("Id"));
        }

        public Dictionary<int, List<FieldItemInfo>> GetAllItems(int formId)
        {
            var allDict = new Dictionary<int, List<FieldItemInfo>>();

            var fieldItemInfoList = GetAll(Q.Where("FormId", formId));

            foreach (var fieldItemInfo in fieldItemInfoList)
            {
                allDict.TryGetValue(fieldItemInfo.FieldId, out var list);

                if (list == null)
                {
                    list = new List<FieldItemInfo>();
                }

                list.Add(fieldItemInfo);

                allDict[fieldItemInfo.FieldId] = list;
            }

            return allDict;
        }
    }
}
