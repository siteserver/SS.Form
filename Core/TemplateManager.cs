using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using SiteServer.Plugin;
using SS.Form.Core.Model;
using SS.Form.Core.Utils;

namespace SS.Form.Core
{
    public static class TemplateManager
    {
        private static string CacheGetFileContent(string filePath)
        {
            ObjectCache cache = MemoryCache.Default;

            if (cache[filePath] is string fileContents) return fileContents;

            var policy = new CacheItemPolicy
            {
                SlidingExpiration = new TimeSpan(0, 1, 0, 0)
            };
            policy.ChangeMonitors.Add(new HostFileChangeMonitor(new List<string> { filePath }));

            fileContents = FormUtils.ReadText(filePath);

            cache.Set(filePath, fileContents, policy);

            return fileContents;
        }

        private static string GetTemplatesDirectoryPath()
        {
            return Context.PluginApi.GetPluginPath(FormUtils.PluginId, "templates");
        }

        public static List<TemplateInfo> GetTemplateInfoList(string type)
        {
            var templateInfoList = new List<TemplateInfo>();

            var directoryPath = GetTemplatesDirectoryPath();
            var directoryNames = FormUtils.GetDirectoryNames(directoryPath);
            foreach (var directoryName in directoryNames)
            {
                var templateInfo = GetTemplateInfo(directoryPath, directoryName);
                if (templateInfo == null) continue;
                if (string.IsNullOrEmpty(type) && string.IsNullOrEmpty(templateInfo.Type) || FormUtils.EqualsIgnoreCase(type, templateInfo.Type))
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
                templateInfo = Context.UtilsApi.JsonDeserialize<TemplateInfo>(FormUtils.ReadText(configPath));
                templateInfo.Name = name;
            }

            return templateInfo;
        }

        public static void Clone(string nameToClone, TemplateInfo templateInfo, string templateHtml = null)
        {
            var directoryPath = Context.PluginApi.GetPluginPath(FormUtils.PluginId, "templates");

            FormUtils.CopyDirectory(FormUtils.PathCombine(directoryPath, nameToClone), FormUtils.PathCombine(directoryPath, templateInfo.Name), true);

            var configJson = Context.UtilsApi.JsonSerialize(templateInfo);
            var configPath = FormUtils.PathCombine(directoryPath, templateInfo.Name, "config.json");
            FormUtils.WriteText(configPath, configJson);

            if (templateHtml != null)
            {
                SetTemplateHtml(templateInfo, templateHtml);
            }
        }

        public static void Edit(TemplateInfo templateInfo)
        {
            var directoryPath = Context.PluginApi.GetPluginPath(FormUtils.PluginId, "templates");

            var configJson = Context.UtilsApi.JsonSerialize(templateInfo);
            var configPath = FormUtils.PathCombine(directoryPath, templateInfo.Name, "config.json");
            FormUtils.WriteText(configPath, configJson);
        }

        public static string GetTemplateHtml(TemplateInfo templateInfo)
        {
            var directoryPath = GetTemplatesDirectoryPath();
            var htmlPath = FormUtils.PathCombine(directoryPath, templateInfo.Name, templateInfo.Main);
            return CacheGetFileContent(htmlPath);
        }

        public static void SetTemplateHtml(TemplateInfo templateInfo, string html)
        {
            var directoryPath = GetTemplatesDirectoryPath();
            var htmlPath = FormUtils.PathCombine(directoryPath, templateInfo.Name, templateInfo.Main);

            FormUtils.WriteText(htmlPath, html);
        }

        public static void DeleteTemplate(string name)
        {
            if (string.IsNullOrEmpty(name)) return;

            var directoryPath = GetTemplatesDirectoryPath();
            var templatePath = FormUtils.PathCombine(directoryPath, name);
            FormUtils.DeleteDirectoryIfExists(templatePath);
        }

        //public static string GetTemplatesDirectoryPath()
        //{
        //    return Context.PluginApi.GetPluginPath(FormUtils.PluginId, "templates");
        //}

        //public static List<TemplateInfo> GetTemplateInfoList()
        //{
        //    var templateInfoList = new List<TemplateInfo>();

        //    var directoryPath = GetTemplatesDirectoryPath();
        //    var directoryNames = FormUtils.GetDirectoryNames(directoryPath);
        //    foreach (var directoryName in directoryNames)
        //    {
        //        var templateInfo = GetTemplateInfo(directoryPath, directoryName);
        //        if (templateInfo != null)
        //        {
        //            templateInfoList.Add(templateInfo);
        //        }
        //    }

        //    return templateInfoList;
        //}

        //public static TemplateInfo GetTemplateInfo(string name)
        //{
        //    var directoryPath = GetTemplatesDirectoryPath();
        //    return GetTemplateInfo(directoryPath, name);
        //}

        //private static TemplateInfo GetTemplateInfo(string templatesDirectoryPath, string name)
        //{
        //    TemplateInfo templateInfo = null;

        //    var configPath = FormUtils.PathCombine(templatesDirectoryPath, name, "config.json");
        //    if (FormUtils.IsFileExists(configPath))
        //    {
        //        templateInfo = FormUtils.JsonDeserialize<TemplateInfo>(FormUtils.ReadText(configPath));
        //        templateInfo.Name = name;
        //    }

        //    return templateInfo;
        //}

        //public static void Clone(string nameToClone, TemplateInfo templateInfo, List<TemplateInfo> templateInfoList)
        //{
        //    var directoryPath = Context.PluginApi.GetPluginPath(FormUtils.PluginId, "templates");

        //    FormUtils.CopyDirectory(FormUtils.PathCombine(directoryPath, nameToClone), FormUtils.PathCombine(directoryPath, templateInfo.Name), true);

        //    var configJson = FormUtils.JsonSerialize(templateInfo);
        //    var configPath = FormUtils.PathCombine(directoryPath, templateInfo.Name, "config.json");
        //    FormUtils.WriteText(configPath, configJson);
        //}

        //public static string GetTemplateHtml(TemplateInfo templateInfo)
        //{
        //    var directoryPath = GetTemplatesDirectoryPath();
        //    var htmlPath = FormUtils.PathCombine(directoryPath, templateInfo.Name, templateInfo.Main);
        //    var html = CacheUtils.Get<string>(htmlPath);
        //    if (html != null) return html;

        //    html = FormUtils.ReadText(htmlPath);

        //    CacheUtils.Insert(htmlPath, html, 1);
        //    return html;
        //}

        //public static void SetTemplateHtml(TemplateInfo templateInfo, string html)
        //{
        //    var directoryPath = GetTemplatesDirectoryPath();
        //    var htmlPath = FormUtils.PathCombine(directoryPath, templateInfo.Name, templateInfo.Main);

        //    FormUtils.WriteText(htmlPath, html);
        //    ClearCache(htmlPath);
        //}

        //public static void DeleteTemplate(string name)
        //{
        //    if (string.IsNullOrEmpty(name)) return;

        //    var directoryPath = GetTemplatesDirectoryPath();
        //    var templatePath = FormUtils.PathCombine(directoryPath, name);
        //    FormUtils.DeleteDirectoryIfExists(templatePath);
        //}

        //public static void ClearCache(string htmlPath)
        //{
        //    CacheUtils.Remove(htmlPath);
        //}
    }
}
