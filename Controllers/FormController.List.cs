using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Core.Utils;

namespace SS.Form.Controllers
{
    public partial class FormController
    {
        [HttpGet, Route("{siteId:int}/{formId:int}")]
        public IHttpActionResult List(int siteId, int formId)
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var page = request.GetQueryInt("page", 1);
                var word = request.GetQueryString("word");

                var formInfo = FormManager.GetFormInfo(siteId, formId);
                if (formInfo == null) return NotFound();

                var fieldInfoList = FieldManager.GetFieldInfoList(formInfo.Id);
                var listAttributeNames = FormUtils.StringCollectionToStringList(formInfo.ListAttributeNames);
                var allAttributeNames = FormManager.GetAllAttributeNames(fieldInfoList);
                var pageSize = FormUtils.GetPageSize(formInfo);

                var (totalCount, logInfoList) = LogManager.Repository.GetLogs(formInfo, formInfo.IsReply, word, page);
                var pages = Convert.ToInt32(Math.Ceiling((double)totalCount / pageSize));
                if (pages == 0) pages = 1;
                if (page > pages) page = pages;
                

                var logs = new List<IDictionary<string, object>>();
                foreach (var logInfo in logInfoList)
                {
                    logs.Add(logInfo.ToDictionary());
                }

                return Ok(new
                {
                    Value = logs,
                    Count = totalCount,
                    Pages = pages,
                    Page = page,
                    FieldInfoList = fieldInfoList,
                    AllAttributeNames = allAttributeNames,
                    ListAttributeNames = listAttributeNames,
                    formInfo.IsReply
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
