using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Datory;
using SiteServer.Plugin;
using SS.Form.Core.Repositories;

namespace SS.Form.Controllers
{
    [RoutePrefix("ping")]
    public class PingController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public HttpResponseMessage Get()
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            response.Content = new StringContent("pong2", Encoding.UTF8);

            return response;
        }
    }
}
