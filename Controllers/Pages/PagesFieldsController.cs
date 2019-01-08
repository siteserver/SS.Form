using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Core.Provider;
using SS.Form.Core.Utils;

namespace SS.Form.Controllers.Pages
{
    [RoutePrefix("pages/fields")]
    public class PagesFieldsController : ApiController
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

                var list = new List<object>();
                foreach (var fieldInfo in FieldManager.GetFieldInfoList(formInfo.Id))
                {
                    list.Add(new
                    {
                        fieldInfo.Id,
                        fieldInfo.Title,
                        InputType = FormUtils.GetFieldTypeText(fieldInfo.FieldType),
                        fieldInfo.Validate,
                        fieldInfo.Taxis
                    });
                }

                return Ok(new
                {
                    Value = list
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(Route)]
        public IHttpActionResult Delete()
        {
            try
            {
                var request = Context.GetCurrentRequest();
                var formInfo = FormManager.GetFormInfoByGet(request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.PluginId)) return Unauthorized();

                var fieldId = request.GetQueryInt("fieldId");
                FieldDao.Delete(fieldId);

                var list = new List<object>();
                foreach (var fieldInfo in FieldManager.GetFieldInfoList(formInfo.Id))
                {
                    list.Add(new
                    {
                        fieldInfo.Id,
                        fieldInfo.Title,
                        InputType = FormUtils.GetFieldTypeText(fieldInfo.FieldType),
                        fieldInfo.Validate,
                        fieldInfo.Taxis
                    });
                }

                return Ok(new
                {
                    Value = list
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
