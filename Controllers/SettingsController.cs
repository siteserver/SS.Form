using System;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Core.Model;
using SS.Form.Core.Provider;
using SS.Form.Core.Utils;

namespace SS.Form.Controllers
{
    [RoutePrefix("settings")]
    public class SettingsController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = Context.GetCurrentRequest();
                var formInfo = FormManager.GetFormInfoByGet(request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.PluginId)) return Unauthorized();

                var fieldInfoList = FieldManager.GetFieldInfoList(formInfo.Id);

                var administratorSmsAttributeNames = FormManager.GetAllAttributeNames(formInfo, fieldInfoList);
                administratorSmsAttributeNames.Remove(nameof(LogInfo.IsReplied));
                administratorSmsAttributeNames.Remove(nameof(LogInfo.ReplyDate));
                administratorSmsAttributeNames.Remove(nameof(LogInfo.ReplyContent));
                var administratorSmsNotifyKeys =
                    FormUtils.StringCollectionToStringList(formInfo.Additional.AdministratorSmsNotifyKeys);

                //try
                //{
                //    var smsPlugin = Context.PluginApi.GetPlugin<SmsPlugin>();
                //    if (smsPlugin != null)
                //    {
                //        isSmsAvaliable = true;
                //    }
                //}
                //catch
                //{
                //    // ignored
                //}

                return Ok(new
                {
                    Value = formInfo,
                    FieldInfoList = fieldInfoList,
                    AdministratorSmsAttributeNames = administratorSmsAttributeNames,
                    AdministratorSmsNotifyKeys = administratorSmsNotifyKeys
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit()
        {
            try
            {
                var request = Context.GetCurrentRequest();
                var formInfo = FormManager.GetFormInfoByPost(request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.PluginId)) return Unauthorized();

                var type = request.GetPostString("type");
                if (FormUtils.EqualsIgnoreCase(type, nameof(FormSettings.IsClosed)))
                {
                    formInfo.Additional.IsClosed = request.GetPostBool(nameof(FormSettings.IsClosed).ToCamelCase());
                    FormDao.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.Title)))
                {
                    formInfo.Title = request.GetPostString(nameof(FormInfo.Title).ToCamelCase());
                    FormDao.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.Description)))
                {
                    formInfo.Description = request.GetPostString(nameof(FormInfo.Description).ToCamelCase());
                    FormDao.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.IsReply)))
                {
                    formInfo.IsReply = request.GetPostBool(nameof(FormInfo.IsReply).ToCamelCase());
                    FormDao.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormSettings.IsTimeout)))
                {
                    formInfo.Additional.IsTimeout = request.GetPostBool(nameof(FormSettings.IsTimeout).ToCamelCase());
                    formInfo.Additional.TimeToStart = FormUtils.ToDateTime(request.GetPostString(nameof(FormSettings.TimeToStart).ToCamelCase()));
                    formInfo.Additional.TimeToEnd = FormUtils.ToDateTime(request.GetPostString(nameof(FormSettings.TimeToEnd).ToCamelCase()));
                    FormDao.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormSettings.IsCaptcha)))
                {
                    formInfo.Additional.IsCaptcha = request.GetPostBool(nameof(FormSettings.IsCaptcha).ToCamelCase());
                    FormDao.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormSettings.IsAdministratorSmsNotify)))
                {
                    formInfo.Additional.IsAdministratorSmsNotify = request.GetPostBool(nameof(FormSettings.IsAdministratorSmsNotify).ToCamelCase());
                    formInfo.Additional.AdministratorSmsNotifyTplId = request.GetPostString(nameof(FormSettings.AdministratorSmsNotifyTplId).ToCamelCase());
                    formInfo.Additional.AdministratorSmsNotifyKeys = request.GetPostString(nameof(FormSettings.AdministratorSmsNotifyKeys).ToCamelCase());
                    formInfo.Additional.AdministratorSmsNotifyMobile = request.GetPostString(nameof(FormSettings.AdministratorSmsNotifyMobile).ToCamelCase());

                    FormDao.Update(formInfo);
                }

                return Ok(new{});
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
