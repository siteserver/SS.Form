using System;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Core.Model;
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
                var request = Request.GetAuthenticatedRequest();

                var formInfo = FormManager.GetFormInfoByGet(Request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.PluginId)) return Unauthorized();

                var fieldInfoList = FieldManager.GetFieldInfoList(formInfo.Id);

                var attributeNames = FormManager.GetAllAttributeNames(fieldInfoList);
                attributeNames.Remove(nameof(LogInfo.IsReplied));
                attributeNames.Remove(nameof(LogInfo.ReplyDate));
                attributeNames.Remove(nameof(LogInfo.ReplyContent));
                var administratorSmsNotifyKeys =
                    FormUtils.StringCollectionToStringList(formInfo.AdministratorSmsNotifyKeys);
                var userSmsNotifyKeys =
                    FormUtils.StringCollectionToStringList(formInfo.UserSmsNotifyKeys);

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
                var request = Request.GetAuthenticatedRequest();

                var formInfo = FormManager.GetFormInfoByPost(Request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.PluginId)) return Unauthorized();

                var type = Request.GetPostString("type");
                if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.IsClosed)))
                {
                    formInfo.IsClosed = Request.GetPostBool(nameof(FormInfo.IsClosed));
                    FormManager.Repository.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.Title)))
                {
                    formInfo.Title = Request.GetPostString(nameof(FormInfo.Title));
                    FormManager.Repository.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.Description)))
                {
                    formInfo.Description = Request.GetPostString(nameof(FormInfo.Description));
                    FormManager.Repository.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.IsReply)))
                {
                    formInfo.IsReply = Request.GetPostBool(nameof(FormInfo.IsReply));
                    FormManager.Repository.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.IsTimeout)))
                {
                    formInfo.IsTimeout = Request.GetPostBool(nameof(FormInfo.IsTimeout));
                    formInfo.TimeToStart = FormUtils.ToDateTime(Request.GetPostString(nameof(FormInfo.TimeToStart)));
                    formInfo.TimeToEnd = FormUtils.ToDateTime(Request.GetPostString(nameof(FormInfo.TimeToEnd)));
                    FormManager.Repository.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.IsCaptcha)))
                {
                    formInfo.IsCaptcha = Request.GetPostBool(nameof(FormInfo.IsCaptcha));
                    FormManager.Repository.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.IsAdministratorSmsNotify)))
                {
                    formInfo.IsAdministratorSmsNotify = Request.GetPostBool(nameof(FormInfo.IsAdministratorSmsNotify));
                    formInfo.AdministratorSmsNotifyTplId = Request.GetPostString(nameof(FormInfo.AdministratorSmsNotifyTplId));
                    formInfo.AdministratorSmsNotifyKeys = Request.GetPostString(nameof(FormInfo.AdministratorSmsNotifyKeys));
                    formInfo.AdministratorSmsNotifyMobile = Request.GetPostString(nameof(FormInfo.AdministratorSmsNotifyMobile));

                    FormManager.Repository.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.IsAdministratorMailNotify)))
                {
                    formInfo.IsAdministratorMailNotify = Request.GetPostBool(nameof(FormInfo.IsAdministratorMailNotify));
                    formInfo.AdministratorMailNotifyAddress = Request.GetPostString(nameof(FormInfo.AdministratorMailNotifyAddress));

                    FormManager.Repository.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.IsUserSmsNotify)))
                {
                    formInfo.IsUserSmsNotify = Request.GetPostBool(nameof(FormInfo.IsUserSmsNotify));
                    formInfo.UserSmsNotifyTplId = Request.GetPostString(nameof(FormInfo.UserSmsNotifyTplId));
                    formInfo.UserSmsNotifyKeys = Request.GetPostString(nameof(FormInfo.UserSmsNotifyKeys));
                    formInfo.UserSmsNotifyMobileName = Request.GetPostString(nameof(FormInfo.UserSmsNotifyMobileName));

                    FormManager.Repository.Update(formInfo);
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
