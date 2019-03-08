using System;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Core.Model;
using SS.Form.Core.Provider;
using SS.Form.Core.Utils;

namespace SS.Form.Controllers.Pages
{
    [RoutePrefix("pages/settings")]
    public class PagesSettingsController : ApiController
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

                var attributeNames = FormManager.GetAllAttributeNames(fieldInfoList);
                attributeNames.Remove(nameof(LogInfo.IsReplied));
                attributeNames.Remove(nameof(LogInfo.ReplyDate));
                attributeNames.Remove(nameof(LogInfo.ReplyContent));
                var administratorSmsNotifyKeys =
                    FormUtils.StringCollectionToStringList(formInfo.Additional.AdministratorSmsNotifyKeys);
                var userSmsNotifyKeys =
                    FormUtils.StringCollectionToStringList(formInfo.Additional.UserSmsNotifyKeys);

                return Ok(new
                {
                    Value = formInfo,
                    FieldInfoList = fieldInfoList,
                    AttributeNames = attributeNames,
                    AdministratorSmsNotifyKeys = administratorSmsNotifyKeys,
                    UserSmsNotifyKeys = userSmsNotifyKeys
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
                    formInfo.Additional.IsClosed = request.GetPostBool(nameof(FormSettings.IsClosed));
                    FormDao.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.Title)))
                {
                    formInfo.Title = request.GetPostString(nameof(FormInfo.Title));
                    FormDao.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.Description)))
                {
                    formInfo.Description = request.GetPostString(nameof(FormInfo.Description));
                    FormDao.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.IsReply)))
                {
                    formInfo.IsReply = request.GetPostBool(nameof(FormInfo.IsReply));
                    FormDao.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormSettings.IsTimeout)))
                {
                    formInfo.Additional.IsTimeout = request.GetPostBool(nameof(FormSettings.IsTimeout));
                    formInfo.Additional.TimeToStart = FormUtils.ToDateTime(request.GetPostString(nameof(FormSettings.TimeToStart)));
                    formInfo.Additional.TimeToEnd = FormUtils.ToDateTime(request.GetPostString(nameof(FormSettings.TimeToEnd)));
                    FormDao.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormSettings.IsCaptcha)))
                {
                    formInfo.Additional.IsCaptcha = request.GetPostBool(nameof(FormSettings.IsCaptcha));
                    FormDao.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormSettings.IsAdministratorSmsNotify)))
                {
                    formInfo.Additional.IsAdministratorSmsNotify = request.GetPostBool(nameof(FormSettings.IsAdministratorSmsNotify));
                    formInfo.Additional.AdministratorSmsNotifyTplId = request.GetPostString(nameof(FormSettings.AdministratorSmsNotifyTplId));
                    formInfo.Additional.AdministratorSmsNotifyKeys = request.GetPostString(nameof(FormSettings.AdministratorSmsNotifyKeys));
                    formInfo.Additional.AdministratorSmsNotifyMobile = request.GetPostString(nameof(FormSettings.AdministratorSmsNotifyMobile));

                    FormDao.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormSettings.IsAdministratorMailNotify)))
                {
                    formInfo.Additional.IsAdministratorMailNotify = request.GetPostBool(nameof(FormSettings.IsAdministratorMailNotify));
                    formInfo.Additional.AdministratorMailNotifyAddress = request.GetPostString(nameof(FormSettings.AdministratorMailNotifyAddress));

                    FormDao.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormSettings.IsUserSmsNotify)))
                {
                    formInfo.Additional.IsUserSmsNotify = request.GetPostBool(nameof(FormSettings.IsUserSmsNotify));
                    formInfo.Additional.UserSmsNotifyTplId = request.GetPostString(nameof(FormSettings.UserSmsNotifyTplId));
                    formInfo.Additional.UserSmsNotifyKeys = request.GetPostString(nameof(FormSettings.UserSmsNotifyKeys));
                    formInfo.Additional.UserSmsNotifyMobileName = request.GetPostString(nameof(FormSettings.UserSmsNotifyMobileName));

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
