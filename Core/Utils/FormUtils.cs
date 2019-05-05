using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using SiteServer.Plugin;

namespace SS.Form.Core.Utils
{
    public static class FormUtils
    {
        public const string PluginId = "SS.Form";

        public const int PageSize = 30;

        private const char UrlSeparatorChar = '/';
        private const char PathSeparatorChar = '\\';

        public static bool EqualsIgnoreCase(string a, string b)
        {
            if (a == b) return true;
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return false;
            return string.Equals(a.Trim().ToLower(), b.Trim().ToLower());
        }

        public static DateTime ToDateTime(string dateTimeStr)
        {
            return ToDateTime(dateTimeStr, DateTime.Now);
        }

        public static DateTime ToDateTime(string dateTimeStr, DateTime defaultValue)
        {
            var datetime = defaultValue;
            if (!string.IsNullOrEmpty(dateTimeStr))
            {
                if (!DateTime.TryParse(dateTimeStr.Trim(), out datetime))
                {
                    datetime = defaultValue;
                }
                return datetime;
            }
            if (datetime <= DateTime.MinValue)
            {
                datetime = DateTime.Now;
            }
            return datetime;
        }

        public static List<string> StringCollectionToStringList(string collection)
        {
            return StringCollectionToStringList(collection, ',');
        }

        private static List<string> StringCollectionToStringList(string collection, char split)
        {
            var list = new List<string>();
            if (!string.IsNullOrEmpty(collection))
            {
                var array = collection.Split(split);
                foreach (var s in array)
                {
                    list.Add(s);
                }
            }
            return list;
        }

        public static bool ContainsIgnoreCase(List<string> list, string target)
        {
            if (list == null || list.Count == 0) return false;

            return list.Any(element => EqualsIgnoreCase(element, target));
        }

        public static int ToIntWithNegative(string intStr, int defaultValue)
        {
            if (!int.TryParse(intStr?.Trim(), out var i))
            {
                i = defaultValue;
            }
            return i;
        }

        public static decimal ToDecimalWithNegative(string intStr, decimal defaultValue)
        {
            if (!decimal.TryParse(intStr?.Trim(), out var i))
            {
                i = defaultValue;
            }
            return i;
        }

        public static bool ToBool(string boolStr, bool defaultValue)
        {
            if (!bool.TryParse(boolStr?.Trim(), out var boolean))
            {
                boolean = defaultValue;
            }
            return boolean;
        }

        public static string ObjectCollectionToString(ICollection collection)
        {
            var builder = new StringBuilder();
            if (collection == null) return builder.ToString();

            foreach (var obj in collection)
            {
                builder.Append(obj.ToString().Trim()).Append(",");
            }
            if (builder.Length != 0) builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }

        public static string JsonSerialize(object obj)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                var timeFormat = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                settings.Converters.Add(timeFormat);

                return JsonConvert.SerializeObject(obj, settings);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static T JsonDeserialize<T>(string json)
        {
            try
            {
                var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
                var timeFormat = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                settings.Converters.Add(timeFormat);

                return JsonConvert.DeserializeObject<T>(json, settings);
            }
            catch
            {
                return default(T);
            }
        }

        private static string GetDirectoryPath(string path)
        {
            var ext = Path.GetExtension(path);
            var directoryPath = !string.IsNullOrEmpty(ext) ? Path.GetDirectoryName(path) : path;
            return directoryPath;
        }

        public static void CreateDirectoryIfNotExists(string path)
        {
            var directoryPath = GetDirectoryPath(path);

            if (IsDirectoryExists(directoryPath)) return;

            try
            {
                Directory.CreateDirectory(directoryPath);
            }
            catch
            {
                //Scripting.FileSystemObject fso = new Scripting.FileSystemObjectClass();
                //string[] directoryNames = directoryPath.Split('\\');
                //string thePath = directoryNames[0];
                //for (int i = 1; i < directoryNames.Length; i++)
                //{
                //    thePath = thePath + "\\" + directoryNames[i];
                //    if (StringUtils.Contains(thePath.ToLower(), ConfigUtils.Instance.PhysicalApplicationPath.ToLower()) && !IsDirectoryExists(thePath))
                //    {
                //        fso.CreateFolder(thePath);
                //    }
                //}                    
            }
        }

        public static void CopyDirectory(string sourcePath, string targetPath, bool isOverride)
        {
            if (!Directory.Exists(sourcePath)) return;

            CreateDirectoryIfNotExists(targetPath);
            var directoryInfo = new DirectoryInfo(sourcePath);
            foreach (var fileSystemInfo in directoryInfo.GetFileSystemInfos())
            {
                var destPath = Path.Combine(targetPath, fileSystemInfo.Name);
                if (fileSystemInfo is FileInfo)
                {
                    CopyFile(fileSystemInfo.FullName, destPath, isOverride);
                }
                else if (fileSystemInfo is DirectoryInfo)
                {
                    CopyDirectory(fileSystemInfo.FullName, destPath, isOverride);
                }
            }
        }

        private static void CopyFile(string sourceFilePath, string destFilePath, bool isOverride)
        {
            try
            {
                CreateDirectoryIfNotExists(destFilePath);

                File.Copy(sourceFilePath, destFilePath, isOverride);
            }
            catch
            {
                // ignored
            }
        }

        public static string PathCombine(params string[] paths)
        {
            var retVal = string.Empty;
            if (paths != null && paths.Length > 0)
            {
                retVal = paths[0]?.Replace(UrlSeparatorChar, PathSeparatorChar).TrimEnd(PathSeparatorChar) ?? string.Empty;
                for (var i = 1; i < paths.Length; i++)
                {
                    var path = paths[i] != null ? paths[i].Replace(UrlSeparatorChar, PathSeparatorChar).Trim(PathSeparatorChar) : string.Empty;
                    retVal = Path.Combine(retVal, path);
                }
            }
            return retVal;
        }

        public static IEnumerable<string> GetDirectoryNames(string directoryPath)
        {
            var directories = Directory.GetDirectories(directoryPath);
            var retVal = new string[directories.Length];
            var i = 0;
            foreach (var directory in directories)
            {
                var directoryInfo = new DirectoryInfo(directory);
                retVal[i++] = directoryInfo.Name;
            }
            return retVal;
        }

        private static bool IsDirectoryExists(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        public static void DeleteDirectoryIfExists(string directoryPath)
        {
            try
            {
                if (IsDirectoryExists(directoryPath))
                {
                    Directory.Delete(directoryPath, true);
                }
            }
            catch
            {
                // ignored
            }
        }

        public static bool DeleteFileIfExists(string filePath)
        {
            var retVal = true;
            try
            {
                if (IsFileExists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
                //try
                //{
                //    Scripting.FileSystemObject fso = new Scripting.FileSystemObjectClass();
                //    fso.DeleteFile(filePath, true);
                //}
                //catch
                //{
                //    retval = false;
                //}
                retVal = false;
            }
            return retVal;
        }

        public static bool IsFileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public static string ReadText(string filePath)
        {
            return File.ReadAllText(filePath, Encoding.UTF8);
        }

        public static void WriteText(string filePath, string content)
        {
            File.WriteAllText(filePath, content, Encoding.UTF8);
        }

        public static string GetShortGuid(bool isUppercase)
        {
            var i = Guid.NewGuid().ToByteArray().Aggregate<byte, long>(1, (current, b) => current * (b + 1));
            var retVal = $"{i - DateTime.Now.Ticks:x}";
            return isUppercase ? retVal.ToUpper() : retVal.ToLower();
        }
    }
}
