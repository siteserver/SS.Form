using System;
using System.Linq;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Core.Model;
using SS.Form.Core.Utils;

namespace SS.Form.Controllers.Pages
{
    [RoutePrefix("pages/templatesLayerEdit")]
    public class PagesTemplatesLayerEditController : ApiController
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

                var name = request.GetQueryString("name");
                var templateInfo = TemplateManager.GetTemplateInfo(name);

                if (!string.IsNullOrEmpty(templateInfo.Publisher))
                {
                    templateInfo = new TemplateInfo();
                }

                return Ok(new
                {
                    Value = templateInfo
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Clone()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.MenuTemplatesPermission)) return Unauthorized();

                var type = request.GetQueryString("type");
                var originalName = request.GetPostString("originalName");
                var name = request.GetPostString("name");
                var description = request.GetPostString("description");
                var templateHtml = request.GetPostString("templateHtml");

                var templateInfoList = TemplateManager.GetTemplateInfoList(type);
                var originalTemplateInfo = templateInfoList.First(x => FormUtils.EqualsIgnoreCase(originalName, x.Name));

                if (templateInfoList.Any(x => FormUtils.EqualsIgnoreCase(name, x.Name)))
                {
                    return BadRequest($"标识为 {name} 的模板已存在，请更换模板标识！");
                }

                var templateInfo = new TemplateInfo
                {
                    Name = name,
                    Main = originalTemplateInfo.Main,
                    Publisher = string.Empty,
                    Description = description,
                    Icon = originalTemplateInfo.Icon
                };
                templateInfoList.Add(templateInfo);

                TemplateManager.Clone(originalName, templateInfo, templateHtml);

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

        [HttpPut, Route(Route)]
        public IHttpActionResult Edit()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.MenuTemplatesPermission)) return Unauthorized();

                var type = request.GetQueryString("type");
                var originalName = request.GetPostString("originalName");
                var name = request.GetPostString("name");
                var description = request.GetPostString("description");

                if (FormUtils.EqualsIgnoreCase(originalName, name))
                {
                    var templateInfoList = TemplateManager.GetTemplateInfoList(type);
                    var originalTemplateInfo = templateInfoList.First(x => FormUtils.EqualsIgnoreCase(originalName, x.Name));

                    originalTemplateInfo.Name = name;
                    originalTemplateInfo.Description = description;
                    TemplateManager.Edit(originalTemplateInfo);
                }
                else
                {
                    var templateInfoList = TemplateManager.GetTemplateInfoList(type);
                    var originalTemplateInfo = templateInfoList.First(x => FormUtils.EqualsIgnoreCase(originalName, x.Name));

                    if (templateInfoList.Any(x => FormUtils.EqualsIgnoreCase(name, x.Name)))
                    {
                        return BadRequest($"标识为 {name} 的模板已存在，请更换模板标识！");
                    }

                    var templateInfo = new TemplateInfo
                    {
                        Name = name,
                        Main = originalTemplateInfo.Main,
                        Publisher = string.Empty,
                        Description = description,
                        Icon = originalTemplateInfo.Icon
                    };
                    templateInfoList.Add(templateInfo);

                    TemplateManager.Clone(originalName, templateInfo);

                    TemplateManager.DeleteTemplate(originalName);
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

        //private const string Route = "";

        //[HttpGet, Route(Route)]
        //public IHttpActionResult Get()
        //{
        //    try
        //    {
        //        var request = Context.AuthenticatedRequest;

        //        var siteId = request.GetQueryInt("siteId");
        //        if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId)) return Unauthorized();

        //        var name = request.GetQueryString("name");
        //        var templateInfoList = TemplateManager.GetTemplateInfoList();
        //        var templateInfo =
        //            templateInfoList.FirstOrDefault(x => FormUtils.EqualsIgnoreCase(name, x.Name));

        //        return Ok(new
        //        {
        //            Value = templateInfo
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }
        //}

        //[HttpPost, Route(Route)]
        //public IHttpActionResult Clone()
        //{
        //    try
        //    {
        //        var request = Context.AuthenticatedRequest;

        //        var siteId = request.GetQueryInt("siteId");
        //        if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId)) return Unauthorized();

        //        var nameToClone = request.GetPostString("nameToClone");
        //        var name = request.GetPostString("name");
        //        var description = request.GetPostString("description");

        //        var templateInfoList = TemplateManager.GetTemplateInfoList();
        //        var templateInfoToClone = templateInfoList.FirstOrDefault(x => FormUtils.EqualsIgnoreCase(nameToClone, x.Name));
        //        if (templateInfoToClone == null) return NotFound();
        //        if (templateInfoList.Any(x => FormUtils.EqualsIgnoreCase(name, x.Name)))
        //        {
        //            return BadRequest($"标识为 {name} 的模板已存在，请更换模板标识！");
        //        }

        //        var templateInfo = new TemplateInfo
        //        {
        //            Name = name,
        //            Main = templateInfoToClone.Main,
        //            Publisher = string.Empty,
        //            Description = description,
        //            Icon = templateInfoToClone.Icon
        //        };
        //        templateInfoList.Add(templateInfo);

        //        TemplateManager.Clone(nameToClone, templateInfo, templateInfoList);

        //        return Ok(new
        //        {
        //            Value = templateInfo
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }
        //}


    }
}
