using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Core.Utils;

namespace SS.Form.Controllers
{
    public partial class FormController
    {
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
    }
}
