using System.Web.Http;

namespace SS.Form.Controllers
{
    public partial class FormController : ApiController
    {
        private static string GetUploadTokenCacheKey(int formId)
        {
            return $"SS.Form.Controllers.Actions.Upload.{formId}";
        }
    }
}
