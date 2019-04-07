using System;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Form.Core;
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

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = Request.GetAuthenticatedRequest();

                var siteId = Request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId))
                {
                    return Unauthorized();
                }
                
                var formInfoList = FormManager.GetFormInfoList(siteId, 0);

                return Ok(new
                {
                    Value = formInfoList
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

                var siteId = Request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId))
                {
                    return Unauthorized();
                }

                var formId = Request.GetQueryInt("formId");

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
                var request = Request.GetAuthenticatedRequest();

                var siteId = Request.GetPostInt("siteId");
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId))
                {
                    return Unauthorized();
                }

                var formInfo = new FormInfo
                {
                    SiteId = siteId,
                    AddDate = DateTime.Now,
                    Title = Request.GetPostString("title"),
                    Description = Request.GetPostString("description")
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
                var request = Request.GetAuthenticatedRequest();

                var siteId = Request.GetPostInt("siteId");
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId))
                {
                    return Unauthorized();
                }
                
                var formId = Request.GetPostInt("formId");
                var formInfo = FormManager.GetFormInfo(siteId, formId);
                formInfo.Title = Request.GetPostString("title");
                formInfo.Description = Request.GetPostString("description");

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
                var request = Request.GetAuthenticatedRequest();
                var siteId = Request.GetPostInt("siteId");
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId))
                {
                    return Unauthorized();
                }

                var formId = Request.GetPostInt("formId");

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
                var request = Request.GetAuthenticatedRequest();

                var siteId = Request.GetPostInt("siteId");
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId))
                {
                    return Unauthorized();
                }

                var formId = Request.GetPostInt("formId");

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
    }
}
