using SiteServer.Plugin;
using SS.Form.Core.Utils;

namespace SS.Form.Core.Parse
{
    public static class StlForm
    {
        public const string ElementName = "stl:form";

        private const string AttributeTitle = "title";

        public static string Parse(IParseContext context)
        {
            var title = string.Empty;

            foreach (var name in context.StlAttributes.AllKeys)
            {
                var value = context.StlAttributes[name];

                if (FormUtils.EqualsIgnoreCase(name, AttributeTitle))
                {
                    title = Context.ParseApi.ParseAttributeValue(value, context);
                }
            }

            var formInfo = !string.IsNullOrEmpty(title) ? FormManager.GetFormInfoByTitle(context.SiteId, title) : FormManager.GetFormInfoByContentId(context.SiteId, context.ChannelId, context.ContentId);

            if (formInfo == null)
            {
                var formInfoList = FormManager.GetFormInfoList(context.SiteId, 0);
                if (formInfoList != null && formInfoList.Count > 0)
                {
                    formInfo = formInfoList[0];
                }
            }

            if (formInfo == null) return string.Empty;

            return $@"
<script>
var $config = {{
    apiUrl: '{Context.UtilsApi.GetApiUrl()}',
    siteId: {context.SiteId},
    formId: {formInfo.Id}
}};
</script>
{context.StlInnerHtml}
";
        }
    }
}
