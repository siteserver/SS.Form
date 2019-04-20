using System.Collections.Generic;
using Datory;
using SiteServer.Plugin;
using SS.Form.Core.Model;

namespace SS.Form.Core.Provider
{
    public class FieldItemRepository : Repository<FieldItemInfo>
    {
        public FieldItemRepository() : base(Context.Environment.DatabaseType, Context.Environment.ConnectionString)
        {

        }

        //        public static void Insert(IDbTransaction trans, FieldItemInfo itemInfo)
        //        {
        //            var sqlString = $@"INSERT INTO {TableName} (
        //    {nameof(FieldItemInfo.FormId)},
        //    {nameof(FieldItemInfo.FieldId)},
        //    {nameof(FieldItemInfo.Value)},
        //    {nameof(FieldItemInfo.IsSelected)},
        //    {nameof(FieldItemInfo.IsExtras)}
        //) VALUES (
        //    @{nameof(FieldItemInfo.FormId)},
        //    @{nameof(FieldItemInfo.FieldId)},
        //    @{nameof(FieldItemInfo.Value)},
        //    @{nameof(FieldItemInfo.IsSelected)},
        //    @{nameof(FieldItemInfo.IsExtras)}
        //)";

        //            var parameters = new[]
        //            {
        //                Context.DatabaseApi.GetParameter(nameof(FieldItemInfo.FormId), itemInfo.FormId),
        //                Context.DatabaseApi.GetParameter(nameof(FieldItemInfo.FieldId), itemInfo.FieldId),
        //                Context.DatabaseApi.GetParameter(nameof(FieldItemInfo.Value), itemInfo.Value),
        //                Context.DatabaseApi.GetParameter(nameof(FieldItemInfo.IsSelected), itemInfo.IsSelected),
        //                Context.DatabaseApi.GetParameter(nameof(FieldItemInfo.IsExtras), itemInfo.IsExtras)
        //            };

        //            Context.DatabaseApi.ExecuteNonQuery(trans, sqlString, parameters);
        //        }

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

        //public static void InsertItems(int formId, int fieldId, List<FieldItemInfo> items)
        //{
        //    if (formId <= 0 || fieldId <= 0 || items == null || items.Count == 0) return;

        //    using (var conn = Context.DatabaseApi.GetConnection(Context.ConnectionString))
        //    {
        //        conn.Open();
        //        using (var trans = conn.BeginTransaction())
        //        {
        //            try
        //            {
        //                foreach (var itemInfo in items)
        //                {
        //                    itemInfo.FormId = formId;
        //                    itemInfo.FieldId = fieldId;
        //                    Insert(trans, itemInfo);
        //                }

        //                trans.Commit();
        //            }
        //            catch
        //            {
        //                trans.Rollback();
        //                throw;
        //            }
        //        }
        //    }
        //}

        public void DeleteByFormId(int formId)
        {
            if (formId == 0) return;

            base.Delete(Q.Where("FormId", formId));
        }

        //     public static void DeleteByFormId(int formId)
        //     {
        //         if (formId == 0) return;

        //         var sqlString = $"DELETE FROM {TableName} WHERE {nameof(FieldItemInfo.FormId)} = @{nameof(FieldItemInfo.FormId)}";

        //         var parms = new []
        //{
        //	Context.DatabaseApi.GetParameter(nameof(FieldItemInfo.FormId), formId)
        //};

        //         Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString, parms);
        //     }

        public void DeleteByFieldId(int fieldId)
        {
            if (fieldId == 0) return;

            base.Delete(Q.Where("FieldId", fieldId));
        }

        //public static void DeleteByFieldId(int fieldId)
        //{
        //    if (fieldId == 0) return;

        //    var sqlString = $"DELETE FROM {TableName} WHERE {nameof(FieldItemInfo.FieldId)} = @{nameof(FieldItemInfo.FieldId)}";

        //    var parms = new[]
        //    {
        //        Context.DatabaseApi.GetParameter(nameof(FieldItemInfo.FieldId), fieldId)
        //    };

        //    Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString, parms);
        //}

        public IList<FieldItemInfo> GetItemInfoList(int fieldId)
        {
            return GetAll(Q.Where("FieldId", fieldId).OrderBy("Id"));
        }

        //     public static List<FieldItemInfo> GetItemInfoList(int fieldId)
        //     {
        //         var items = new List<FieldItemInfo>();

        //         var sqlString =
        //             $@"SELECT {nameof(FieldItemInfo.Id)}, {nameof(FieldItemInfo.FormId)}, {nameof(FieldItemInfo.FieldId)}, {nameof(FieldItemInfo.Value)}, {nameof(FieldItemInfo.IsSelected)}, {nameof(FieldItemInfo.IsExtras)} FROM {TableName} WHERE ({nameof(FieldItemInfo.FieldId)} = @{nameof(FieldItemInfo.FieldId)})";

        //         var parms = new []
        //{
        //             Context.DatabaseApi.GetParameter(nameof(FieldItemInfo.FieldId), fieldId)
        //};

        //         using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString, parms))
        //         {
        //             while (rdr.Read())
        //             {
        //                 items.Add(GetFieldItemInfo(rdr));
        //             }
        //             rdr.Close();
        //         }

        //         return items;
        //     }

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

        //public static Dictionary<int, List<FieldItemInfo>> GetAllItems()
        //{
        //    var allDict = new Dictionary<int, List<FieldItemInfo>>();

        //    using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, $"SELECT {nameof(FieldItemInfo.Id)}, {nameof(FieldItemInfo.FormId)}, {nameof(FieldItemInfo.FieldId)}, {nameof(FieldItemInfo.Value)}, {nameof(FieldItemInfo.IsSelected)}, {nameof(FieldItemInfo.IsExtras)} FROM {TableName}"))
        //    {
        //        while (rdr.Read())
        //        {
        //            var item = GetFieldItemInfo(rdr);

        //            allDict.TryGetValue(item.FieldId, out var list);

        //            if (list == null)
        //            {
        //                list = new List<FieldItemInfo>();
        //            }

        //            list.Add(item);

        //            allDict[item.FieldId] = list;
        //        }
        //        rdr.Close();
        //    }

        //    return allDict;
        //}
    }
}
