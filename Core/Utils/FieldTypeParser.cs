using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using SiteServer.Plugin;
using SS.Form.Core.Model;

namespace SS.Form.Core.Utils
{
    //public class FieldTypeParser
    //{
    //    private FieldTypeParser()
    //    {
    //    }

    //    public static string Parse(FieldInfo fieldInfo, FieldSettings settings)
    //    {
    //        string retval;

    //        var fieldType = fieldInfo.FieldType;
    //        var attributeName = FieldManager.GetAttributeId(fieldInfo.Id);
    //        var attributes = GetGeneralAttributes(attributeName, fieldInfo);

    //        if (fieldType == InputType.Text.Value)
    //        {
    //            retval = ParseText(attributeName, attributes, fieldInfo, settings);
    //        }
    //        else if (fieldType == InputType.TextArea.Value)
    //        {
    //            retval = ParseTextArea(attributeName, attributes, fieldInfo, settings);
    //        }
    //        else if (fieldType == InputType.CheckBox.Value)
    //        {
    //            retval = ParseCheckBox(attributeName, attributes, fieldInfo);
    //        }
    //        else if (fieldType == InputType.Radio.Value)
    //        {
    //            retval = ParseRadio(attributeName, attributes, fieldInfo);
    //        }
    //        else if (fieldType == InputType.SelectOne.Value)
    //        {
    //            retval = ParseSelectOne(attributes, fieldInfo);
    //        }
    //        else if (fieldType == InputType.SelectMultiple.Value)
    //        {
    //            retval = ParseSelectMultiple(attributes, fieldInfo);
    //        }
    //        else
    //        {
    //            throw new ArgumentOutOfRangeException();
    //        }

    //        return retval;
    //    }

    //    private static string GetGeneralAttributes(string attributeName, FieldInfo fieldInfo)
    //    {
    //        var attributes = new Dictionary<string, string>
    //        {
    //            {"name", attributeName},
    //            {"v-model", $"attributes.{attributeName}"}
    //        };
            
    //        if (!string.IsNullOrEmpty(fieldInfo.Validate))
    //        {
    //            attributes.Add("v-validate", $@"'{fieldInfo.Validate}'");
    //        }

    //        var builder = new StringBuilder();
    //        foreach (var attribute in attributes)
    //        {
    //            builder.Append($@"{attribute.Key}=""{attribute.Value}"" ");
    //        }

    //        return builder.ToString();
    //    }

    //    public static string ParseText(string attributeName, string attributes, FieldInfo fieldInfo, FieldSettings settings)
    //    {
    //        return $@"<input type=""text"" value=""{fieldInfo.Value}"" {attributes}/>";
    //    }

    //    public static string ParseTextArea(string attributeName, string attributes, FieldInfo fieldInfo, FieldSettings settings)
    //    {
    //        return $@"<textarea {attributes}>{HttpUtility.HtmlDecode(fieldInfo.Value)}</textarea>";
    //    }

    //    private static string ParseCheckBox(string attributeName, string attributes, FieldInfo fieldInfo)
    //    {
    //        var builder = new StringBuilder();
    //        var items = fieldInfo.Items ?? new List<FieldItemInfo>();

    //        foreach (var item in items)
    //        {
    //            builder.Append(
    //                $@"<input type=""checkbox"" id=""{attributeName}_{item.Id}"" value=""{item.Value}"" {attributes}><label for=""{attributeName}_{item.Id}"">{item.Value}</label>");
    //            if (item.IsExtras)
    //            {
    //                var extrasId = FieldManager.GetExtrasId(fieldInfo.Id, item.Id);
    //                builder.Append($@"<input type=""text"" value="""" name=""{extrasId}"" v-model=""attributes.{extrasId}"" style=""width: 300px;margin-left: 10px;"">");
    //            }
    //            builder.Append("<br />");
    //        }
    //        builder.Length -= 6;

    //        return builder.ToString();
    //    }

    //    private static string ParseRadio(string attributeName, string attributes, FieldInfo fieldInfo)
    //    {
    //        var builder = new StringBuilder();
    //        var items = fieldInfo.Items ?? new List<FieldItemInfo>();

    //        foreach (var item in items)
    //        {
    //            builder.Append(
    //                $@"<input type=""radio"" id=""{attributeName}_{item.Id}"" value=""{item.Value}"" {attributes}><label for=""{attributeName}_{item.Id}"">{item.Value}</label>");
    //            if (item.IsExtras)
    //            {
    //                var extrasId = FieldManager.GetExtrasId(fieldInfo.Id, item.Id);
    //                builder.Append($@"<input type=""text"" value="""" name=""{extrasId}"" v-model=""attributes.{extrasId}"" style=""width: 300px;margin-left: 10px;"">");
    //            }
    //            builder.Append("<br />");
    //        }
    //        builder.Length -= 6;

    //        return builder.ToString();
    //    }

    //    private static string ParseSelectOne(string attributes, FieldInfo fieldInfo)
    //    {
    //        var builder = new StringBuilder();

    //        var items = fieldInfo.Items ?? new List<FieldItemInfo>();

    //        var selectedValue = !string.IsNullOrEmpty(fieldInfo.Value) ? fieldInfo.Value : string.Empty;

    //        builder.Append($@"<select {attributes}>");
    //        foreach (var item in items)
    //        {
    //            var isSelected = item.Value == selectedValue ? "selected" : string.Empty;

    //            builder.Append($@"<option value=""{item.Value}"" {isSelected}>{item.Value}</option>");
    //        }
    //        builder.Append("</select>");

    //        return builder.ToString();
    //    }

    //    private static string ParseSelectMultiple(string attributes, FieldInfo fieldInfo)
    //    {
    //        var builder = new StringBuilder();

    //        var items = fieldInfo.Items ?? new List<FieldItemInfo>();

    //        var selectedValues = !string.IsNullOrEmpty(fieldInfo.Value) ? fieldInfo.Value : string.Empty;
    //        var selectedValueList = FormUtils.StringCollectionToStringList(selectedValues);

    //        builder.Append($@"<select multiple style=""height: 130px"" {attributes}>");
    //        foreach (var item in items)
    //        {
    //            var isSelected = selectedValueList.Contains(item.Value) ? "selected" : string.Empty;

    //            builder.Append($@"<option value=""{item.Value}"" {isSelected}>{item.Value}</option>");
    //        }
    //        builder.Append("</select>");

    //        return builder.ToString();
    //    }
    //}
}
