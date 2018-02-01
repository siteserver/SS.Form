using System;
using System.Web.UI.WebControls;
using SS.Form.Model;

namespace SS.Form.Core
{
    public class FieldTypeUtils
    {
        public static string GetValue(FieldType type)
        {
            if (type == FieldType.CheckBox)
            {
                return "CheckBox";
            }
            if (type == FieldType.Radio)
            {
                return "Radio";
            }
            if (type == FieldType.SelectOne)
            {
                return "SelectOne";
            }
            if (type == FieldType.SelectMultiple)
            {
                return "SelectMultiple";
            }
            if (type == FieldType.Text)
            {
                return "Text";
            }
            if (type == FieldType.TextArea)
            {
                return "TextArea";
            }
            throw new Exception();
        }

        public static string GetText(FieldType type)
        {
            if (type == FieldType.CheckBox)
            {
                return "多选项";
            }
            if (type == FieldType.Radio)
            {
                return "单选项";
            }
            if (type == FieldType.SelectOne)
            {
                return "下拉列表(单选)";
            }
            if (type == FieldType.SelectMultiple)
            {
                return "下拉列表(多选)";
            }
            if (type == FieldType.Text)
            {
                return "文本框(单行)";
            }
            if (type == FieldType.TextArea)
            {
                return "文本框(多行)";
            }
            throw new Exception();
        }

        public static FieldType GetEnumType(string typeStr)
        {
            var retval = FieldType.Text;

            if (Equals(FieldType.CheckBox, typeStr))
            {
                retval = FieldType.CheckBox;
            }
            else if (Equals(FieldType.Radio, typeStr))
            {
                retval = FieldType.Radio;
            }
            else if (Equals(FieldType.SelectOne, typeStr))
            {
                retval = FieldType.SelectOne;
            }
            else if (Equals(FieldType.SelectMultiple, typeStr))
            {
                retval = FieldType.SelectMultiple;
            }
            else if (Equals(FieldType.Text, typeStr))
            {
                retval = FieldType.Text;
            }
            else if (Equals(FieldType.TextArea, typeStr))
            {
                retval = FieldType.TextArea;
            }

            return retval;
        }

        public static bool Equals(FieldType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, FieldType type)
        {
            return Equals(type, typeStr);
        }

        public static bool EqualsAny(string typeStr, params FieldType[] types)
        {
            foreach (var type in types)
            {
                if (Equals(type, typeStr))
                {
                    return true;
                }
            }
            return false;
        }

        public static ListItem GetListItem(FieldType type, bool selected)
        {
            var item = new ListItem(GetText(type), GetValue(type));
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
                listControl.Items.Add(GetListItem(FieldType.Text, false));
                listControl.Items.Add(GetListItem(FieldType.TextArea, false));
                listControl.Items.Add(GetListItem(FieldType.CheckBox, false));
                listControl.Items.Add(GetListItem(FieldType.Radio, false));
                listControl.Items.Add(GetListItem(FieldType.SelectOne, false));
                listControl.Items.Add(GetListItem(FieldType.SelectMultiple, false));
            }
        }

        public static bool IsItems(FieldType type)
        {
            if (type == FieldType.CheckBox || type == FieldType.Radio || type == FieldType.SelectMultiple || type == FieldType.SelectOne)
            {
                return true;
            }
            return false;
        }

        public static bool IsPureString(FieldType type)
        {
            if (type == FieldType.CheckBox || type == FieldType.Radio || type == FieldType.SelectMultiple || type == FieldType.SelectOne)
            {
                return false;
            }
            return true;
        }
    }
}
