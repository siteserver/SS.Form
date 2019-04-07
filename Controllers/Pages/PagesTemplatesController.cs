using System;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Core.Utils;

namespace SS.Form.Controllers.Pages
{
    [RoutePrefix("pages/templates")]
    public class PagesTemplatesController : ApiController
    {
        private const string Route = "";
        private const string RouteHtml = "html";

        [HttpGet, Route(Route)]
        public IHttpActionResult List()
        {
            try
            {
                var request = Request.GetAuthenticatedRequest();

                var siteId = Request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId)) return Unauthorized();

                return Ok(new
                {
                    Value = TemplateManager.GetTemplateInfoList()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteHtml)]
        public IHttpActionResult GetHtml()
        {
            try
            {
                var request = Request.GetAuthenticatedRequest();

                var siteId = Request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId)) return Unauthorized();

                var name = Request.GetQueryString("name");
                var templateInfo = TemplateManager.GetTemplateInfo(name);
                var html = TemplateManager.GetTemplateHtml(templateInfo);

                return Ok(new
                {
                    Value = html
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

                var siteId = Request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId)) return Unauthorized();

                var name = Request.GetPostString("name");
                var templateHtml = Request.GetPostString("templateHtml");
                var templateInfo = TemplateManager.GetTemplateInfo(name);

                TemplateManager.SetTemplateHtml(templateInfo, templateHtml);

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

        [HttpDelete, Route(Route)]
        public IHttpActionResult Delete()
        {
            try
            {
                var request = Request.GetAuthenticatedRequest();

                var siteId = Request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId)) return Unauthorized();

                var name = Request.GetQueryString("name");
                TemplateManager.DeleteTemplate(name);

                return Ok(new
                {
                    Value = TemplateManager.GetTemplateInfoList()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
