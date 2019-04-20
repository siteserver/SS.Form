using System.Web;
using SiteServer.Plugin;
using SS.Form.Core.Utils;

namespace SS.Form.Core
{
    public static class StlForm
    {
        public const string ElementName = "stl:form";

        private const string AttributeTitle = "title";
        private const string AttributeName = "name";
        private const string AttributeType = "type";

        public static string Parse(IParseContext context)
        {
            var formName = string.Empty;
            var type = string.Empty;

            foreach (var name in context.StlAttributes.AllKeys)
            {
                var value = context.StlAttributes[name];

                if (FormUtils.EqualsIgnoreCase(name, AttributeTitle) || FormUtils.EqualsIgnoreCase(name, AttributeName))
                {
                    formName = Context.ParseApi.ParseAttributeValue(value, context);
                }
                else if (FormUtils.EqualsIgnoreCase(name, AttributeType))
                {
                    type = Context.ParseApi.ParseAttributeValue(value, context);
                }
            }

            var formInfo = !string.IsNullOrEmpty(formName) ? FormManager.GetFormInfoByTitle(context.SiteId, formName) : FormManager.GetFormInfoByContentId(context.SiteId, context.ChannelId, context.ContentId);

            if (formInfo == null)
            {
                var formInfoList = FormManager.GetFormInfoList(context.SiteId, 0);
                if (formInfoList != null && formInfoList.Count > 0)
                {
                    formInfo = formInfoList[0];
                }
            }

            if (formInfo == null) return string.Empty;

            if (string.IsNullOrEmpty(context.StlInnerHtml))
            {
                var elementId = $"iframe_{FormUtils.GetShortGuid(false)}";
                var libUrl = Context.PluginApi.GetPluginUrl(FormUtils.PluginId, "assets/lib/iframe-resizer-3.6.3/iframeResizer.min.js");
                var pageUrl = Context.PluginApi.GetPluginUrl(FormUtils.PluginId, $"templates/{type}/index.html?siteId={context.SiteId}&formId={formInfo.Id}&apiUrl={HttpUtility.UrlEncode(Context.Environment.ApiUrl)}");

                return $@"
<iframe id=""{elementId}"" frameborder=""0"" scrolling=""no"" src=""{pageUrl}"" style=""width: 1px;min-width: 100%;""></iframe>
<script type=""text/javascript"" src=""{libUrl}""></script>
<script type=""text/javascript"">iFrameResize({{log: false}}, '#{elementId}')</script>
";
            }

            return $@"
<script>
var $config = {{
    apiUrl: '{Context.Environment.ApiUrl}',
    siteId: {context.SiteId},
    formId: {formInfo.Id}
}};
</script>
{context.StlInnerHtml}
";
        }
    }
}
