using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SiteServer.Plugin;
using SS.Form.Core.Model;
using SS.Form.Core.Provider;
using SS.Form.Core.Utils;
using SS.Mail;
using SS.SMS;

namespace SS.Form.Core
{
    public static class FormManager
    {
        private static class FormManagerCache
        {
            private static readonly object LockObject = new object();

            private static string GetCacheKey(int siteId)
            {
                return $"SS.Form.Core.FormManager.{siteId}";
            }

            public static List<FormInfo> GetCacheFormInfoList(int siteId)
            {
                var cacheKey = GetCacheKey(siteId);
                var retval = CacheUtils.Get<List<FormInfo>>(cacheKey);
                if (retval != null) return retval;

                lock (LockObject)
                {
                    retval = CacheUtils.Get<List<FormInfo>>(cacheKey);
                    if (retval == null)
                    {
                        retval = FormDao.GetFormInfoList(siteId);

                        CacheUtils.InsertHours(cacheKey, retval, 12);
                    }
                }

                return retval;
            }

            public static void Update(FormInfo formInfo)
            {
                lock (LockObject)
                {
                    var formInfoList = GetCacheFormInfoList(formInfo.SiteId);
                    var index = formInfoList.FindIndex(x => x.Id == formInfo.Id);
                    if (index != -1)
                    {
                        formInfoList[index] = formInfo;
                    }
                    else
                    {
                        formInfoList.Add(formInfo);
                    }
                }
            }

            public static void Clear(int siteId)
            {
                var cacheKey = GetCacheKey(siteId);
                CacheUtils.Remove(cacheKey);
            }
        }

        public static List<FormInfo> GetFormInfoList(int siteId, int channelId)
        {
            var formInfoList = FormManagerCache.GetCacheFormInfoList(siteId);

            return formInfoList.Where(formInfo => formInfo.ChannelId == channelId).OrderByDescending(formInfo => formInfo.Taxis == 0 ? int.MaxValue : formInfo.Taxis).ToList();
        }

        public static FormInfo GetFormInfo(int siteId, int id)
        {
            var formInfoList = FormManagerCache.GetCacheFormInfoList(siteId);

            return formInfoList.FirstOrDefault(x => x.Id == id);
        }

        public static FormInfo GetFormInfoByGet(IRequest request)
        {
            return GetFormInfoByRequest(request, true);
        }

        public static FormInfo GetFormInfoByPost(IRequest request)
        {
            return GetFormInfoByRequest(request, false);
        }

        private static FormInfo GetFormInfoByRequest(IRequest request, bool get)
        {
            var siteId = get ? request.GetQueryInt("siteId") : request.GetPostInt("siteId");
            var channelId = get ? request.GetQueryInt("channelId") : request.GetPostInt("channelId");
            var contentId = get ? request.GetQueryInt("contentId") : request.GetPostInt("contentId");
            var formId = get ? request.GetQueryInt("formId") : request.GetPostInt("formId");

            return formId > 0 ? GetFormInfo(siteId, formId) : GetFormInfoOrCreateIfNotExists(siteId, channelId, contentId);
        }

        public static FormInfo GetFormInfoByContentId(int siteId, int channelId, int contentId)
        {
            var formInfoList = FormManagerCache.GetCacheFormInfoList(siteId);
            return formInfoList.FirstOrDefault(x => x.ChannelId == channelId && x.ContentId == contentId);
        }

        private static FormInfo GetFormInfoOrCreateIfNotExists(int siteId, int channelId, int contentId)
        {
            return GetFormInfoByContentId(siteId, channelId, contentId) ?? FormDao.CreateDefaultForm(siteId, channelId, contentId);
        }

        public static FormInfo GetFormInfoByTitle(int siteId, string title)
        {
            var formInfoList = FormManagerCache.GetCacheFormInfoList(siteId);
            return formInfoList.FirstOrDefault(x => x.Title == title);
        }

        public static readonly string DefaultListAttributeNames =
            $"{nameof(LogInfo.Id)},{nameof(LogInfo.AddDate)}";

        public static List<string> GetAllAttributeNames(FormInfo formInfo, List<FieldInfo> fieldInfoList)
        {
            var allAttributeNames = new List<string>
            {
                nameof(LogInfo.Id)
            };
            foreach (var fieldInfo in fieldInfoList)
            {
                allAttributeNames.Add(fieldInfo.Title);
            }
            allAttributeNames.Add(nameof(LogInfo.AddDate));

            return allAttributeNames;
        }

