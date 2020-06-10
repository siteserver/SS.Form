﻿using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Core.Utils;

namespace SS.Form.Controllers.Pages
{
    [RoutePrefix("pages/logs")]
    public class PagesLogsController : ApiController
    {
        private const string Route = "";
        private const string ActionsExport = "actions/export";
        private const string ActionsVisible = "actions/visible";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var page = request.GetQueryInt("page", 1);

                var formInfo = FormManager.GetFormInfoByGet(request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.MenuFormsPermission)) return Unauthorized();

                var fieldInfoList = FieldManager.GetFieldInfoList(formInfo.Id);
                var listAttributeNames = FormUtils.StringCollectionToStringList(formInfo.ListAttributeNames);
                var allAttributeNames = FormManager.GetAllAttributeNames(fieldInfoList);
                var pageSize = FormUtils.GetPageSize(formInfo);

                var (total, logInfoList) = LogManager.Repository.GetLogs(formInfo, false, null, page);
                var pages = Convert.ToInt32(Math.Ceiling((double)total / pageSize));
                if (pages == 0) pages = 1;
                if (page > pages) page = pages;

                var logs = new List<Dictionary<string, object>>();
                foreach (var logInfo in logInfoList)
                {
                    logs.Add(LogManager.GetDict(fieldInfoList, logInfo));
                }

                return Ok(new
                {
                    Value = logs,
                    Count = total,
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
                var request = Context.AuthenticatedRequest;

                var formInfo = FormManager.GetFormInfoByGet(request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.MenuFormsPermission)) return Unauthorized();

                var page = request.GetQueryInt("page", 1);
                var logId = request.GetQueryInt("logId");
                var logInfo = LogManager.Repository.GetLogInfo(logId);
                if (logInfo == null) return NotFound();

                LogManager.Repository.Delete(formInfo, logInfo);

                var pageSize = FormUtils.GetPageSize(formInfo);

                var (total, logInfoList) = LogManager.Repository.GetLogs(formInfo, false, null, page);
                var pages = Convert.ToInt32(Math.Ceiling((double)total / pageSize));
                if (pages == 0) pages = 1;
                if (page > pages) page = pages;

                var logs = new List<IDictionary<string, object>>();
                foreach (var info in logInfoList)
                {
                    logs.Add(info.ToDictionary());
                }

                return Ok(new
                {
                    Value = logs,
                    Count = total,
                    Pages = pages,
                    Page = page
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(ActionsExport)]
        public IHttpActionResult Export()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var formInfo = FormManager.GetFormInfoByPost(request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.MenuFormsPermission)) return Unauthorized();

                var fieldInfoList = FieldManager.GetFieldInfoList(formInfo.Id);
                var logs = LogManager.Repository.GetAllLogInfoList(formInfo);

                var head = new List<string> { "编号" };
                foreach (var fieldInfo in fieldInfoList)
                {
                    head.Add(fieldInfo.Title);
                }
                head.Add("添加时间");

                var rows = new List<List<string>>();
                
                foreach (var log in logs)
                {
                    var row = new List<string>
                    {
                        log.Guid
                    };
                    foreach (var fieldInfo in fieldInfoList)
                    {
                        row.Add(LogManager.GetValue(fieldInfo, log));
                    }

                    if (log.AddDate.HasValue)
                    {
                        row.Add(log.AddDate.Value.ToString("yyyy-MM-dd HH:mm"));
                    }

                    rows.Add(row);
                }

                var fileName = $"{formInfo.Title}.csv";
                CsvUtils.Export(Context.PluginApi.GetPluginPath(FormUtils.PluginId, fileName), head, rows);
                var downloadUrl = Context.PluginApi.GetPluginUrl(FormUtils.PluginId, fileName);

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

        [HttpPost, Route(ActionsVisible)]
        public IHttpActionResult Visible()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var formInfo = FormManager.GetFormInfoByPost(request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.MenuFormsPermission)) return Unauthorized();

                var attributeName = request.GetPostString("attributeName");

                var attributeNames = FormUtils.StringCollectionToStringList(formInfo.ListAttributeNames);
                if (attributeNames.Contains(attributeName))
                {
                    attributeNames.Remove(attributeName);
                }
                else
                {
                    attributeNames.Add(attributeName);
                }

                formInfo.ListAttributeNames = FormUtils.ObjectCollectionToString(attributeNames);
                FormManager.Repository.Update(formInfo);

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
