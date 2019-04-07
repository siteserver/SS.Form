using System;
using System.Collections.Generic;
using SiteServer.Plugin;
using SS.Form.Core.Model;
using SS.Form.Core.Provider;
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
                if (fieldInfo.FieldType == InputType.CheckBox.Value || fieldInfo.FieldType == InputType.SelectMultiple.Value)
                {
                    value = string.Join(",", FormUtils.JsonDeserialize<List<string>>(logInfo.Get<string>(fieldInfo.Title)));
                }
                else if (fieldInfo.FieldType == InputType.Date.Value)
                {
                    var date = logInfo.Get<DateTime?>(fieldInfo.Title);
                    if (date.HasValue)
                    {
                        value = date.Value.ToString("yyyy-MM-dd");
                    }
                }
                else if (fieldInfo.FieldType == InputType.DateTime.Value)
                {
                    var datetime = logInfo.Get<DateTime?>(fieldInfo.Title);
                    if (datetime.HasValue)
                    {
                        value = datetime.Value.ToString("yyyy-MM-dd HH:mm");
                    }
                }
                else
                {
                    value = logInfo.Get<string>(fieldInfo.Title);
                }
            }

            return value;
        }

        public static Dictionary<string, object> GetDict(List<FieldInfo> fieldInfoList, LogInfo logInfo)
        {
            var dict = new Dictionary<string, object>
            {
                {nameof(LogInfo.Id), logInfo.Id},
                {nameof(LogInfo.AddDate), logInfo.AddDate.ToString("yyyy-MM-dd HH:mm")},
                {nameof(LogInfo.IsReplied), logInfo.IsReplied},
                {
                    nameof(LogInfo.ReplyDate),
                    logInfo.IsReplied ? logInfo.ReplyDate.ToString("yyyy-MM-dd HH:mm") : string.Empty
                },
                {nameof(LogInfo.ReplyContent), logInfo.IsReplied ? logInfo.ReplyContent : string.Empty}
            };
            foreach (var fieldInfo in fieldInfoList)
            {
                dict[fieldInfo.Title] = GetValue(fieldInfo, logInfo);
            }

            return dict;
        }
    }
}
