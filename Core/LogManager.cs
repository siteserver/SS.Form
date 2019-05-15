using System;
using System.Collections.Generic;
using SiteServer.Plugin;
using SS.Form.Core.Model;
using SS.Form.Core.Repositories;
using SS.Form.Core.Utils;

namespace SS.Form.Core
{
    public static class LogManager
    {
        public static LogRepository Repository => new LogRepository();

        public static string GetValue(FieldInfo fieldInfo, LogInfo logInfo)
        {
            var value = string.Empty;
            if (logInfo.ContainsKey(fieldInfo.Title))
            {
                var fieldValue = logInfo.Get<string>(fieldInfo.Title);

                if (fieldInfo.FieldType == InputType.CheckBox.Value || fieldInfo.FieldType == InputType.SelectMultiple.Value)
                {
                    var list = FormUtils.JsonDeserialize<List<string>>(fieldValue);
                    if (list != null)
                    {
                        value = string.Join(",", list);
                    }
                }
                else if (fieldInfo.FieldType == InputType.Date.Value)
                {
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        var date = FormUtils.ToDateTime(fieldValue, DateTime.MinValue);
                        if (date != DateTime.MinValue)
                        {
                            value = date.ToString("yyyy-MM-dd");
                        }
                    }
                }
                else if (fieldInfo.FieldType == InputType.DateTime.Value)
                {
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        var date = FormUtils.ToDateTime(fieldValue, DateTime.MinValue);
                        if (date != DateTime.MinValue)
                        {
                            value = date.ToString("yyyy-MM-dd HH:mm");
                        }
                    }
                }
                else
                {
                    value = fieldValue;
                }
            }

            return value;
        }

        public static Dictionary<string, object> GetDict(List<FieldInfo> fieldInfoList, LogInfo logInfo)
        {
            var dict = new Dictionary<string, object>
            {
                {nameof(LogInfo.Id), logInfo.Id},
                {nameof(LogInfo.Guid), logInfo.Guid},
                {nameof(LogInfo.IsReplied), logInfo.IsReplied},
                {nameof(LogInfo.ReplyContent), logInfo.IsReplied ? logInfo.ReplyContent : string.Empty}
            };
            if (logInfo.AddDate.HasValue)
            {
                dict[nameof(LogInfo.AddDate)] = logInfo.AddDate.Value.ToString("yyyy-MM-dd HH:mm");
            }
            if (logInfo.ReplyDate.HasValue)
            {
                dict[nameof(LogInfo.ReplyDate)] = logInfo.ReplyDate.Value.ToString("yyyy-MM-dd HH:mm");
            }
            foreach (var fieldInfo in fieldInfoList)
            {
                dict[fieldInfo.Title] = GetValue(fieldInfo, logInfo);
            }

            return dict;
        }
    }
}
