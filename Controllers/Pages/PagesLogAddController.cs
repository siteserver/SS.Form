using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Core.Model;
using SS.Form.Core.Utils;

namespace SS.Form.Controllers.Pages
{
    [RoutePrefix("pages/logAdd")]
    public class PagesLogAddController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = Request.GetAuthenticatedRequest();

                var formInfo = FormManager.GetFormInfoByGet(Request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.PluginId)) return Unauthorized();

                var logId = Request.GetQueryInt("logId");
                var fieldInfoList = FieldManager.GetFieldInfoList(formInfo.Id);

                if (logId > 0)
                {
                    var logInfo = LogManager.Repository.Get(logId);
                    foreach (var fieldInfo in fieldInfoList)
                    {
                        if (fieldInfo.FieldType == InputType.CheckBox.Value || fieldInfo.FieldType == InputType.SelectMultiple.Value)
                        {
                            fieldInfo.Value = FormUtils.JsonDeserialize<List<string>>(logInfo.Get<string>(fieldInfo.Title));
                        }
                        else if (fieldInfo.FieldType == InputType.Date.Value || fieldInfo.FieldType == InputType.DateTime.Value)
                        {
                            fieldInfo.Value = logInfo.Get<DateTime>(fieldInfo.Title);
                        }
                        else
                        {
                            fieldInfo.Value = logInfo.Get<string>(fieldInfo.Title);
                        }
                    }
                }

                return Ok(new
                {
                    Value = fieldInfoList
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

                var formInfo = FormManager.GetFormInfoByPost(Request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.PluginId)) return Unauthorized();

                var logId = Request.GetPostInt("logId");

                var logInfo = logId > 0
                    ? LogManager.Repository.Get(logId)
                    : new LogInfo
                    {
                        FormId = formInfo.Id,
                        AddDate = DateTime.Now
                    };
                var fieldInfoList = FieldManager.GetFieldInfoList(formInfo.Id);
                foreach (var fieldInfo in fieldInfoList)
                {
                    if (Request.IsPostExists(fieldInfo.Title))
                    {
                        var value = Request.GetPostString(fieldInfo.Title);
                        if (fieldInfo.FieldType == InputType.Date.Value || fieldInfo.FieldType == InputType.DateTime.Value)
                        {
                            var dt = FormUtils.ToDateTime(Request.GetPostString(fieldInfo.Title));
                            logInfo.Set(fieldInfo.Title, dt.ToLocalTime());
                        }

                        else
                        {
                            logInfo.Set(fieldInfo.Title, value);
                        }
                    }
                    
                }

                if (logId == 0)
                {
                    logInfo.Id = LogManager.Repository.Insert(formInfo, logInfo);
                    NotifyManager.SendNotify(formInfo, fieldInfoList, logInfo);
                }
                else
                {
                    LogManager.Repository.Update(logInfo);
                }

                return Ok(new{});
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
