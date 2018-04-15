using System.Collections.Generic;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Model;

namespace SS.Form.Parse
{
    public static class StlForm
    {
        public const string ElementName = "stl:form";

        private const string AttributeTitle = "title";
        private const string AttributeTheme = "theme";

        public static string Parse(IParseContext context)
        {
            var title = string.Empty;
            var theme = string.Empty;

            foreach (var name in context.StlAttributes.Keys)
            {
                var value = context.StlAttributes[name];

                if (Utils.EqualsIgnoreCase(name, AttributeTitle))
                {
                    title = Main.Instance.ParseApi.ParseAttributeValue(value, context);
                }
                else if (Utils.EqualsIgnoreCase(name, AttributeTheme))
                {
                    theme = value;
                }
            }

            var formId = !string.IsNullOrEmpty(title) ? Main.Instance.FormDao.GetFormIdByTitle(context.SiteId, title) : Main.Instance.FormDao.GetFormIdByContentId(context.SiteId, context.ChannelId, context.ContentId);

            var formInfo = Main.Instance.FormDao.GetFormInfo(formId);
            var formSettings = new FormSettings(formInfo?.Settings);

            if (string.IsNullOrEmpty(theme))
            {
                theme = formSettings.DefaultTheme;
            }
            theme = ParseUtils.GetTheme(theme);

            if (formInfo == null) return string.Empty;

            var templateHtml = context.StlInnerXml;
            if (string.IsNullOrWhiteSpace(templateHtml))
            {
                templateHtml = ParseUtils.GetTemplateHtml(theme);
            }
            else
            {
                var stlElements = Main.Instance.ParseApi.GetStlElements(templateHtml, new List<string>
                {
                    "stl:template",
                    "stl:yes",
                    "stl:no"
                });
            }

            return ParseImpl(context, formInfo, formSettings, templateHtml);
        }

        private static string ParseImpl(IParseContext context, FormInfo formInfo, FormSettings formSettings, string templateHtml)
        {
            ParseUtils.RegisterPage(context);

            var vueId = "vm_" + System.Guid.NewGuid().ToString().Replace("-", string.Empty);
            ParseUtils.RegisterCode(context, vueId, formInfo, formSettings);

            return $@"
<div id=""{vueId}"">
    <template v-show=""imgUrl"" style=""display: none"">
        {templateHtml}
    </template>
</div>
";
        }
    }
}
