using System;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Core.Model;
using SS.Form.Core.Utils;

namespace SS.Form.Controllers.Pages
{
    [RoutePrefix("pages/logsLayerReply")]
    public class PagesLogsLayerReplyController : ApiController
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
                var logInfo = LogManager.Repository.Get(logId);

                var attributeNames = FormManager.GetAllAttributeNames(formInfo, fieldInfoList);
                if (!logInfo.IsReplied)
                {
                    attributeNames.Remove(nameof(LogInfo.ReplyDate));
                }
                attributeNames.Remove(nameof(LogInfo.ReplyContent));

                return Ok(new
                {
                    Value = logInfo.ToDictionary(),
                    AttributeNames = attributeNames
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
                var logInfo = LogManager.Repository.Get(logId);
                if (logInfo == null) return NotFound();

                logInfo.ReplyContent = Request.GetPostString("replyContent");

                LogManager.Repository.Reply(formInfo, logInfo);

                return Ok(new{});
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
