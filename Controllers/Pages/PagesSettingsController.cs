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

                var administratorSmsAttributeNames = FormManager.GetAllAttributeNames(formInfo, fieldInfoList);
                administratorSmsAttributeNames.Remove(nameof(LogInfo.IsReplied));
                administratorSmsAttributeNames.Remove(nameof(LogInfo.ReplyDate));
                administratorSmsAttributeNames.Remove(nameof(LogInfo.ReplyContent));
                var administratorSmsNotifyKeys =
                    FormUtils.StringCollectionToStringList(formInfo.AdministratorSmsNotifyKeys);

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
                var request = Request.GetAuthenticatedRequest();

                var formInfo = FormManager.GetFormInfoByPost(Request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.PluginId)) return Unauthorized();

                var type = Request.GetPostString("type");
                if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.IsClosed)))
                {
                    formInfo.IsClosed = Request.GetPostBool(nameof(FormInfo.IsClosed).ToCamelCase());
                    FormManager.Repository.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.Title)))
                {
                    formInfo.Title = Request.GetPostString(nameof(FormInfo.Title).ToCamelCase());
                    FormManager.Repository.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.Description)))
                {
                    formInfo.Description = Request.GetPostString(nameof(FormInfo.Description).ToCamelCase());
                    FormManager.Repository.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.IsReply)))
                {
                    formInfo.IsReply = Request.GetPostBool(nameof(FormInfo.IsReply).ToCamelCase());
                    FormManager.Repository.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.IsTimeout)))
                {
                    formInfo.IsTimeout = Request.GetPostBool(nameof(FormInfo.IsTimeout).ToCamelCase());
                    formInfo.TimeToStart = FormUtils.ToDateTime(Request.GetPostString(nameof(FormInfo.TimeToStart).ToCamelCase()));
                    formInfo.TimeToEnd = FormUtils.ToDateTime(Request.GetPostString(nameof(FormInfo.TimeToEnd).ToCamelCase()));
                    FormManager.Repository.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.IsCaptcha)))
                {
                    formInfo.IsCaptcha = Request.GetPostBool(nameof(FormInfo.IsCaptcha).ToCamelCase());
                    FormManager.Repository.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.IsAdministratorSmsNotify)))
                {
                    formInfo.IsAdministratorSmsNotify = Request.GetPostBool(nameof(FormInfo.IsAdministratorSmsNotify).ToCamelCase());
                    formInfo.AdministratorSmsNotifyTplId = Request.GetPostString(nameof(FormInfo.AdministratorSmsNotifyTplId).ToCamelCase());
                    formInfo.AdministratorSmsNotifyKeys = Request.GetPostString(nameof(FormInfo.AdministratorSmsNotifyKeys).ToCamelCase());
                    formInfo.AdministratorSmsNotifyMobile = Request.GetPostString(nameof(FormInfo.AdministratorSmsNotifyMobile).ToCamelCase());

                    FormManager.Repository.Update(formInfo);
                }
                else if (FormUtils.EqualsIgnoreCase(type, nameof(FormInfo.IsAdministratorMailNotify)))
                {
                    formInfo.IsAdministratorMailNotify = Request.GetPostBool(nameof(FormInfo.IsAdministratorMailNotify).ToCamelCase());
                    formInfo.AdministratorMailNotifyAddress = Request.GetPostString(nameof(FormInfo.AdministratorMailNotifyAddress).ToCamelCase());

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
