using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Core.Utils;

namespace SS.Form.Controllers.Pages
{
    [RoutePrefix("pages/fields")]
    public class PagesFieldsController : ApiController
    {
        private const string Route = "";
        private const string RouteExport = "actions/export";
        private const string RouteImport = "actions/import";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = Request.GetAuthenticatedRequest();

                var formInfo = FormManager.GetFormInfoByGet(Request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.PluginId)) return Unauthorized();

                var adminToken = Context.AdminApi.GetAccessToken(request.AdminId, request.AdminName, TimeSpan.FromDays(1));

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
                    Value = list,
                    AdminToken = adminToken
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
                var request = Request.GetAuthenticatedRequest();

                var formInfo = FormManager.GetFormInfoByGet(Request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.PluginId)) return Unauthorized();

                var fieldId = Request.GetQueryInt("fieldId");
                FieldManager.Repository.Delete(fieldId);
                FieldManager.ClearCache(formInfo.Id);

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

        [HttpPost, Route(RouteExport)]
        public IHttpActionResult Export()
        {
            try
            {
                var request = Request.GetAuthenticatedRequest();

                var formInfo = FormManager.GetFormInfoByPost(Request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.PluginId)) return Unauthorized();

                var fileName = FieldManager.Export(formInfo.Id);
                var url = Context.UtilsApi.GetRootUrl($"SiteFiles/TemporaryFiles/{fileName}");

                return Ok(new
                {
                    Value = url
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteImport)]
        public IHttpActionResult Import()
        {
            try
            {
                var request = Request.GetAuthenticatedRequest();

                var formInfo = FormManager.GetFormInfoByGet(Request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.PluginId)) return Unauthorized();

                foreach (string name in HttpContext.Current.Request.Files)
                {
                    var postFile = HttpContext.Current.Request.Files[name];

                    if (postFile == null)
                    {
                        return BadRequest("Could not read zip from body");
                    }
                    
                    var filePath = Context.UtilsApi.GetTemporaryFilesPath("表单字段.zip");
                    FormUtils.DeleteFileIfExists(filePath);

                    if (!FormUtils.EqualsIgnoreCase(Path.GetExtension(postFile.FileName), ".zip"))
                    {
                        return BadRequest("zip file extension is not correct");
                    }

                    postFile.SaveAs(filePath);

                    FieldManager.Import(formInfo.SiteId, formInfo.Id, filePath);
                }

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
