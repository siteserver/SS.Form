using System;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Core.Utils;

namespace SS.Form.Controllers.Pages
{
    [RoutePrefix("pages/fieldsLayerValidate")]
    public class PagesFieldsLayerValidateController : ApiController
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

                var fieldId = Request.GetQueryInt("fieldId");
                var fieldInfo = FieldManager.GetFieldInfo(formInfo.Id, fieldId);

                var veeValidate = string.Empty;
                if (fieldInfo != null)
                {
                    veeValidate = fieldInfo.Validate;
                }

                return Ok(new
                {
                    Value = veeValidate
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

                var fieldId = Request.GetPostInt("fieldId");
                var value = Request.GetPostString("value");

                var fieldInfo = FieldManager.GetFieldInfo(formInfo.Id, fieldId);
                fieldInfo.Validate = value;

                FieldManager.Repository.Update(fieldInfo, false);

                return Ok(new{});
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
