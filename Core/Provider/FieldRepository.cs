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
        public FieldRepository() : base(Context.Environment.DatabaseType, Context.Environment.ConnectionString)
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

        //        public static void Insert(int siteId, FieldInfo fieldInfo)
        //        {
        //            fieldInfo.Taxis = GetMaxTaxis(fieldInfo.FormId) + 1;

        //            var sqlString = $@"INSERT INTO {TableName}
        //(
        //    {nameof(FieldInfo.FormId)}, 
        //    {nameof(FieldInfo.Taxis)},
        //    {nameof(FieldInfo.Title)},
        //    {nameof(FieldInfo.Description)},
        //    {nameof(FieldInfo.PlaceHolder)},
        //    {nameof(FieldInfo.FieldType)},
        //    {nameof(FieldInfo.Validate)},
        //    {nameof(FieldInfo.Columns)},
        //    {nameof(FieldInfo.Height)}
        //) VALUES (
        //    @{nameof(FieldInfo.FormId)}, 
        //    @{nameof(FieldInfo.Taxis)},
        //    @{nameof(FieldInfo.Title)},
        //    @{nameof(FieldInfo.Description)},
        //    @{nameof(FieldInfo.PlaceHolder)},
        //    @{nameof(FieldInfo.FieldType)},
        //    @{nameof(FieldInfo.Validate)},
        //    @{nameof(FieldInfo.Columns)},
        //    @{nameof(FieldInfo.Height)}
        //)";

        //            var parameters = new []
        //			{
        //                Context.DatabaseApi.GetParameter(nameof(FieldInfo.FormId), fieldInfo.FormId),
        //                Context.DatabaseApi.GetParameter(nameof(FieldInfo.Taxis), fieldInfo.Taxis),
        //                Context.DatabaseApi.GetParameter(nameof(FieldInfo.Title), fieldInfo.Title),
        //                Context.DatabaseApi.GetParameter(nameof(FieldInfo.Description), fieldInfo.Description),
        //                Context.DatabaseApi.GetParameter(nameof(FieldInfo.PlaceHolder), fieldInfo.PlaceHolder),
        //                Context.DatabaseApi.GetParameter(nameof(FieldInfo.FieldType), fieldInfo.FieldType),
        //			    Context.DatabaseApi.GetParameter(nameof(FieldInfo.Validate), fieldInfo.Validate),
        //                Context.DatabaseApi.GetParameter(nameof(FieldInfo.Columns), fieldInfo.Columns),
        //			    Context.DatabaseApi.GetParameter(nameof(FieldInfo.Height), fieldInfo.Height)
        //            };

        //            var id = Context.DatabaseApi.ExecuteNonQueryAndReturnId(TableName, nameof(FieldInfo.Id), Context.ConnectionString, sqlString, parameters);

        //            FieldItemDao.InsertItems(fieldInfo.FormId, id, fieldInfo.Items);

        //            var formInfo = FormManager.GetFormInfo(siteId, fieldInfo.FormId);
        //            var list = FormUtils.StringCollectionToStringList(formInfo.ListAttributeNames);
        //            list.Add(fieldInfo.Title);
        //            formInfo.ListAttributeNames = FormUtils.ObjectCollectionToString(list);
        //            FormDao.Update(formInfo);

        //            FieldManager.ClearCache();
        //        }

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

        //     public static void Update(FieldInfo info, bool updateItems)
        //     {
        //         var sqlString = $@"UPDATE {TableName} SET
        //             {nameof(FieldInfo.FormId)} = @{nameof(FieldInfo.FormId)}, 
        //             {nameof(FieldInfo.Taxis)} = @{nameof(FieldInfo.Taxis)}, 
        //             {nameof(FieldInfo.Title)} = @{nameof(FieldInfo.Title)},
        //             {nameof(FieldInfo.Description)} = @{nameof(FieldInfo.Description)},
        //             {nameof(FieldInfo.PlaceHolder)} = @{nameof(FieldInfo.PlaceHolder)},
        //             {nameof(FieldInfo.FieldType)} = @{nameof(FieldInfo.FieldType)},
        //             {nameof(FieldInfo.Validate)} = @{nameof(FieldInfo.Validate)},
        //             {nameof(FieldInfo.Columns)} = @{nameof(FieldInfo.Columns)},
        //             {nameof(FieldInfo.Height)} = @{nameof(FieldInfo.Height)}
        //         WHERE {nameof(FieldInfo.Id)} = @{nameof(FieldInfo.Id)}";

        //         var parameters = new []
        //{
        //             Context.DatabaseApi.GetParameter(nameof(FieldInfo.FormId), info.FormId),
        //             Context.DatabaseApi.GetParameter(nameof(FieldInfo.Taxis), info.Taxis),
        //             Context.DatabaseApi.GetParameter(nameof(FieldInfo.Title), info.Title),
        //             Context.DatabaseApi.GetParameter(nameof(FieldInfo.Description), info.Description),
        //             Context.DatabaseApi.GetParameter(nameof(FieldInfo.PlaceHolder), info.PlaceHolder),
        //             Context.DatabaseApi.GetParameter(nameof(FieldInfo.FieldType), info.FieldType),
        //    Context.DatabaseApi.GetParameter(nameof(FieldInfo.Validate), info.Validate),
        //             Context.DatabaseApi.GetParameter(nameof(FieldInfo.Columns), info.Columns),
        //    Context.DatabaseApi.GetParameter(nameof(FieldInfo.Height), info.Height),
        //             Context.DatabaseApi.GetParameter(nameof(FieldInfo.Id), info.Id)
        //         };

        //         Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString, parameters);

        //         if (updateItems)
        //         {
        //             FieldItemDao.DeleteByFieldId(info.Id);
        //             FieldItemDao.InsertItems(info.FormId, info.Id, info.Items);
        //         }

        //         FieldManager.ClearCache();
        //     }

        public bool Delete(int formId, int fieldId)
        {
            var deleted = base.Delete(fieldId);

            FieldManager.ItemRepository.DeleteByFieldId(fieldId);

            FieldManager.ClearCache(formId);

            return deleted;
        }

        //public static void Delete(int fieldId)
        //{
        //    var sqlString = $"DELETE FROM {TableName} WHERE {nameof(FieldInfo.Id)} = @{nameof(FieldInfo.Id)}";

        //    var parameters = new []
        //    {
        //        Context.DatabaseApi.GetParameter(nameof(FieldInfo.Id), fieldId)
        //    };

        //    Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString, parameters);

        //    FieldItemDao.DeleteByFieldId(fieldId);

        //    FieldManager.ClearCache();
        //}

        public void DeleteByFormId(int formId)
        {
            base.Delete(Q.Where("FormId", formId));

            FieldManager.ItemRepository.DeleteByFormId(formId);

            FieldManager.ClearCache(formId);
        }

        //public static void DeleteByFormId(int formId)
        //{
        //    var sqlString = $"DELETE FROM {TableName} WHERE {nameof(FieldInfo.FormId)} = @{nameof(FieldInfo.FormId)}";

        //    var parameters = new[]
        //    {
        //        Context.DatabaseApi.GetParameter(nameof(FieldInfo.FormId), formId)
        //    };

        //    Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString, parameters);

        //    FieldItemDao.DeleteByFormId(formId);

        //    FieldManager.ClearCache();
        //}

        public bool IsTitleExists(int formId, string title)
        {
            return Exists(Q.Where("FormId", formId).Where("Title", title));
        }

        //     public static bool IsTitleExists(int formId, string title)
        //     {
        //         var exists = false;

        //         var sqlString = $@"SELECT Id FROM {TableName} WHERE 
        // {nameof(FieldInfo.FormId)} = @{nameof(FieldInfo.FormId)} AND 
        // {nameof(FieldInfo.Title)} = @{nameof(FieldInfo.Title)}";

        //         var parameters = new []
        //{
        //             Context.DatabaseApi.GetParameter(nameof(FieldInfo.FormId), formId),
        //             Context.DatabaseApi.GetParameter(nameof(FieldInfo.Title), title)
        //         };

        //         using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString, parameters))
        //         {
        //             if (rdr.Read() && !rdr.IsDBNull(0))
        //             {
        //                 exists = true;
        //             }
        //             rdr.Close();
        //         }

        //         return exists;
        //     }

        private int GetMaxTaxis(int formId)
        {
            return Max("Taxis", Q.Where("FormId", formId)) ?? 0;
        }

        //private static int GetMaxTaxis(int formId)
        //{
        //    var sqlString =
        //        $"SELECT MAX(Taxis) AS MaxTaxis FROM {TableName} WHERE {nameof(FieldInfo.FormId)} = {formId}";
        //    var maxTaxis = 0;

        //    using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString))
        //    {
        //        if (rdr.Read() && !rdr.IsDBNull(0))
        //        {
        //            maxTaxis = rdr.GetInt32(0);
        //        }
        //        rdr.Close();
        //    }
        //    return maxTaxis;
        //}

        //     public static void TaxisDown(int id)
        //     {
        //         var fieldInfo = FieldManager.GetFieldInfo(id);
        //         if (fieldInfo == null) return;

        //         var sqlString = Context.DatabaseApi.GetPageSqlString(TableName, "Id, Taxis", $"WHERE {nameof(FieldInfo.FormId)} = @{nameof(FieldInfo.FormId)} AND Taxis > (SELECT Taxis FROM {TableName} WHERE Id = @Id)", "ORDER BY Taxis", 0, 1);

        //         var higherId = 0;
        //         var higherTaxis = 0;

        //         var parameters = new []
        //         {
        //             Context.DatabaseApi.GetParameter(nameof(FieldInfo.FormId), fieldInfo.FormId),
        //             Context.DatabaseApi.GetParameter(nameof(FieldInfo.Id), id)
        //         };

        //         using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString, parameters))
        //         {
        //             if (rdr.Read() && !rdr.IsDBNull(0))
        //             {
        //                 higherId = rdr.GetInt32(0);
        //                 higherTaxis = rdr.GetInt32(1);
        //             }
        //             rdr.Close();
        //         }

        //         if (higherId != 0)
        //         {
        //             SetTaxis(id, higherTaxis);
        //             SetTaxis(higherId, fieldInfo.Taxis);
        //         }
        //     }

        //     public static void TaxisUp(int id)
        //     {
        //         var fieldInfo = FieldManager.GetFieldInfo(id);
        //         if (fieldInfo == null) return;

        //         var sqlString = Context.DatabaseApi.GetPageSqlString(TableName, "Id, Taxis", $"WHERE {nameof(FieldInfo.FormId)} = @{nameof(FieldInfo.FormId)} AND Taxis < (SELECT Taxis FROM {TableName} WHERE Id = @Id)", "ORDER BY Taxis DESC", 0, 1);
        //         var lowerId = 0;
        //         var lowerTaxis = 0;

        //         var parameters = new []
        //         {
        //             Context.DatabaseApi.GetParameter(nameof(FieldInfo.FormId), fieldInfo.FormId),
        //             Context.DatabaseApi.GetParameter(nameof(FieldInfo.Id), id)
        //         };

        //         using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString, parameters))
        //         {
        //             if (rdr.Read() && !rdr.IsDBNull(0))
        //             {
        //                 lowerId = rdr.GetInt32(0);
        //                 lowerTaxis = rdr.GetInt32(1);
        //             }
        //             rdr.Close();
        //         }

        //         if (lowerId != 0)
        //         {
        //             SetTaxis(id, lowerTaxis);
        //             SetTaxis(lowerId, fieldInfo.Taxis);
        //         }
        //     }

        //     private static void SetTaxis(int id, int taxis)
        //     {
        //         var sqlString = $"UPDATE {TableName} SET Taxis = @Taxis WHERE Id = @Id";

        //         var parameters = new []
        //{
        //	Context.DatabaseApi.GetParameter(nameof(FieldInfo.Taxis), taxis),
        //             Context.DatabaseApi.GetParameter(nameof(FieldInfo.Id), id)
        //};

        //         Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString, parameters);

        //         FieldManager.ClearCache();
        //     }

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

