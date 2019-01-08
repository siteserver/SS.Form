using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Core.Provider;
using SS.Form.Core.Utils;

namespace SS.Form.Controllers.Pages
{
    [RoutePrefix("pages/logs")]
    public class PagesLogsController : ApiController
    {
        private const string Route = "";
        private const string RouteActionsExport = "actions/export";
        private const string RouteActionsVisible = "actions/visible";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = Context.GetCurrentRequest();
                var formInfo = FormManager.GetFormInfoByGet(request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.PluginId)) return Unauthorized();

                var fieldInfoList = FieldManager.GetFieldInfoList(formInfo.Id);
                var listAttributeNames = FormUtils.StringCollectionToStringList(formInfo.Additional.ListAttributeNames);
                var allAttributeNames = FormManager.GetAllAttributeNames(formInfo, fieldInfoList);

                var pages = Convert.ToInt32(Math.Ceiling((double)formInfo.TotalCount / FormUtils.PageSize));
                if (pages == 0) pages = 1;
                var page = request.GetQueryInt("page", 1);
                if (page > pages) page = pages;
                var logInfoList = LogDao.GetLogInfoList(formInfo, false, page);

                var logs = new List<Dictionary<string, object>>();
                foreach (var logInfo in logInfoList)
                {
                    logs.Add(logInfo.ToDictionary());
                }

                return Ok(new
                {
                    Value = logs,
                    Count = formInfo.TotalCount,
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

        [HttpDelete, Route(Route)]
        public IHttpActionResult Delete()
        {
            try
            {
                var request = Context.GetCurrentRequest();
                var formInfo = FormManager.GetFormInfoByGet(request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.PluginId)) return Unauthorized();

                var logId = request.GetQueryInt("logId");
                var logInfo = LogDao.GetLogInfo(logId);
                if (logInfo == null) return NotFound();

                LogDao.Delete(formInfo, logInfo);

                var pages = Convert.ToInt32(Math.Ceiling((double)formInfo.TotalCount / FormUtils.PageSize));
                if (pages == 0) pages = 1;
                var page = request.GetQueryInt("page", 1);
                if (page > pages) page = pages;
                var logInfoList = LogDao.GetLogInfoList(formInfo, false, page);

                var logs = new List<Dictionary<string, object>>();
                foreach (var info in logInfoList)
                {
                    logs.Add(info.ToDictionary());
                }

                return Ok(new
                {
                    Value = logs,
                    Count = formInfo.TotalCount,
                    Pages = pages,
                    Page = page
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsExport)]
        public IHttpActionResult Export()
        {
            try
            {
                var request = Context.GetCurrentRequest();
                var formInfo = FormManager.GetFormInfoByPost(request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.PluginId)) return Unauthorized();

                var fieldInfoList = FieldManager.GetFieldInfoList(formInfo.Id);
                var logs = LogDao.GetLogInfoList(formInfo.Id, false, 0, formInfo.TotalCount);

                var head = new List<string> { "序号" };
                foreach (var fieldInfo in fieldInfoList)
                {
                    head.Add(fieldInfo.Title);
                }
                head.Add("添加时间");

                var rows = new List<List<string>>();

                var index = 1;
                foreach (var log in logs)
                {
                    var row = new List<string>
                    {
                        index++.ToString()
                    };
                    foreach (var fieldInfo in fieldInfoList)
                    {
                        row.Add(log.GetString(fieldInfo.Title));
                    }
                    row.Add(log.AddDate.ToString("yyyy-MM-dd HH:mm"));

                    rows.Add(row);
                }

                var relatedPath = "表单数据.csv";

                CsvUtils.Export(Context.PluginApi.GetPluginPath(FormUtils.PluginId, relatedPath), head, rows);
                var downloadUrl = Context.PluginApi.GetPluginUrl(FormUtils.PluginId, relatedPath);

                return Ok(new
                {
                    Value = downloadUrl
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsVisible)]
        public IHttpActionResult Visible()
        {
            try
            {
                var request = Context.GetCurrentRequest();
                var formInfo = FormManager.GetFormInfoByPost(request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.PluginId)) return Unauthorized();

                var attributeName = request.GetPostString("attributeName");

                var attributeNames = FormUtils.StringCollectionToStringList(formInfo.Additional.ListAttributeNames);
                if (attributeNames.Contains(attributeName))
                {
                    attributeNames.Remove(attributeName);
                }
                else
                {
                    attributeNames.Add(attributeName);
                }

                formInfo.Additional.ListAttributeNames = FormUtils.ObjectCollectionToString(attributeNames);
                FormDao.Update(formInfo);

                return Ok(new
                {
                    Value = attributeNames
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
