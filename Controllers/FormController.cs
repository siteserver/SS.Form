using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Core.Model;
using SS.Form.Core.Utils;

namespace SS.Form.Controllers
{
    public class FormController : ApiController
    {
        private static string GetUploadTokenCacheKey(int formId)
        {
            return $"SS.Form.Controllers.Actions.Upload.{formId}";
        }

        [HttpPost, Route("{siteId:int}/{formId:int}/actions/get")]
        public IHttpActionResult GetForm(int siteId, int formId)
        {
            try
            {
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

                var fieldInfoList = FieldManager.GetFieldInfoList(formInfo.Id);

                var uploadToken = FormUtils.GetShortGuid(false);

                var cacheKey = GetUploadTokenCacheKey(formId);
                var cacheList = CacheUtils.Get<List<string>>(cacheKey) ?? new List<string>();
                cacheList.Add(uploadToken);
                CacheUtils.Insert(cacheKey, cacheList, 12);

                return Ok(new
                {
                    Value = fieldInfoList,
                    formInfo.Title,
                    formInfo.Description,
                    formInfo.IsCaptcha,
                    UploadToken = uploadToken
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("{siteId:int}/{formId:int}/actions/upload")]
        public IHttpActionResult UploadFile(int siteId, int formId)
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var fieldId = request.GetQueryInt("fieldId");
                var uploadToken = request.GetQueryString("uploadToken");

                var cacheKey = GetUploadTokenCacheKey(formId);
                var cacheList = CacheUtils.Get<List<string>>(cacheKey) ?? new List<string>();
                if (!cacheList.Contains(uploadToken))
                {
                    return Unauthorized();
                }

                var fieldInfo = FieldManager.GetFieldInfo(formId, fieldId);
                if (fieldInfo.FieldType != InputType.Image.Value)
                {
                    return Unauthorized();
                }

                var imageUrl = string.Empty;

                foreach (string name in HttpContext.Current.Request.Files)
                {
                    var postFile = HttpContext.Current.Request.Files[name];

                    if (postFile == null)
                    {
                        return BadRequest("Could not read image from body");
                    }

                    var filePath = Context.SiteApi.GetUploadFilePath(siteId, postFile.FileName);

                    if (!FormUtils.IsImage(Path.GetExtension(filePath)))
                    {
                        return BadRequest("image file extension is not correct");
                    }

                    postFile.SaveAs(filePath);

                    imageUrl = Context.SiteApi.GetSiteUrlByFilePath(filePath);
                }

                return Ok(new
                {
                    Value = imageUrl,
                    FieldId = fieldId
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route("{siteId:int}/{formId:int}/actions/upload")]
        public IHttpActionResult DeleteFile(int siteId, int formId)
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var fieldId = request.GetQueryInt("fieldId");
                var uploadToken = request.GetQueryString("uploadToken");

                var cacheKey = GetUploadTokenCacheKey(formId);
                var cacheList = CacheUtils.Get<List<string>>(cacheKey) ?? new List<string>();
                if (!cacheList.Contains(uploadToken))
                {
                    return Unauthorized();
                }

                var fieldInfo = FieldManager.GetFieldInfo(formId, fieldId);
                if (fieldInfo.FieldType != InputType.Image.Value)
                {
                    return Unauthorized();
                }

                return Ok(new
                {
                    Value = string.Empty,
                    FieldId = fieldId
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

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

        [HttpGet, Route("{siteId:int}/{formId:int}")]
        public IHttpActionResult List(int siteId, int formId)
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var formInfo = FormManager.GetFormInfo(siteId, formId);
                if (formInfo == null) return NotFound();

                var fieldInfoList = FieldManager.GetFieldInfoList(formInfo.Id);
                var listAttributeNames = FormUtils.StringCollectionToStringList(formInfo.ListAttributeNames);
                var allAttributeNames = FormManager.GetAllAttributeNames(fieldInfoList);

                var pages = Convert.ToInt32(Math.Ceiling((double)formInfo.TotalCount / FormUtils.PageSize));
                if (pages == 0) pages = 1;
                var page = request.GetQueryInt("page", 1);
                if (page > pages) page = pages;
                var logInfoList = LogManager.Repository.GetLogInfoList(formInfo, formInfo.IsReply, page);

                var logs = new List<IDictionary<string, object>>();
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
    }
}
