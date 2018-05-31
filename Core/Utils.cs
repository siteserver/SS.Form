using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using SiteServer.Plugin;

namespace SS.Form.Core
{
    public static class Utils
    {
        public static readonly Color[] Colors = { Color.FromArgb(37, 72, 91), Color.FromArgb(68, 24, 25), Color.FromArgb(17, 46, 2), Color.FromArgb(70, 16, 100), Color.FromArgb(24, 88, 74) };

        public static string GetFieldTypeText(string fieldType)
        {
            if (fieldType == InputType.CheckBox.Value)
            {
                return "多选项";
            }
            if (fieldType == InputType.Radio.Value)
            {
                return "单选项";
            }
            if (fieldType == InputType.SelectOne.Value)
            {
                return "下拉列表(单选)";
            }
            if (fieldType == InputType.SelectMultiple.Value)
            {
                return "下拉列表(多选)";
            }
            if (fieldType == InputType.Text.Value)
            {
                return "文本框(单行)";
            }
            if (fieldType == InputType.TextArea.Value)
            {
                return "文本框(多行)";
            }
            throw new Exception();
        }

        public static ListItem GetListItem(InputType type, bool selected)
        {
            var item = new ListItem(GetFieldTypeText(type.Value), type.Value);
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItems(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(InputType.Text, false));
                listControl.Items.Add(GetListItem(InputType.TextArea, false));
                listControl.Items.Add(GetListItem(InputType.CheckBox, false));
                listControl.Items.Add(GetListItem(InputType.Radio, false));
                listControl.Items.Add(GetListItem(InputType.SelectOne, false));
                listControl.Items.Add(GetListItem(InputType.SelectMultiple, false));
            }
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

        private static DateTime ToDateTime(string dateTimeStr, DateTime defaultValue)
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

        public static int ToInt(string intStr)
        {
            int i;
            if (!int.TryParse(intStr?.Trim(), out i))
            {
                i = 0;
            }
            return i;
        }

        public static string GetMessageHtml(string message, bool isSuccess)
        {
            return isSuccess
                ? $@"<div class=""alert alert-success"" role=""alert"">{message}</div>"
                : $@"<div class=""alert alert-danger"" role=""alert"">{message}</div>";
        }

        public static string ReplaceNewline(string inputString, string replacement)
        {
            if (string.IsNullOrEmpty(inputString)) return string.Empty;
            var retVal = new StringBuilder();
            inputString = inputString.Trim();
            foreach (var t in inputString)
            {
                switch (t)
                {
                    case '\n':
                        retVal.Append(replacement);
                        break;
                    case '\r':
                        break;
                    default:
                        retVal.Append(t);
                        break;
                }
            }
            return retVal.ToString();
        }

        public static string GetSelectedListControlValueCollection(ListControl listControl)
        {
            var list = new List<string>();
            if (listControl != null)
            {
                foreach (ListItem item in listControl.Items)
                {
                    if (item.Selected)
                    {
                        list.Add(item.Value);
                    }
                }
            }
            return string.Join(",", list);
        }

        public static List<string> StringCollectionToStringList(string collection)
        {
            return StringCollectionToStringList(collection, ',');
        }

        public static List<string> StringCollectionToStringList(string collection, char split)
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

        public static string ReadText(string filePath)
        {
            var sr = new StreamReader(filePath, Encoding.UTF8);
            var text = sr.ReadToEnd();
            sr.Close();
            return text;
        }

        public static void SelectSingleItem(ListControl listControl, string value)
        {
            if (listControl == null) return;

            listControl.ClearSelection();

            foreach (ListItem item in listControl.Items)
            {
                if (string.Equals(item.Value, value))
                {
                    item.Selected = true;
                    break;
                }
            }
        }

        public static void SelectMultiItems(ListControl listControl, params string[] values)
        {
            if (listControl == null) return;

            listControl.ClearSelection();
            foreach (ListItem item in listControl.Items)
            {
                foreach (var value in values)
                {
                    if (string.Equals(item.Value, value))
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }
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
    }
}
