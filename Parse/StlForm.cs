using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Model;

namespace SS.Form.Parse
{
    public class StlForm
    {
        private StlForm() { }
        public const string ElementName = "stl:form";

        public const string AttributeTitle = "title";
        public const string AttributeTheme = "theme";

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

            return formInfo == null ? string.Empty : ParseImpl(context, formInfo, formSettings, theme);
        }

        public static string ParseImpl(IParseContext context, FormInfo formInfo, FormSettings formSettings, string theme)
        {
            ParseUtils.RegisterPage(context);

            var vueId = "vm_" + System.Guid.NewGuid().ToString().Replace("-", string.Empty);
            ParseUtils.RegisterFormCode(context, vueId, formInfo, formSettings);

            var templateHtml = !string.IsNullOrWhiteSpace(context.StlInnerXml) ? Main.Instance.ParseApi.ParseInnerXml(context.StlInnerXml, context) : ParseUtils.GetTemplateHtml(theme);

            return $@"
<div id=""{vueId}"" class=""normalize"">
    {templateHtml}
</div>
";
        }
    }
}
