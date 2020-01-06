using System;
using System.Linq;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Core.Utils;

namespace SS.Form.Controllers.Pages
{
    [RoutePrefix("pages/templatesLayerPreview")]
    public class PagesTemplatesLayerPreviewController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.MenuTemplatesPermission)) return Unauthorized();

                var formInfoList = FormManager.GetFormInfoList(siteId, 0);

                var type = request.GetQueryString("type");
                var name = request.GetQueryString("name");
                var templateInfoList = TemplateManager.GetTemplateInfoList(type);
                var templateInfo =
                    templateInfoList.FirstOrDefault(x => FormUtils.EqualsIgnoreCase(name, x.Name));

                return Ok(new
                {
                    Value = templateInfo,
                    FormInfoList = formInfoList
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
