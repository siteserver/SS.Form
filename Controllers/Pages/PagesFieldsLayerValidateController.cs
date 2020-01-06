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
                var request = Context.AuthenticatedRequest;

                var formInfo = FormManager.GetFormInfoByGet(request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.MenuFormsPermission)) return Unauthorized();

                var fieldId = request.GetQueryInt("fieldId");
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
                var request = Context.AuthenticatedRequest;

                var formInfo = FormManager.GetFormInfoByPost(request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.MenuFormsPermission)) return Unauthorized();

                var fieldId = request.GetPostInt("fieldId");
                var value = request.GetPostString("value");

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