        public static string GetFormTitle(FormInfo formInfo)
        {
            var text = "表单管理 (0)";
            if (formInfo == null) return text;

            if (formInfo.TotalCount == 0)
            {
                formInfo.TotalCount = LogDao.GetCount(formInfo.Id);
                if (formInfo.TotalCount > 0)
                {
                    FormDao.Update(formInfo);
                }
            }

            text = $"{(formInfo.ContentId > 0 ? "表单管理" : formInfo.Title)} ({formInfo.TotalCount})";
            if (!formInfo.IsReply) return text;

            if (formInfo.TotalCount - formInfo.RepliedCount > 0)
            {
                text = $@"<span class=""text-danger"">{text}</span>";
            }

            return text;
        }

        public static string GetTemplateHtml(string templateType, string directoryName)
        {
            var htmlPath = Context.PluginApi.GetPluginPath(FormUtils.PluginId, $"templates/{directoryName}/index.html");

            var html = CacheUtils.Get<string>(htmlPath);
            if (html != null) return html;

            html = FormUtils.ReadText(htmlPath);
            var startIndex = html.IndexOf("<body", StringComparison.Ordinal) + 5;
            var length = html.IndexOf("</body>", StringComparison.Ordinal) - startIndex;
            html = html.Substring(startIndex, length);
            html = html.Substring(html.IndexOf('\n'));

//            var jsPath = Context.PluginApi.GetPluginPath(FormUtils.PluginId, $"assets/js/{templateType}.js");
//            var javascript = FormUtils.ReadText(jsPath);
//            html = html.Replace(
//                $@"<script src=""../../assets/js/{templateType}.js"" type=""text/javascript""></script>",
//                $@"<script type=""text/javascript"">
//{javascript}
//</script>");
            html = html.Replace("../../", "{stl.rootUrl}/SiteFiles/plugins/SS.Form/");
            html = html.Replace("../", "{stl.rootUrl}/SiteFiles/plugins/SS.Form/templates/");

            CacheUtils.InsertHours(htmlPath, html, 1);
            return html;
        }

        public static void UpdateCache(FormInfo formInfo)
        {
            FormManagerCache.Update(formInfo);
        }

        public static void ClearCache(int siteId)
        {
            FormManagerCache.Clear(siteId);
        }

        public static void Notify(FormInfo formInfo, LogInfo logInfo)
        {
            if (formInfo.Additional.IsAdministratorSmsNotify && !string.IsNullOrEmpty(formInfo.Additional.AdministratorSmsNotifyTplId) && !string.IsNullOrEmpty(formInfo.Additional.AdministratorSmsNotifyKeys) && !string.IsNullOrEmpty(formInfo.Additional.AdministratorSmsNotifyMobile))
            {
                var smsPlugin = Context.PluginApi.GetPlugin<SmsPlugin>();
                if (smsPlugin != null && smsPlugin.IsReady)
                {
                    var parameters = new Dictionary<string, string>();
                    var keys = formInfo.Additional.AdministratorSmsNotifyKeys.Split(',');
                    foreach (var key in keys)
                    {
                        if (key == nameof(LogInfo.Id))
                        {
                            parameters.Add(key, logInfo.Id.ToString());
                        }
                        else if (key == nameof(LogInfo.AddDate))
                        {
                            parameters.Add(key, logInfo.AddDate.ToString("yyyy-MM-dd HH:mm"));
                        }
                        else
                        {
                            parameters.Add(key, logInfo.GetString(key));
                        }
                    }
                    smsPlugin.Send(formInfo.Additional.AdministratorSmsNotifyMobile, formInfo.Additional.AdministratorSmsNotifyTplId, parameters, out _);
                }
            }

            if (formInfo.Additional.IsAdministratorMailNotify && !string.IsNullOrEmpty(formInfo.Additional.AdministratorMailNotifyAddress))
            {
                var mailPlugin = Context.PluginApi.GetPlugin<MailPlugin>();
                if (mailPlugin != null && mailPlugin.IsReady)
                {
                    var fieldInfoList = FieldManager.GetFieldInfoList(formInfo.Id);

                    var templateHtml = MailTemplateManager.GetTemplateHtml();
                    var listHtml = MailTemplateManager.GetListHtml();

                    var keyValueList = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("编号", logInfo.Id.ToString()),
                        new KeyValuePair<string, string>("提交时间", logInfo.AddDate.ToString("yyyy-MM-dd HH:mm"))
                    };
                    foreach (var fieldInfo in fieldInfoList)
                    {
                        keyValueList.Add(new KeyValuePair<string, string>(fieldInfo.Title, LogManager.GetValue(fieldInfo, logInfo)));
                    }

                    var list = new StringBuilder();
                    foreach (var kv in keyValueList)
                    {
                        list.Append(listHtml.Replace("{{key}}", kv.Key).Replace("{{value}}", kv.Value));
                    }

                    mailPlugin.Send(formInfo.Additional.AdministratorMailNotifyAddress, string.Empty, "[SiteServer CMS] 通知邮件", templateHtml.Replace("{{title}}", formInfo.Title).Replace("{{list}}", list.ToString()), out _);
                }
            }
        }
    }
}
