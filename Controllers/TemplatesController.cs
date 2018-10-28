using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Core.Provider;
using SS.Form.Core.Utils;

namespace SS.Form.Controllers
{
    [RoutePrefix("templates")]
    public class TemplatesController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = Context.GetCurrentRequest();
                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId)) return Unauthorized();

                var templateType = request.GetQueryString("templateType");
                var directoryPath = Context.PluginApi.GetPluginPath(FormUtils.PluginId, "templates");
                var templateUrl = Context.PluginApi.GetPluginUrl(FormUtils.PluginId, "templates");

                var formInfo = FormManager.GetFormInfoByContentId(siteId, 0, 0) ?? FormDao.CreateDefaultForm(siteId, 0, 0);

                var templates = new List<object>();
                foreach (var directoryName in FormUtils.GetDirectoryNames(directoryPath))
                {
                    if (FormUtils.StartsWithIgnoreCase(directoryName, templateType))
                    {
                        var html = FormManager.GetTemplateHtml(templateType, directoryName);
                        templates.Add(new
                        {
                            Id = directoryName,
                            FormId = formInfo.Id,
                            TemplateUrl = templateUrl,
                            Html = html
                        });
                    }
                }
                
                return Ok(new
                {
                    Value = templates
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
