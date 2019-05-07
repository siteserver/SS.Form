using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Core.Model;
using SS.Form.Core.Utils;
using SS.SMS.Core;

namespace SS.Form.Controllers.Pages
{
    [RoutePrefix("pages/logAdd")]
    public class PagesLogAddController : ApiController
    {
        private const string Route = "";
        private const string RouteActionsUpload = "actions/upload";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var formInfo = FormManager.GetFormInfoByGet(request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.PluginId)) return Unauthorized();

                var logId = request.GetQueryInt("logId");
                var fieldInfoList = FieldManager.GetFieldInfoList(formInfo.Id);
                LogInfo logInfo = null;
                if (logId > 0)
                {
                    logInfo = LogManager.Repository.GetLogInfo(logId);
                }
                var list = new List<FieldInfo>();

                foreach (var fieldInfo in fieldInfoList)
                {
                    object value;
                    if (fieldInfo.FieldType == InputType.CheckBox.Value || fieldInfo.FieldType == InputType.SelectMultiple.Value)
                    {
                        value = logInfo != null
                            ? FormUtils.JsonDeserialize<List<string>>(logInfo.Get<string>(fieldInfo.Title))
                            : new List<string>();
                    }
                    //else if (fieldInfo.FieldType == InputType.Image.Value)
                    //{
                    //    value = logInfo != null
                    //        ? new List<string> {logInfo.Get<string>(fieldInfo.Title)}
                    //        : new List<string>();
                    //}
                    else if (fieldInfo.FieldType == InputType.Date.Value || fieldInfo.FieldType == InputType.DateTime.Value)
                    {
                        value = logInfo?.Get<DateTime>(fieldInfo.Title);
                    }
                    else
                    {
                        value = logInfo?.Get<string>(fieldInfo.Title);
                    }

                    if (value == null)
                    {
                        value = string.Empty;
                    }

                    list.Add(new FieldInfo
                    {
                        Id = fieldInfo.Id,
                        Title = fieldInfo.Title,
                        Description = fieldInfo.Description,
                        PlaceHolder = fieldInfo.PlaceHolder,
                        FieldType = fieldInfo.FieldType,
                        Validate = fieldInfo.Validate,
                        Columns = fieldInfo.Columns,
                        Height = fieldInfo.Height,
                        Items = fieldInfo.Items,
                        Value = value
                    });
                }

                var adminToken = Context.AdminApi.GetAccessToken(request.AdminId, request.AdminName, TimeSpan.FromDays(1));

                return Ok(new
                {
                    Value = list,
                    AdminToken = adminToken
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsUpload)]
        public IHttpActionResult UploadFile()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var siteId = request.GetQueryInt("siteId");
                var fieldId = request.GetQueryInt("fieldId");

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId))
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

        [HttpDelete, Route(RouteActionsUpload)]
        public IHttpActionResult DeleteFile()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var siteId = request.GetQueryInt("siteId");
                var fieldId = request.GetQueryInt("fieldId");

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, FormUtils.PluginId))
                {
                    return Unauthorized();
                }

                var fileUrl = request.GetPostString("value");

                var filePath = FormUtils.PathCombine(Context.Environment.PhysicalApplicationPath, fileUrl);
                FormUtils.DeleteFileIfExists(filePath);

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

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var formInfo = FormManager.GetFormInfoByPost(request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.PluginId)) return Unauthorized();

                var logId = request.GetPostInt("logId");

                var logInfo = logId > 0
                    ? LogManager.Repository.GetLogInfo(logId)
                    : new LogInfo
                    {
                        FormId = formInfo.Id,
                        AddDate = DateTime.Now
                    };
                var fieldInfoList = FieldManager.GetFieldInfoList(formInfo.Id);
                foreach (var fieldInfo in fieldInfoList)
                {
                    if (request.IsPostExists(fieldInfo.Title))
                    {
                        var value = request.GetPostString(fieldInfo.Title);
                        if (fieldInfo.FieldType == InputType.Date.Value || fieldInfo.FieldType == InputType.DateTime.Value)
                        {
                            var dt = FormUtils.ToDateTime(request.GetPostString(fieldInfo.Title));
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
