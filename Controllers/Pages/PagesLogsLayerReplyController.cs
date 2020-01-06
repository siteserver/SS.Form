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
                var request = Context.AuthenticatedRequest;

                var formInfo = FormManager.GetFormInfoByGet(request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.MenuFormsPermission)) return Unauthorized();

                var logId = request.GetQueryInt("logId");
                var fieldInfoList = FieldManager.GetFieldInfoList(formInfo.Id);
                var logInfo = LogManager.Repository.GetLogInfo(logId);

                var attributeNames = FormManager.GetAllAttributeNames(fieldInfoList);
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
                var request = Context.AuthenticatedRequest;

                var formInfo = FormManager.GetFormInfoByPost(request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.MenuFormsPermission)) return Unauthorized();

                var logId = request.GetPostInt("logId");
                var logInfo = LogManager.Repository.GetLogInfo(logId);
                if (logInfo == null) return NotFound();

                logInfo.ReplyContent = request.GetPostString("replyContent");

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
