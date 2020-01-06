using System;
using System.Linq;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Core.Model;
using SS.Form.Core.Utils;

namespace SS.Form.Controllers.Pages
{
    [RoutePrefix("pages/templateHtml")]
    public class PagesTemplateHtmlController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetTemplateInfo()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.MenuTemplatesPermission)) return Unauthorized();

                var type = request.GetQueryString("type");
                var name = request.GetQueryString("name");
                var templateInfo = TemplateManager.GetTemplateInfo(name);
                var html = TemplateManager.GetTemplateHtml(templateInfo);

                var isSystem = TemplateManager.GetTemplateInfoList(type).Any(x => !string.IsNullOrEmpty(x.Publisher) && FormUtils.EqualsIgnoreCase(name, x.Name));
                if (isSystem)
                {
                    templateInfo = new TemplateInfo();
                }

                return Ok(new
                {
                    Value = templateInfo,
                    TemplateHtml = html,
                    IsSystem = isSystem
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

                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.MenuTemplatesPermission)) return Unauthorized();

                var name = request.GetPostString("name");
                var templateHtml = request.GetPostString("templateHtml");
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
    }
}
