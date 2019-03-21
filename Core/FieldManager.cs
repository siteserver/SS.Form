using System.Collections.Generic;
using System.IO;
using System.Linq;
using SiteServer.Plugin;
using SS.Form.Core.Model;
using SS.Form.Core.Provider;
using SS.Form.Core.Utils;

namespace SS.Form.Core
{
    public static class FieldManager
    {
        private static class FieldManagerCache
        {
            private static readonly object LockObject = new object();
            private const string CacheKey = "SS.Form.Core.FieldManager";

            public static IEnumerable<KeyValuePair<string, FieldInfo>> GetAllFieldInfoList()
            {
                var retVal = CacheUtils.Get<List<KeyValuePair<string, FieldInfo>>>(CacheKey);
                if (retVal != null) return retVal;

                lock (LockObject)
                {
                    retVal = CacheUtils.Get<List<KeyValuePair<string, FieldInfo>>>(CacheKey);
                    if (retVal != null) return retVal;

                    retVal = FieldDao.GetAllFieldInfoList();

                    CacheUtils.InsertHours(CacheKey, retVal, 12);
                }

                return retVal;
            }

            public static void Clear()
            {
                CacheUtils.Remove(CacheKey);
            }
        }

        public static List<FieldInfo> GetFieldInfoList(int formId)
        {
            var fieldInfoList = new List<FieldInfo>();

            var entries = FieldManagerCache.GetAllFieldInfoList();
            var startKey = GetKeyPrefix(formId);
            var list = entries.Where(tuple => tuple.Key.StartsWith(startKey)).ToList();
            foreach (var pair in list)
            {
                if (pair.IsDefault()) continue;

                fieldInfoList.Add(pair.Value);
            }

            return fieldInfoList.OrderBy(fieldInfo => fieldInfo.Taxis == 0 ? int.MaxValue : fieldInfo.Taxis).ToList();
        }

        public static FieldInfo GetFieldInfo(int id)
        {
            var entries = FieldManagerCache.GetAllFieldInfoList();

            var entry = entries.FirstOrDefault(x => x.Value != null && x.Value.Id == id);
            return entry.IsDefault() ? null : entry.Value;
        }

        public static void ClearCache()
        {
            FieldManagerCache.Clear();
        }

        private static string GetKeyPrefix(int formId)
        {
            return $"{formId}$";
        }

        public static string GetKey(int formId, string title)
        {
            return $"{GetKeyPrefix(formId)}{title}";
        }

        public static string GetExtrasId(int fieldId, int itemId)
        {
            return $"attr_{fieldId}_{itemId}";
        }

        public static bool IsExtra(FieldInfo fieldInfo)
        {
            if (!FormUtils.IsSelectFieldType(fieldInfo.FieldType) || fieldInfo.Items == null || fieldInfo.Items.Count == 0) return false;
            foreach (var item in fieldInfo.Items)
            {
                if (item.IsExtras)
                {
                    return true;
                }
            }
            return false;
        }

        public static string Export(int formId)
        {
            var filePath = Context.UtilsApi.GetTemporaryFilesPath("表单字段.zip");
            var directoryPath = Context.UtilsApi.GetTemporaryFilesPath("FormFields");

            FormUtils.DeleteDirectoryIfExists(directoryPath);
            FormUtils.CreateDirectoryIfNotExists(directoryPath);

            var fieldInfoList = GetFieldInfoList(formId);

            var fieldIdList = fieldInfoList.Select(x => x.Id).ToList();
            FormUtils.WriteText(Path.Combine(directoryPath, "fieldIdList.json"), FormUtils.JsonSerialize(fieldIdList));

            foreach (var fieldInfo in fieldInfoList)
            {
                FormUtils.WriteText(Path.Combine(directoryPath, fieldInfo.Id + ".json"), FormUtils.JsonSerialize(fieldInfo));
            }

            Context.UtilsApi.CreateZip(filePath, directoryPath);

            return "表单字段.zip";
        }

        public static void Import(int siteId, int formId, string filePath)
        {
            var directoryPath = Context.UtilsApi.GetTemporaryFilesPath("FormFields");

            FormUtils.DeleteDirectoryIfExists(directoryPath);
            FormUtils.CreateDirectoryIfNotExists(directoryPath);

            Context.UtilsApi.ExtractZip(filePath, directoryPath);

            var fieldIdList =
                FormUtils.JsonDeserialize<List<int>>(
                    FormUtils.ReadText(Path.Combine(directoryPath, "fieldIdList.json")));

            foreach (var fieldId in fieldIdList)
            {
                var fieldInfo = FormUtils.JsonDeserialize<FieldInfo>(FormUtils.ReadText(Path.Combine(directoryPath, fieldId + ".json")));
                if (fieldInfo == null) continue;

                if (string.IsNullOrEmpty(fieldInfo.Title)) continue;

                if (FieldDao.IsTitleExists(formId, fieldInfo.Title)) continue;

                fieldInfo.FormId = formId;

                FieldDao.Insert(siteId, fieldInfo);
            }
        }
    }
}
