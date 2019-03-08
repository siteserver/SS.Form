using SiteServer.Plugin;
using SS.Form.Core.Utils;

namespace SS.Form.Core
{
    public static class MailTemplateManager
    {
        private static string GetTemplatesDirectoryPath()
        {
            return Context.PluginApi.GetPluginPath(FormUtils.PluginId, "assets/mail");
        }

        public static string GetTemplateHtml()
        {
            var directoryPath = GetTemplatesDirectoryPath();
            var htmlPath = FormUtils.PathCombine(directoryPath, "template.html");
            var html = CacheUtils.Get<string>(htmlPath);
            if (html != null) return html;

            html = FormUtils.ReadText(htmlPath);

            CacheUtils.InsertHours(htmlPath, html, 24);
            return html;
        }

        public static string GetListHtml()
        {
            var directoryPath = GetTemplatesDirectoryPath();
            var htmlPath = FormUtils.PathCombine(directoryPath, "list.html");
            var html = CacheUtils.Get<string>(htmlPath);
            if (html != null) return html;

            html = FormUtils.ReadText(htmlPath);

            CacheUtils.InsertHours(htmlPath, html, 24);
            return html;
        }
    }
}
