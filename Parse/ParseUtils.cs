using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SS.Form.Core;
using SS.Form.Model;

namespace SS.Form.Parse
{
    public static class ParseUtils
    {
        public static string GetTheme(string theme)
        {
            var retval = theme;
            if (string.IsNullOrEmpty(retval) ||
                !File.Exists(Main.Instance.PluginApi.GetPluginPath($"themes/{retval}/template.html")))
            {
                retval = "bootstrap-large";
            }
            return retval;
        }

        public static List<string> GetThemeList()
        {
            var directoryPath = Main.Instance.PluginApi.GetPluginPath("themes");
            return Utils.GetDirectoryNames(directoryPath).ToList();
        }

        public static string GetTemplateHtml(string theme)
        {
            var htmlPath = Main.Instance.PluginApi.GetPluginPath($"themes/{theme}/template.html");
            var themeUrl = Main.Instance.PluginApi.GetPluginUrl($"themes/{theme}/");

            var html = Utils.ReadText(htmlPath);
            var startIndex = html.IndexOf("<!-- template start -->", StringComparison.Ordinal) + "<!-- template start -->".Length;
            var length = html.IndexOf("<!-- template end -->", StringComparison.Ordinal) - startIndex;
            html = html.Substring(startIndex, length).Trim();
            html = html.Replace(@"<link rel=""stylesheet"" type=""text/css"" href=""", $@"<link rel=""stylesheet"" type=""text/css"" href=""{themeUrl}");

            return html;
        }

        public static string GetFormStlElement(FormInfo formInfo)
        {
            var settings = new FormSettings(formInfo.Settings);

            var titleAttr = formInfo.ContentId == 0 && !string.IsNullOrEmpty(formInfo.Title)
                ? $@" title=""{formInfo.Title}"""
                : string.Empty;
            var theme = GetTheme(settings.DefaultTheme);

            return $@" <stl:form theme=""{theme}""{titleAttr}>
    {GetTemplateHtml(theme)}
</stl:form>";
        }
    }
}
