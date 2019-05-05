using System.Collections.Generic;
using System.Linq;
using SiteServer.Plugin;
using SS.Form.Core.Model;
using SS.Form.Core.Repositories;
using SS.Form.Core.Utils;

namespace SS.Form.Core
{
    public static class FieldManager
    {
        public static FieldRepository Repository => new FieldRepository();
        public static FieldItemRepository ItemRepository => new FieldItemRepository();

        private static class FieldManagerCache
        {
            private static readonly object LockObject = new object();

            private static string GetCacheKey(int formId)
            {
                return $"SS.Form.Core.FieldManager.{formId}";
            }

            public static List<KeyValuePair<string, FieldInfo>> GetAllFieldInfoList(int formId)
            {
                var cacheKey = GetCacheKey(formId);
                var retVal = CacheUtils.Get<List<KeyValuePair<string, FieldInfo>>>(cacheKey);
                if (retVal != null) return retVal;

                lock (LockObject)
                {
                    retVal = CacheUtils.Get<List<KeyValuePair<string, FieldInfo>>>(cacheKey);
                    if (retVal == null)
                    {
                        retVal = Repository.GetAllFieldInfoList(formId);

                        CacheUtils.Insert(cacheKey, retVal, 12);
                    }
                }

                return retVal;
            }

            public static void Clear(int formId)
            {
                var cacheKey = GetCacheKey(formId);
                CacheUtils.Remove(cacheKey);
            }
        }

        public static List<FieldInfo> GetFieldInfoList(int formId)
        {
            var fieldInfoList = new List<FieldInfo>();

            var entries = FieldManagerCache.GetAllFieldInfoList(formId);
            var startKey = GetKeyPrefix(formId);
            var list = entries.Where(tuple => tuple.Key.StartsWith(startKey)).ToList();
            foreach (var pair in list)
            {
                if (pair.IsDefault()) continue;

                fieldInfoList.Add((FieldInfo)pair.Value.Clone());
            }

            return fieldInfoList.OrderBy(fieldInfo => fieldInfo.Taxis == 0 ? int.MaxValue : fieldInfo.Taxis).ToList();
        }

        public static FieldInfo GetFieldInfo(int formId, int id)
        {
            var entries = FieldManagerCache.GetAllFieldInfoList(formId);

            var entry = entries.FirstOrDefault(x => x.Value != null && x.Value.Id == id);
            return entry.IsDefault() ? null : (FieldInfo)entry.Value.Clone();
        }

        public static void ClearCache(int formId)
        {
            FieldManagerCache.Clear(formId);
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
            if (!IsSelectFieldType(fieldInfo.FieldType) || fieldInfo.Items == null || fieldInfo.Items.Count == 0) return false;
            foreach (var item in fieldInfo.Items)
            {
                if (item.IsExtras)
                {
                    return true;
                }
            }
            return false;
        }

        public static string GetFieldTypeText(string fieldType)
        {
            if (fieldType == InputType.TextArea.Value)
            {
                return "文本框(多行)";
            }
            if (fieldType == InputType.CheckBox.Value)
            {
                return "复选框";
            }
            if (fieldType == InputType.Radio.Value)
            {
                return "单选框";
            }
            if (fieldType == InputType.SelectOne.Value)
            {
                return "下拉列表(单选)";
            }
            if (fieldType == InputType.SelectMultiple.Value)
            {
                return "下拉列表(多选)";
            }
            if (fieldType == InputType.Date.Value)
            {
                return "日期选择框";
            }
            if (fieldType == InputType.DateTime.Value)
            {
                return "日期时间选择框";
            }
            if (fieldType == InputType.Hidden.Value)
            {
                return "隐藏";
            }

            return "文本框(单行)";
        }

        private static bool IsSelectFieldType(string fieldType)
        {
            return FormUtils.EqualsIgnoreCase(fieldType, InputType.CheckBox.Value) ||
                   FormUtils.EqualsIgnoreCase(fieldType, InputType.Radio.Value) ||
                   FormUtils.EqualsIgnoreCase(fieldType, InputType.SelectMultiple.Value) ||
                   FormUtils.EqualsIgnoreCase(fieldType, InputType.SelectOne.Value);
        }
    }
}
