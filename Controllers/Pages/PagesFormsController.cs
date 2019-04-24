using System;
using System.IO;
using System.Web;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Core.ImportExport;
using SS.Form.Core.Model;
using SS.Form.Core.Utils;

namespace SS.Form.Controllers.Pages
{
    [RoutePrefix("pages/forms")]
    public class PagesFormsController : ApiController
    {
        private const string Route = "";
        private const string RouteActionsUp = "actions/up";
        private const string RouteActionsDown = "actions/down";
        private const string RouteExport = "actions/export";
        private const string RouteImport = "actions/import";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId))
                {
                    return Unauthorized();
                }
                
                var formInfoList = FormManager.GetFormInfoList(siteId, 0);
                var adminToken = Context.AdminApi.GetAccessToken(request.AdminId, request.AdminName, TimeSpan.FromDays(1));

                return Ok(new
                {
                    Value = formInfoList,
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
                var request = Context.AuthenticatedRequest;

                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId))
                {
                    return Unauthorized();
                }

                var formId = request.GetQueryInt("formId");

                FormManager.Repository.Delete(siteId, formId);

                return Ok(new
                {
                    Value = FormManager.GetFormInfoList(siteId, 0)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Add()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var siteId = request.GetPostInt("siteId");
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId))
                {
                    return Unauthorized();
                }

                var formInfo = new FormInfo
                {
                    SiteId = siteId,
                    AddDate = DateTime.Now,
                    Title = request.GetPostString("title"),
                    Description = request.GetPostString("description")
                };

                FormManager.Repository.Insert(formInfo);

                return Ok(new
                {
                    Value = FormManager.GetFormInfoList(siteId, 0)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route(Route)]
        public IHttpActionResult Edit()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var siteId = request.GetPostInt("siteId");
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId))
                {
                    return Unauthorized();
                }
                
                var formId = request.GetPostInt("formId");
                var formInfo = FormManager.GetFormInfo(siteId, formId);
                formInfo.Title = request.GetPostString("title");
                formInfo.Description = request.GetPostString("description");

                FormManager.Repository.Update(formInfo);

                return Ok(new
                {
                    Value = FormManager.GetFormInfoList(siteId, 0)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsUp)]
        public IHttpActionResult Up()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var siteId = request.GetPostInt("siteId");
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId))
                {
                    return Unauthorized();
                }

                var formId = request.GetPostInt("formId");

                FormManager.Repository.UpdateTaxisToUp(siteId, formId);

                return Ok(new
                {
                    Value = FormManager.GetFormInfoList(siteId, 0)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsDown)]
        public IHttpActionResult Down()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var siteId = request.GetPostInt("siteId");
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId))
                {
                    return Unauthorized();
                }

                var formId = request.GetPostInt("formId");

                FormManager.Repository.UpdateTaxisToDown(siteId, formId);

                return Ok(new
                {
                    Value = FormManager.GetFormInfoList(siteId, 0)
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
                var request = Context.AuthenticatedRequest;

                var formInfo = FormManager.GetFormInfoByPost(request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.PluginId)) return Unauthorized();

                var fileName = $"{formInfo.Title}.zip";
                var directoryPath = Context.UtilsApi.GetTemporaryFilesPath("form");
                FormUtils.DeleteDirectoryIfExists(directoryPath);

                FormBox.ExportForm(formInfo.SiteId, directoryPath, formInfo.Id);

                Context.UtilsApi.CreateZip(Context.UtilsApi.GetTemporaryFilesPath(fileName), directoryPath);

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
                var request = Context.AuthenticatedRequest;
                var siteId = request.GetQueryInt("siteId");

                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId)) return Unauthorized();

                foreach (string name in HttpContext.Current.Request.Files)
                {
                    var postFile = HttpContext.Current.Request.Files[name];

                    if (postFile == null)
                    {
                        return BadRequest("Could not read zip from body");
                    }

                    var filePath = Context.UtilsApi.GetTemporaryFilesPath("form.zip");
                    FormUtils.DeleteFileIfExists(filePath);

                    if (!FormUtils.EqualsIgnoreCase(Path.GetExtension(postFile.FileName), ".zip"))
                    {
                        return BadRequest("zip file extension is not correct");
                    }

                    postFile.SaveAs(filePath);

                    var directoryPath = Context.UtilsApi.GetTemporaryFilesPath("form");
                    FormUtils.DeleteDirectoryIfExists(directoryPath);
                    Context.UtilsApi.ExtractZip(filePath, directoryPath);

                    FormBox.ImportForm(siteId, directoryPath, true);

                    //FieldManager.Import(formInfo.SiteId, formInfo.Id, filePath);
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
