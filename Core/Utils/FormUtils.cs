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

        public static string GetFieldTypeText(string fieldType)
        {
            if (fieldType == InputType.Text.Value)
            {
                return "文本框(单行)";
            }
            if (fieldType == InputType.TextArea.Value)
            {
                return "文本框(多行)";
            }
            if (fieldType == InputType.CheckBox.Value)
            {
                return "复选框";
            }
            if (fieldType == InputType.Radio.Value)
            {
                return "单选框";
            }
            if (fieldType == InputType.SelectOne.Value)
            {
                return "下拉列表(单选)";
            }
            if (fieldType == InputType.SelectMultiple.Value)
            {
                return "下拉列表(多选)";
            }
            if (fieldType == InputType.Date.Value)
            {
                return "日期选择框";
            }
            if (fieldType == InputType.DateTime.Value)
            {
                return "日期时间选择框";
            }
            if (fieldType == InputType.Hidden.Value)
            {
                return "隐藏";
            }

            throw new Exception();
        }

        public static bool IsSelectFieldType(string fieldType)
        {
            return EqualsIgnoreCase(fieldType, InputType.CheckBox.Value) ||
                   EqualsIgnoreCase(fieldType, InputType.Radio.Value) ||
                   EqualsIgnoreCase(fieldType, InputType.SelectMultiple.Value) ||
                   EqualsIgnoreCase(fieldType, InputType.SelectOne.Value);
        }

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

        public static bool StartsWithIgnoreCase(string text, string startString)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(startString)) return false;
            return text.Trim().ToLower().StartsWith(startString.Trim().ToLower()) || string.Equals(text.Trim(), startString.Trim(), StringComparison.OrdinalIgnoreCase);
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

        public static int ToIntWithNagetive(string intStr, int defaultValue)
        {
            int i;
            if (!int.TryParse(intStr?.Trim(), out i))
            {
                i = defaultValue;
            }
            return i;
        }

        public static decimal ToDecimalWithNagetive(string intStr, decimal defaultValue)
        {
            decimal i;
            if (!decimal.TryParse(intStr?.Trim(), out i))
            {
                i = defaultValue;
            }
            return i;
        }

        public static bool ToBool(string boolStr, bool defaultValue)
        {
            bool boolean;
            if (!bool.TryParse(boolStr?.Trim(), out boolean))
            {
                boolean = defaultValue;
            }
            return boolean;
        }

        public static string ObjectCollectionToString(ICollection collection)
        {
            var builder = new StringBuilder();
            if (collection != null)
            {
                foreach (var obj in collection)
                {
                    builder.Append(obj.ToString().Trim()).Append(",");
                }
                if (builder.Length != 0) builder.Remove(builder.Length - 1, 1);
            }
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

        public static string GetDirectoryPath(string path)
        {
            var ext = Path.GetExtension(path);
            var directoryPath = !string.IsNullOrEmpty(ext) ? Path.GetDirectoryName(path) : path;
            return directoryPath;
        }

        public static void CreateDirectoryIfNotExists(string path)
        {
            var directoryPath = GetDirectoryPath(path);

            if (!IsDirectoryExists(directoryPath))
            {
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

        public static bool CopyFile(string sourceFilePath, string destFilePath, bool isOverride)
        {
            var returnValue = true;
            try
            {
                CreateDirectoryIfNotExists(destFilePath);

                File.Copy(sourceFilePath, destFilePath, isOverride);
            }
            catch
            {
                returnValue = false;
            }
            return returnValue;
        }

        public const char UrlSeparatorChar = '/';
        public const char PathSeparatorChar = '\\';

        public static string PathCombine(params string[] paths)
        {
            var retval = string.Empty;
            if (paths != null && paths.Length > 0)
            {
                retval = paths[0]?.Replace(UrlSeparatorChar, PathSeparatorChar).TrimEnd(PathSeparatorChar) ?? string.Empty;
                for (var i = 1; i < paths.Length; i++)
                {
                    var path = paths[i] != null ? paths[i].Replace(UrlSeparatorChar, PathSeparatorChar).Trim(PathSeparatorChar) : string.Empty;
                    retval = Path.Combine(retval, path);
                }
            }
            return retval;
        }

        public static string[] GetDirectoryNames(string directoryPath)
        {
            var directorys = Directory.GetDirectories(directoryPath);
            var retval = new string[directorys.Length];
            var i = 0;
            foreach (var directory in directorys)
            {
                var directoryInfo = new DirectoryInfo(directory);
                retval[i++] = directoryInfo.Name;
            }
            return retval;
        }

        public static bool IsDirectoryExists(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        public static bool DeleteDirectoryIfExists(string directoryPath)
        {
            var retval = true;
            try
            {
                if (IsDirectoryExists(directoryPath))
                {
                    Directory.Delete(directoryPath, true);
                }
            }
            catch
            {
                retval = false;
            }
            return retval;
        }

        public static bool IsFileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public static string ReadText(string filePath)
        {
            var sr = new StreamReader(filePath, Encoding.UTF8);
            var text = sr.ReadToEnd();
            sr.Close();
            return text;
        }

        public static void WriteText(string filePath, string content)
        {
            var file = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
            using (var writer = new StreamWriter(file, Encoding.UTF8))
            {
                writer.Write(content);
                writer.Flush();
                writer.Close();

                file.Close();
            }
        }

        public static string GetShortGuid(bool isUppercase)
        {
            long i = 1;
            foreach (var b in Guid.NewGuid().ToByteArray())
            {
                i *= b + 1;
            }
            string retval = $"{i - DateTime.Now.Ticks:x}";
            return isUppercase ? retval.ToUpper() : retval.ToLower();
        }
    }
}
