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
    }
}
