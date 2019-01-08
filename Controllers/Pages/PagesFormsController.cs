using System;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Core.Model;
using SS.Form.Core.Provider;
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
                var request = Context.GetCurrentRequest();
                var siteId = request.GetQueryInt("siteId");
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
                var request = Context.GetCurrentRequest();
                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId))
                {
                    return Unauthorized();
                }

                var formId = request.GetQueryInt("formId");

                FormDao.Delete(siteId, formId);

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
                var request = Context.GetCurrentRequest();
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

                FormDao.Insert(formInfo);

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
                var request = Context.GetCurrentRequest();
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

                FormDao.Update(formInfo);

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
                var request = Context.GetCurrentRequest();
                var siteId = request.GetPostInt("siteId");
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId))
                {
                    return Unauthorized();
                }

                var formId = request.GetPostInt("formId");

                FormDao.UpdateTaxisToUp(siteId, formId);

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
                var request = Context.GetCurrentRequest();
                var siteId = request.GetPostInt("siteId");
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId))
                {
                    return Unauthorized();
                }

                var formId = request.GetPostInt("formId");

                FormDao.UpdateTaxisToDown(siteId, formId);

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
