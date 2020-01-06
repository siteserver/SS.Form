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

        [HttpGet, Route(Route)]
        public IHttpActionResult List()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.MenuTemplatesPermission)) return Unauthorized();

                var type = request.GetQueryString("type");

                return Ok(new
                {
                    Value = TemplateManager.GetTemplateInfoList(type)
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
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.MenuTemplatesPermission)) return Unauthorized();

                var type = request.GetQueryString("type");
                var name = request.GetQueryString("name");
                TemplateManager.DeleteTemplate(name);

                return Ok(new
                {
                    Value = TemplateManager.GetTemplateInfoList(type)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //private const string Route = "";
        //private const string RouteHtml = "html";

        //[HttpGet, Route(Route)]
        //public IHttpActionResult List()
        //{
        //    try
        //    {
        //        var request = Context.AuthenticatedRequest;

        //        var siteId = request.GetQueryInt("siteId");
        //        if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId)) return Unauthorized();

        //        return Ok(new
        //        {
        //            Value = TemplateManager.GetTemplateInfoList()
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }
        //}

        //[HttpGet, Route(RouteHtml)]
        //public IHttpActionResult GetHtml()
        //{
        //    try
        //    {
        //        var request = Context.AuthenticatedRequest;

        //        var siteId = request.GetQueryInt("siteId");
        //        if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId)) return Unauthorized();

        //        var name = request.GetQueryString("name");
        //        var templateInfo = TemplateManager.GetTemplateInfo(name);
        //        var html = TemplateManager.GetTemplateHtml(templateInfo);

        //        return Ok(new
        //        {
        //            Value = html
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }
        //}

        //[HttpPost, Route(Route)]
        //public IHttpActionResult Submit()
        //{
        //    try
        //    {
        //        var request = Context.AuthenticatedRequest;

        //        var siteId = request.GetQueryInt("siteId");
        //        if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId)) return Unauthorized();

        //        var name = request.GetPostString("name");
        //        var templateHtml = request.GetPostString("templateHtml");
        //        var templateInfo = TemplateManager.GetTemplateInfo(name);

        //        TemplateManager.SetTemplateHtml(templateInfo, templateHtml);

        //        return Ok(new
        //        {
        //            Value = true
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }
        //}

        //[HttpDelete, Route(Route)]
        //public IHttpActionResult Delete()
        //{
        //    try
        //    {
        //        var request = Context.AuthenticatedRequest;

        //        var siteId = request.GetQueryInt("siteId");
        //        if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId)) return Unauthorized();

        //        var name = request.GetQueryString("name");
        //        TemplateManager.DeleteTemplate(name);

        //        return Ok(new
        //        {
        //            Value = TemplateManager.GetTemplateInfoList()
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }
        //}
    }
}
