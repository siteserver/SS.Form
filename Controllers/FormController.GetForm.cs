using System;
using System.Collections.Generic;
using System.Web.Http;
using SS.Form.Core;
using SS.Form.Core.Utils;

namespace SS.Form.Controllers
{
    public partial class FormController
    {
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
    }
}
