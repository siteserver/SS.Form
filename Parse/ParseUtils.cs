using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Model;

namespace SS.Form.Parse
{
    public static class ParseUtils
    {
        public static void RegisterPage(IParseContext context)
        {
            var assetsUrl = PluginContext.PluginApi.GetPluginUrl("assets");

            context.FootCodes["SS.Form.Parse.Foot"] = $@"
<script src=""{assetsUrl}/js/vue-2.1.10.min.js"" type=""text/javascript""></script>
<script src=""{assetsUrl}/js/vee-validate.js"" type=""text/javascript""></script>
<script src=""{assetsUrl}/js/jquery.min.js"" type=""text/javascript""></script>";
        }

        public static void RegisterCode(IParseContext context, string vueId, FormInfo formInfo, FormSettings formSettings)
        {
            var fieldInfoList = Main.FieldDao.GetFieldInfoList(formInfo.Id, true);

            var imgUrl = $"{PluginContext.PluginApi.GetPluginApiUrl(Main.PluginId)}/{nameof(ApiUtils.Captcha)}/{formInfo.Id}";
            var apiUrlSubmit = $"{PluginContext.PluginApi.GetPluginApiUrl(Main.PluginId)}/{nameof(ApiUtils.Submit)}/{formInfo.Id}";

            var schemas = new List<object>();
            var values = new StringBuilder();
            foreach (var fieldInfo in fieldInfoList)
            {
                var fieldType = fieldInfo.FieldType;
                var attributeName = FieldManager.GetAttributeId(fieldInfo.Id);
                var settings = new FieldSettings(fieldInfo.Settings);
                schemas.Add(new
                {
                    AttributeName = attributeName,
                    fieldInfo.Title,
                    fieldInfo.Description,
                    fieldInfo.FieldType,
                    settings.IsRequired,
                    Items = fieldInfo.Items ?? new List<FieldItemInfo>()
                });

                if (fieldType == InputType.SelectMultiple.Value || fieldType == InputType.CheckBox.Value)
                {
                    values.Append($"{attributeName}: [],");
                }
                else
                {
                    values.Append($"{attributeName}: '',");
                }
            }

            context.FootCodes["SS.Form.Parse.Foot." + vueId] = $@"
<script type=""text/javascript"">
Vue.use(VeeValidate);
var {vueId} = new Vue({{
    el: '#{vueId}',
    data: {{
        title: {Utils.JsonSerialize(formInfo.Title)},
        description: {Utils.JsonSerialize(formInfo.Description)},
        isEnabled: {(formInfo.IsTimeout ? $"new Date() > new Date('{formInfo.TimeToStart:yyyy-MM-dd HH:mm}') && new Date() < new Date('{formInfo.TimeToEnd:yyyy-MM-dd HH:mm}')" : true.ToString().ToLower())},
        isCaptcha: {formSettings.IsCaptcha.ToString().ToLower()},
        attributes: {{{values.ToString().Trim()}}},
        code: '',
        imgUrl: '',
        isSuccess: false,
        errorMessage: '',
        schemas: {Utils.JsonSerialize(schemas)}
    }},
    methods: {{
        load: function (event) {{
            this.imgUrl = '{imgUrl}?' + new Date().getTime();
        }},
        getAttributeName: function (title) {{
            var result = $.grep(this.schemas, function (e) {{ return e.title == title; }});
            if (result && result.length == 1) {{
                return result[0].attributeName;
            }}
            return '';
        }},
        submit: function (event) {{
            this.errorMessage = '';
            var $this = this;
            var data = {{
                code: this.code,
                attributes: this.attributes
            }};
          
            this.$validator.validateAll().then(function (result) {{
                if (result) {{
                    $.ajax({{
                        url : ""{apiUrlSubmit}"",
                        xhrFields: {{
                            withCredentials: true
                        }},
                        type: ""POST"",
                        data: JSON.stringify(data),
                        contentType: ""application/json; charset=utf-8"",
                        dataType: ""json"",
                        success: function(data)
                        {{
                            $this.isSuccess = true;
                        }},
                        error: function (err)
                        {{
                            var err = JSON.parse(err.responseText);
                            $this.isSuccess = false;
                            $this.errorMessage = err.message;
                        }}
                    }});
                }}
            }});
        }}
    }}
}});
{vueId}.load();
</script>";
        }

        public static string GetTheme(string theme)
        {
            var retval = theme;
            if (string.IsNullOrEmpty(retval) ||
                !File.Exists(PluginContext.PluginApi.GetPluginPath($"themes/{retval}/template.html")))
            {
                retval = "bootstrap-large";
            }
            return retval;
        }

        public static List<string> GetThemeList()
        {
            var directoryPath = PluginContext.PluginApi.GetPluginPath("themes");
            return Utils.GetDirectoryNames(directoryPath).ToList();
        }

        public static string GetTemplateHtml(string theme)
        {
            var htmlPath = PluginContext.PluginApi.GetPluginPath($"themes/{theme}/template.html");
            var themeUrl = PluginContext.PluginApi.GetPluginUrl($"themes/{theme}");

            var html = CacheUtils.Get<string>(htmlPath);
            if (html != null) return html;

            html = Utils.ReadText(htmlPath);
            var startIndex = html.IndexOf("<!-- template start -->", StringComparison.Ordinal) + "<!-- template start -->".Length;
            var length = html.IndexOf("<!-- template end -->", StringComparison.Ordinal) - startIndex;
            html = html.Substring(startIndex, length);

            html = $@"
<link rel=""stylesheet"" type=""text/css"" href=""{themeUrl}/style.css"" />
{html}";

            CacheUtils.InsertHours(htmlPath, html, 3);
            return html;
        }

        public static string GetFormStlElement(FormInfo formInfo)
        {
            var settings = new FormSettings(formInfo.Settings);

            var titleAttr = formInfo.ContentId == 0 && !string.IsNullOrEmpty(formInfo.Title)
                ? $@" title=""{formInfo.Title}"""
                : string.Empty;
            var theme = GetTheme(settings.DefaultTheme);

            return $@" <stl:form{titleAttr}>
    {GetTemplateHtml(theme)}
</stl:form>";
        }
    }
}
