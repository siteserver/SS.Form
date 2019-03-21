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
            if (formInfo.Additional.IsAdministratorSmsNotify &&
                !string.IsNullOrEmpty(formInfo.Additional.AdministratorSmsNotifyTplId) &&
                !string.IsNullOrEmpty(formInfo.Additional.AdministratorSmsNotifyMobile))
            {
                var smsPlugin = Context.PluginApi.GetPlugin<SmsPlugin>();
                if (smsPlugin != null && smsPlugin.IsReady)
                {
                    var parameters = new Dictionary<string, string>();
                    if (!string.IsNullOrEmpty(formInfo.Additional.AdministratorSmsNotifyKeys))
                    {
                        var keys = formInfo.Additional.AdministratorSmsNotifyKeys.Split(',');
                        foreach (var key in keys)
                        {
                            if (FormUtils.EqualsIgnoreCase(key, nameof(LogInfo.Id)))
                            {
                                parameters.Add(key, logInfo.Id.ToString());
                            }
                            else if (FormUtils.EqualsIgnoreCase(key, nameof(LogInfo.AddDate)))
                            {
                                parameters.Add(key, logInfo.AddDate.ToString("yyyy-MM-dd HH:mm"));
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

                    smsPlugin.Send(formInfo.Additional.AdministratorSmsNotifyMobile,
                        formInfo.Additional.AdministratorSmsNotifyTplId, parameters, out _);
                }
            }

            if (formInfo.Additional.IsAdministratorMailNotify &&
                !string.IsNullOrEmpty(formInfo.Additional.AdministratorMailNotifyAddress))
            {
                var mailPlugin = Context.PluginApi.GetPlugin<MailPlugin>();
                if (mailPlugin != null && mailPlugin.IsReady)
                {
                    var templateHtml = MailTemplateManager.GetTemplateHtml();
                    var listHtml = MailTemplateManager.GetListHtml();

                    var keyValueList = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("编号", logInfo.Id.ToString()),
                        new KeyValuePair<string, string>("提交时间", logInfo.AddDate.ToString("yyyy-MM-dd HH:mm"))
                    };
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

                    mailPlugin.Send(formInfo.Additional.AdministratorMailNotifyAddress, string.Empty,
                        "[SiteServer CMS] 通知邮件",
                        templateHtml.Replace("{{title}}", $"{formInfo.Title} - {siteInfo.SiteName}").Replace("{{list}}", list.ToString()), out _);
                }
            }

            if (formInfo.Additional.IsUserSmsNotify &&
                !string.IsNullOrEmpty(formInfo.Additional.UserSmsNotifyTplId) &&
                !string.IsNullOrEmpty(formInfo.Additional.UserSmsNotifyMobileName))
            {
                var smsPlugin = Context.PluginApi.GetPlugin<SmsPlugin>();
                if (smsPlugin != null && smsPlugin.IsReady)
                {
                    var parameters = new Dictionary<string, string>();
                    if (!string.IsNullOrEmpty(formInfo.Additional.UserSmsNotifyKeys))
                    {
                        var keys = formInfo.Additional.UserSmsNotifyKeys.Split(',');
                        foreach (var key in keys)
                        {
                            if (FormUtils.EqualsIgnoreCase(key, nameof(LogInfo.Id)))
                            {
                                parameters.Add(key, logInfo.Id.ToString());
                            }
                            else if (FormUtils.EqualsIgnoreCase(key, nameof(LogInfo.AddDate)))
                            {
                                parameters.Add(key, logInfo.AddDate.ToString("yyyy-MM-dd HH:mm"));
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

                    var mobileFieldInfo = fieldInfoList.FirstOrDefault(x => FormUtils.EqualsIgnoreCase(formInfo.Additional.UserSmsNotifyMobileName, x.Title));
                    if (mobileFieldInfo != null)
                    {
                        var mobile = LogManager.GetValue(mobileFieldInfo, logInfo);
                        if (!string.IsNullOrEmpty(mobile))
                        {
                            smsPlugin.Send(mobile, formInfo.Additional.UserSmsNotifyTplId, parameters, out _);
                        }
                    }
                }
            }
        }
    }
}
