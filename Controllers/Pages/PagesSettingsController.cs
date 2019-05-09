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
                var request = Context.AuthenticatedRequest;

                var formInfo = FormManager.GetFormInfoByGet(request);
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
                var request = Context.AuthenticatedRequest;

                var formInfo = FormManager.GetFormInfoByPost(request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.PluginId)) return Unauthorized();

                var type = request.GetPostString("type");
                if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.IsClosed)))
                {
                    formInfo.IsClosed = request.GetPostBool(nameof(FormInfo.IsClosed));
                    FormManager.Repository.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.Title)))
                {
                    formInfo.Title = request.GetPostString(nameof(FormInfo.Title));
                    FormManager.Repository.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.Description)))
                {
                    formInfo.Description = request.GetPostString(nameof(FormInfo.Description));
                    FormManager.Repository.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.IsReply)))
                {
                    formInfo.IsReply = request.GetPostBool(nameof(FormInfo.IsReply));
                    FormManager.Repository.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.PageSize)))
                {
                    formInfo.PageSize = request.GetPostInt(nameof(FormInfo.PageSize));
                    FormManager.Repository.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.IsTimeout)))
                {
                    formInfo.IsTimeout = request.GetPostBool(nameof(FormInfo.IsTimeout));
                    formInfo.TimeToStart = FormUtils.ToDateTime(request.GetPostString(nameof(FormInfo.TimeToStart)));
                    formInfo.TimeToEnd = FormUtils.ToDateTime(request.GetPostString(nameof(FormInfo.TimeToEnd)));
                    FormManager.Repository.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.IsCaptcha)))
                {
                    formInfo.IsCaptcha = request.GetPostBool(nameof(FormInfo.IsCaptcha));
                    FormManager.Repository.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.IsAdministratorSmsNotify)))
                {
                    formInfo.IsAdministratorSmsNotify = request.GetPostBool(nameof(FormInfo.IsAdministratorSmsNotify));
                    formInfo.AdministratorSmsNotifyTplId = request.GetPostString(nameof(FormInfo.AdministratorSmsNotifyTplId));
                    formInfo.AdministratorSmsNotifyKeys = request.GetPostString(nameof(FormInfo.AdministratorSmsNotifyKeys));
                    formInfo.AdministratorSmsNotifyMobile = request.GetPostString(nameof(FormInfo.AdministratorSmsNotifyMobile));

                    FormManager.Repository.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.IsAdministratorMailNotify)))
                {
                    formInfo.IsAdministratorMailNotify = request.GetPostBool(nameof(FormInfo.IsAdministratorMailNotify));
                    formInfo.AdministratorMailNotifyAddress = request.GetPostString(nameof(FormInfo.AdministratorMailNotifyAddress));

                    FormManager.Repository.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.IsUserSmsNotify)))
                {
                    formInfo.IsUserSmsNotify = request.GetPostBool(nameof(FormInfo.IsUserSmsNotify));
                    formInfo.UserSmsNotifyTplId = request.GetPostString(nameof(FormInfo.UserSmsNotifyTplId));
                    formInfo.UserSmsNotifyKeys = request.GetPostString(nameof(FormInfo.UserSmsNotifyKeys));
                    formInfo.UserSmsNotifyMobileName = request.GetPostString(nameof(FormInfo.UserSmsNotifyMobileName));

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
