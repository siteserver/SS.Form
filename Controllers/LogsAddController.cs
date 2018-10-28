using System;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Core.Model;
using SS.Form.Core.Provider;
using SS.Form.Core.Utils;

namespace SS.Form.Controllers
{
    [RoutePrefix("logs/add")]
    public class LogsAddController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = Context.GetCurrentRequest();
                var formInfo = FormManager.GetFormInfoByGet(request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.PluginId)) return Unauthorized();

                var logId = request.GetQueryInt("logId");
                var fieldInfoList = FieldManager.GetFieldInfoList(formInfo.Id);

                if (logId > 0)
                {
                    var logInfo = LogDao.GetLogInfo(logId);
                    foreach (var fieldInfo in fieldInfoList)
                    {
                        fieldInfo.Value = logInfo.GetString(fieldInfo.Title);
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
                var request = Context.GetCurrentRequest();
                var formInfo = FormManager.GetFormInfoByPost(request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.PluginId)) return Unauthorized();

                var logId = request.GetPostInt("logId");

                var logInfo = logId > 0
                    ? LogDao.GetLogInfo(logId)
                    : new LogInfo
                    {
                        FormId = formInfo.Id,
                        AddDate = DateTime.Now
                    };
                var fieldInfoList = FieldManager.GetFieldInfoList(formInfo.Id);
                foreach (var fieldInfo in fieldInfoList)
                {
                    logInfo.Set(fieldInfo.Title, request.GetPostString(fieldInfo.Title));
                }

                if (logId == 0)
                {
                    LogDao.Insert(formInfo, logInfo);
                }
                else
                {
                    LogDao.Update(logInfo);
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
