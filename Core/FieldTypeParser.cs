using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using SiteServer.Plugin;
using SS.Form.Model;

namespace SS.Form.Core
{
    public class FieldTypeParser
    {
        private FieldTypeParser()
        {
        }

        public static string Parse(FieldInfo fieldInfo, FieldSettings settings)
        {
            string retval;

            var fieldType = fieldInfo.FieldType;
            var attributeName = FieldManager.GetAttributeId(fieldInfo.Id);
            var attributes = GetGeneralAttributes(fieldType, attributeName, settings);

            if (fieldType == InputType.Text.Value)
            {
                retval = ParseText(attributeName, attributes, fieldInfo, settings);
            }
            else if (fieldType == InputType.TextArea.Value)
            {
                retval = ParseTextArea(attributeName, attributes, fieldInfo, settings);
            }
            else if (fieldType == InputType.CheckBox.Value)
            {
                retval = ParseCheckBox(attributeName, attributes, fieldInfo);
            }
            else if (fieldType == InputType.Radio.Value)
            {
                retval = ParseRadio(attributeName, attributes, fieldInfo);
            }
            else if (fieldType == InputType.SelectOne.Value)
            {
                retval = ParseSelectOne(attributes, fieldInfo);
            }
            else if (fieldType == InputType.SelectMultiple.Value)
            {
                retval = ParseSelectMultiple(attributes, fieldInfo);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

            return retval;
        }

        private static string GetGeneralAttributes(string fieldType, string attributeName, FieldSettings settings)
        {
            var attributes = new Dictionary<string, string>
            {
                {"name", attributeName},
                {"v-model", $"attributes.{attributeName}"}
            };

            var validateList = new List<string>();
            if (settings.IsRequired)
            {
                validateList.Add("required");
            }

            if (fieldType == InputType.Text.Value || fieldType == InputType.TextArea.Value)
            {
                attributes.Add(":class", $@"{{'error': errors.has('{attributeName}') }}");
                if (settings.MinNum > 0)
                {
                    validateList.Add($"min:{settings.MinNum}");
                }
                if (settings.MaxNum > 0)
                {
                    validateList.Add($"max:{settings.MaxNum}");
                }

                if (settings.ValidateType == ValidateType.Integer)
                    validateList.Add("numeric");
                else if (settings.ValidateType == ValidateType.Email)
                {
                    validateList.Add("email");
                }
                else if (settings.ValidateType == ValidateType.Chinese)
                {
                    validateList.Add("regex:^[\\u0391-\\uFFE5]+$");
                }
                else if (settings.ValidateType == ValidateType.English)
                {
                    validateList.Add("regex:^[A-Za-z]+$");
                }
                else if (settings.ValidateType == ValidateType.Url)
                {
                    validateList.Add("url:require_protocol");
                }
                else if (settings.ValidateType == ValidateType.Phone)
                {
                    validateList.Add("regex:^((\\(\\d{3}\\))|(\\d{3}\\-))?(\\(0\\d{2,3}\\)|0\\d{2,3}-)?[1-9]\\d{6,7}$");
                }
                else if (settings.ValidateType == ValidateType.Mobile)
                {
                    validateList.Add("regex:^((\\(\\d{3}\\))|(\\d{3}\\-))?1\\d{10}$");
                }
                else if (settings.ValidateType == ValidateType.Currency)
                {
                    validateList.Add("decimal");
                }
                else if (settings.ValidateType == ValidateType.Zip)
                {
                    validateList.Add("regex:^[1-9]\\d{5}$");
                }
                else if (settings.ValidateType == ValidateType.IdCard)
                {
                    validateList.Add("regex:^\\d{15}(\\d{2}[A-Za-z0-9])?$");
                }
            }
            
            if (validateList.Count > 0)
            {
                attributes.Add("v-validate", $@"'{string.Join("|", validateList)}'");
            }

            var builder = new StringBuilder();
            foreach (var attribute in attributes)
            {
                builder.Append($@"{attribute.Key}=""{attribute.Value}"" ");
            }

            return builder.ToString();
        }

        public static string ParseText(string attributeName, string attributes, FieldInfo fieldInfo, FieldSettings settings)
        {
            return $@"<input type=""text"" value=""{fieldInfo.Value}"" {attributes}/>";
        }

        public static string ParseTextArea(string attributeName, string attributes, FieldInfo fieldInfo, FieldSettings settings)
        {
            return $@"<textarea {attributes}>{HttpUtility.HtmlDecode(fieldInfo.Value)}</textarea>";
        }

        private static string ParseCheckBox(string attributeName, string attributes, FieldInfo fieldInfo)
        {
            var builder = new StringBuilder();
            var items = fieldInfo.Items ?? new List<FieldItemInfo>();

            foreach (var item in items)
            {
                builder.Append(
                    $@"<input type=""checkbox"" id=""{attributeName}_{item.Id}"" value=""{item.Value}"" {attributes}><label for=""{attributeName}_{item.Id}"">{item.Value}</label>");
                if (item.IsExtras)
                {
                    var extrasId = FieldManager.GetExtrasId(fieldInfo.Id, item.Id);
                    builder.Append($@"<input type=""text"" value="""" name=""{extrasId}"" v-model=""attributes.{extrasId}"" style=""width: 300px;margin-left: 10px;"">");
                }
                builder.Append("<br />");
            }
            builder.Length -= 6;

            return builder.ToString();
        }

        private static string ParseRadio(string attributeName, string attributes, FieldInfo fieldInfo)
        {
            var builder = new StringBuilder();
            var items = fieldInfo.Items ?? new List<FieldItemInfo>();

            foreach (var item in items)
            {
                builder.Append(
                    $@"<input type=""radio"" id=""{attributeName}_{item.Id}"" value=""{item.Value}"" {attributes}><label for=""{attributeName}_{item.Id}"">{item.Value}</label>");
                if (item.IsExtras)
                {
                    var extrasId = FieldManager.GetExtrasId(fieldInfo.Id, item.Id);
                    builder.Append($@"<input type=""text"" value="""" name=""{extrasId}"" v-model=""attributes.{extrasId}"" style=""width: 300px;margin-left: 10px;"">");
                }
                builder.Append("<br />");
            }
            builder.Length -= 6;

            return builder.ToString();
        }

        private static string ParseSelectOne(string attributes, FieldInfo fieldInfo)
        {
            var builder = new StringBuilder();

            var items = fieldInfo.Items ?? new List<FieldItemInfo>();

            var selectedValue = !string.IsNullOrEmpty(fieldInfo.Value) ? fieldInfo.Value : string.Empty;

            builder.Append($@"<select {attributes}>");
            foreach (var item in items)
            {
                var isSelected = item.Value == selectedValue ? "selected" : string.Empty;

                builder.Append($@"<option value=""{item.Value}"" {isSelected}>{item.Value}</option>");
            }
            builder.Append("</select>");

            return builder.ToString();
        }

        private static string ParseSelectMultiple(string attributes, FieldInfo fieldInfo)
        {
            var builder = new StringBuilder();

            var items = fieldInfo.Items ?? new List<FieldItemInfo>();

            var selectedValues = !string.IsNullOrEmpty(fieldInfo.Value) ? fieldInfo.Value : string.Empty;
            var selectedValueList = Utils.StringCollectionToStringList(selectedValues);

            builder.Append($@"<select multiple style=""height: 130px"" {attributes}>");
            foreach (var item in items)
            {
                var isSelected = selectedValueList.Contains(item.Value) ? "selected" : string.Empty;

                builder.Append($@"<option value=""{item.Value}"" {isSelected}>{item.Value}</option>");
            }
            builder.Append("</select>");

            return builder.ToString();
        }
    }
}
