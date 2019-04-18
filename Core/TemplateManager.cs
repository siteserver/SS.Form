using System.Collections.Generic;
using SiteServer.Plugin;
using SS.Form.Core.Model;
using SS.Form.Core.Utils;

namespace SS.Form.Core
{
    public static class TemplateManager
    {
        public static string GetTemplatesDirectoryPath()
        {
            return Context.PluginApi.GetPluginPath(FormUtils.PluginId, "templates");
        }

        public static List<TemplateInfo> GetTemplateInfoList()
        {
            var templateInfoList = new List<TemplateInfo>();

            var directoryPath = GetTemplatesDirectoryPath();
            var directoryNames = FormUtils.GetDirectoryNames(directoryPath);
            foreach (var directoryName in directoryNames)
            {
                var templateInfo = GetTemplateInfo(directoryPath, directoryName);
                if (templateInfo != null)
                {
                    templateInfoList.Add(templateInfo);
                }
            }

            return templateInfoList;
        }

        public static TemplateInfo GetTemplateInfo(string name)
        {
            var directoryPath = GetTemplatesDirectoryPath();
            return GetTemplateInfo(directoryPath, name);
        }

        private static TemplateInfo GetTemplateInfo(string templatesDirectoryPath, string name)
        {
            TemplateInfo templateInfo = null;

            var configPath = FormUtils.PathCombine(templatesDirectoryPath, name, "config.json");
            if (FormUtils.IsFileExists(configPath))
            {
                templateInfo = FormUtils.JsonDeserialize<TemplateInfo>(FormUtils.ReadText(configPath));
                templateInfo.Name = name;
            }

            return templateInfo;
        }

        public static void Clone(string nameToClone, TemplateInfo templateInfo, List<TemplateInfo> templateInfoList)
        {
            var directoryPath = Context.PluginApi.GetPluginPath(FormUtils.PluginId, "templates");

            FormUtils.CopyDirectory(FormUtils.PathCombine(directoryPath, nameToClone), FormUtils.PathCombine(directoryPath, templateInfo.Name), true);

            var configJson = FormUtils.JsonSerialize(templateInfo);
            var configPath = FormUtils.PathCombine(directoryPath, templateInfo.Name, "config.json");
            FormUtils.WriteText(configPath, configJson);
        }

        public static string GetTemplateHtml(TemplateInfo templateInfo)
        {
            var directoryPath = GetTemplatesDirectoryPath();
            var htmlPath = FormUtils.PathCombine(directoryPath, templateInfo.Name, templateInfo.Main);
            var html = CacheUtils.Get<string>(htmlPath);
            if (html != null) return html;

            html = FormUtils.ReadText(htmlPath);

            CacheUtils.InsertHours(htmlPath, html, 1);
            return html;
        }

        public static void SetTemplateHtml(TemplateInfo templateInfo, string html)
        {
            var directoryPath = GetTemplatesDirectoryPath();
            var htmlPath = FormUtils.PathCombine(directoryPath, templateInfo.Name, templateInfo.Main);

            FormUtils.WriteText(htmlPath, html);
            ClearCache(htmlPath);
        }

        public static void DeleteTemplate(string name)
        {
            if (string.IsNullOrEmpty(name)) return;

            var directoryPath = GetTemplatesDirectoryPath();
            var templatePath = FormUtils.PathCombine(directoryPath, name);
            FormUtils.DeleteDirectoryIfExists(templatePath);
        }

        public static void ClearCache(string htmlPath)
        {
            CacheUtils.Remove(htmlPath);
        }
    }
}
