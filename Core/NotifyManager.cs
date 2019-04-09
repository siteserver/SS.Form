using System.Collections.Generic;
using System.Linq;
using System.Text;
using SiteServer.Plugin;
using SS.Form.Core.Model;
using SS.Form.Core.Utils;
using SS.Mail;
using SS.SMS;

namespace SS.Form.Core
{
    public static class NotifyManager
    {
        public static void SendNotify(FormInfo formInfo, List<FieldInfo> fieldInfoList, LogInfo logInfo)
        {
            if (formInfo.IsAdministratorSmsNotify &&
                !string.IsNullOrEmpty(formInfo.AdministratorSmsNotifyTplId) &&
                !string.IsNullOrEmpty(formInfo.AdministratorSmsNotifyMobile))
            {
                var smsPlugin = Context.PluginApi.GetPlugin<SMS.Plugin>();
                if (smsPlugin != null && smsPlugin.IsReady)
                {
                    var parameters = new Dictionary<string, string>();
                    if (!string.IsNullOrEmpty(formInfo.AdministratorSmsNotifyKeys))
                    {
                        var keys = formInfo.AdministratorSmsNotifyKeys.Split(',');
                        foreach (var key in keys)
                        {
                            if (FormUtils.EqualsIgnoreCase(key, nameof(LogInfo.Id)))
                            {
                                parameters.Add(key, logInfo.Id.ToString());
                            }
                            else if (FormUtils.EqualsIgnoreCase(key, nameof(LogInfo.AddDate)))
                            {
                                if (logInfo.AddDate.HasValue)
                                {
                                    parameters.Add(key, logInfo.AddDate.Value.ToString("yyyy-MM-dd HH:mm"));
                                }
                            }
                            else
                            {
                                var value = string.Empty;
                                var fieldInfo =
                                    fieldInfoList.FirstOrDefault(x => FormUtils.EqualsIgnoreCase(key, x.Title));
                                if (fieldInfo != null)
                                {
                                    value = LogManager.GetValue(fieldInfo, logInfo);
                                }

                                parameters.Add(key, value);
                            }
                        }
                    }

                    smsPlugin.Send(formInfo.AdministratorSmsNotifyMobile,
                        formInfo.AdministratorSmsNotifyTplId, parameters, out _);
                }
            }

            if (formInfo.IsAdministratorMailNotify &&
                !string.IsNullOrEmpty(formInfo.AdministratorMailNotifyAddress))
            {
                var mailPlugin = Context.PluginApi.GetPlugin<Mail.Plugin>();
                if (mailPlugin != null && mailPlugin.IsReady)
                {
                    var templateHtml = MailTemplateManager.GetTemplateHtml();
                    var listHtml = MailTemplateManager.GetListHtml();

                    var keyValueList = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("编号", logInfo.Id.ToString())
                    };
                    if (logInfo.AddDate.HasValue)
                    {
                        keyValueList.Add(new KeyValuePair<string, string>("提交时间", logInfo.AddDate.Value.ToString("yyyy-MM-dd HH:mm")));
                    }
                    foreach (var fieldInfo in fieldInfoList)
                    {
                        keyValueList.Add(new KeyValuePair<string, string>(fieldInfo.Title,
                            LogManager.GetValue(fieldInfo, logInfo)));
                    }

                    var list = new StringBuilder();
                    foreach (var kv in keyValueList)
                    {
                        list.Append(listHtml.Replace("{{key}}", kv.Key).Replace("{{value}}", kv.Value));
                    }

                    var siteInfo = Context.SiteApi.GetSiteInfo(formInfo.SiteId);

                    mailPlugin.Send(formInfo.AdministratorMailNotifyAddress, string.Empty,
                        "[SiteServer CMS] 通知邮件",
                        templateHtml.Replace("{{title}}", $"{formInfo.Title} - {siteInfo.SiteName}").Replace("{{list}}", list.ToString()), out _);
                }
            }

            if (formInfo.IsUserSmsNotify &&
                !string.IsNullOrEmpty(formInfo.UserSmsNotifyTplId) &&
                !string.IsNullOrEmpty(formInfo.UserSmsNotifyMobileName))
            {
                var smsPlugin = Context.PluginApi.GetPlugin<SMS.Plugin>();
                if (smsPlugin != null && smsPlugin.IsReady)
                {
                    var parameters = new Dictionary<string, string>();
                    if (!string.IsNullOrEmpty(formInfo.UserSmsNotifyKeys))
                    {
                        var keys = formInfo.UserSmsNotifyKeys.Split(',');
                        foreach (var key in keys)
                        {
                            if (FormUtils.EqualsIgnoreCase(key, nameof(LogInfo.Id)))
                            {
                                parameters.Add(key, logInfo.Id.ToString());
                            }
                            else if (FormUtils.EqualsIgnoreCase(key, nameof(LogInfo.AddDate)))
                            {
                                if (logInfo.AddDate.HasValue)
                                {
                                    parameters.Add(key, logInfo.AddDate.Value.ToString("yyyy-MM-dd HH:mm"));
                                }
                            }
                            else
                            {
                                var value = string.Empty;
                                var fieldInfo =
                                    fieldInfoList.FirstOrDefault(x => FormUtils.EqualsIgnoreCase(key, x.Title));
                                if (fieldInfo != null)
                                {
                                    value = LogManager.GetValue(fieldInfo, logInfo);
                                }

                                parameters.Add(key, value);
                            }
                        }
                    }

                    var mobileFieldInfo = fieldInfoList.FirstOrDefault(x => FormUtils.EqualsIgnoreCase(formInfo.UserSmsNotifyMobileName, x.Title));
                    if (mobileFieldInfo != null)
                    {
                        var mobile = LogManager.GetValue(mobileFieldInfo, logInfo);
                        if (!string.IsNullOrEmpty(mobile))
                        {
                            smsPlugin.Send(mobile, formInfo.UserSmsNotifyTplId, parameters, out _);
                        }
                    }
                }
            }
        }
    }
}