//        public static List<KeyValuePair<string, FieldInfo>> GetAllFieldInfoList()
//        {
//            var pairs = new List<KeyValuePair<string, FieldInfo>>();

//            var allItemsDict = FieldItemDao.GetAllItems();

//            var sqlString = $@"SELECT
//    {nameof(FieldInfo.Id)}, 
//    {nameof(FieldInfo.FormId)}, 
//    {nameof(FieldInfo.Taxis)},
//    {nameof(FieldInfo.Title)},
//    {nameof(FieldInfo.Description)},
//    {nameof(FieldInfo.PlaceHolder)},
//    {nameof(FieldInfo.FieldType)},
//    {nameof(FieldInfo.Validate)},
//    {nameof(FieldInfo.Columns)},
//    {nameof(FieldInfo.Height)}
//FROM {TableName} 
//ORDER BY {nameof(FieldInfo.Taxis)} DESC, {nameof(FieldInfo.Id)} DESC";

//            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString))
//            {
//                while (rdr.Read())
//                {
//                    var fieldInfo = GetFieldInfo(rdr);

//                    allItemsDict.TryGetValue(fieldInfo.Id, out var items);
//                    if (items == null)
//                    {
//                        items = new List<FieldItemInfo>();
//                    }
//                    fieldInfo.Items = items;

//                    var key = FieldManager.GetKey(fieldInfo.FormId, fieldInfo.Title);

//                    if (pairs.All(pair => pair.Key != key))
//                    {
//                        var pair = new KeyValuePair<string, FieldInfo>(key, fieldInfo);
//                        pairs.Add(pair);
//                    }
//                }
//                rdr.Close();
//            }

//            return pairs;
//        }
    }
}
