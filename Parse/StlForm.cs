using System.Collections.Generic;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Model;
using SS.Form.Provider;

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

            foreach (var name in context.StlAttributes.AllKeys)
            {
                var value = context.StlAttributes[name];

                if (Utils.EqualsIgnoreCase(name, AttributeTitle))
                {
                    title = Context.ParseApi.ParseAttributeValue(value, context);
                }
                else if (Utils.EqualsIgnoreCase(name, AttributeTheme))
                {
                    theme = value;
                }
            }

            var formId = !string.IsNullOrEmpty(title) ? FormDao.GetFormIdByTitle(context.SiteId, title) : FormDao.GetFormIdByContentId(context.SiteId, context.ChannelId, context.ContentId);

            var formInfo = FormDao.GetFormInfo(formId);
            var formSettings = new FormSettings(formInfo?.Settings);

            if (string.IsNullOrEmpty(theme))
            {
                theme = formSettings.DefaultTheme;
            }
            theme = ParseUtils.GetTheme(theme);

            if (formInfo == null) return string.Empty;

            var templateHtml = context.StlInnerHtml;
            if (string.IsNullOrWhiteSpace(templateHtml))
            {
                templateHtml = ParseUtils.GetTemplateHtml(theme);
            }
            else
            {
                var stlElements = Context.ParseApi.GetStlElements(templateHtml, new List<string>
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
