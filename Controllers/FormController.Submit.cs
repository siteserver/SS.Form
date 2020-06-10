using System;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Core.Model;

namespace SS.Form.Controllers
{
    public partial class FormController
    {
        [HttpPost, Route("{siteId:int}/{formId:int}")]
        public IHttpActionResult Submit(int siteId, int formId)
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var formInfo = FormManager.GetFormInfo(siteId, formId);
                if (formInfo == null) return NotFound();
                if (formInfo.IsClosed)
                {
                    return BadRequest("对不起，表单已被禁用");
                }

                if (formInfo.IsTimeout && (formInfo.TimeToStart > DateTime.Now || formInfo.TimeToEnd < DateTime.Now))
                {
                    return BadRequest("对不起，表单只允许在规定的时间内提交");
                }

                var logInfo = new LogInfo
                {
                    FormId = formInfo.Id,
                    AddDate = DateTime.Now
                };

                var fieldInfoList = FieldManager.GetFieldInfoList(formInfo.Id);
                foreach (var fieldInfo in fieldInfoList)
                {
                    var value = request.GetPostString(fieldInfo.Title);
                    logInfo.Set(fieldInfo.Title, value);
                    if (FieldManager.IsExtra(fieldInfo))
                    {
                        foreach (var item in fieldInfo.Items)
                        {
                            var extrasId = FieldManager.GetExtrasId(fieldInfo.Id, item.Id);
                            var extras = request.GetPostString(extrasId);
                            if (!string.IsNullOrEmpty(extras))
                            {
                                logInfo.Set(extrasId, extras);
                            }
                        }
                    }
                }

                logInfo.Id = LogManager.Repository.Insert(formInfo, logInfo);
                NotifyManager.SendNotify(formInfo, fieldInfoList, logInfo);

                return Ok(logInfo);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
