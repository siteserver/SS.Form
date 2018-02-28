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
            var assetsUrl = Main.Instance.PluginApi.GetPluginUrl("assets");

            context.StlPageHead["SS.Form.Parse.Head"] = $@"
<link rel=""stylesheet"" type=""text/css"" href=""{assetsUrl}/normalize.css"" />";

            context.StlPageFoot["SS.Form.Parse.Foot"] = $@"
<script src=""{assetsUrl}/js/vue-2.1.10.min.js"" type=""text/javascript""></script>
<script src=""{assetsUrl}/js/vee-validate.js"" type=""text/javascript""></script>
<script src=""{assetsUrl}/js/jquery.min.js"" type=""text/javascript""></script>";
        }

        public static void RegisterFormCode(IParseContext context, string vueId, FormInfo formInfo, FormSettings formSettings)
        {
            var fieldInfoList = Main.Instance.FieldDao.GetFieldInfoList(formInfo.Id, true);

            var imgUrl = Main.Instance.PluginApi.GetPluginApiUrl(nameof(ApiUtils.Captcha), formInfo.Id.ToString());
            var apiUrlSubmit = Main.Instance.PluginApi.GetPluginApiUrl(nameof(ApiUtils.Submit), formInfo.Id.ToString());

            var schemas = new List<object>();
            var values = new StringBuilder();
            foreach (var fieldInfo in fieldInfoList)
            {
                var fieldType = FieldTypeUtils.GetEnumType(fieldInfo.FieldType);
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

                if (fieldType == FieldType.SelectMultiple || fieldType == FieldType.CheckBox)
                {
                    values.Append($"{attributeName}: [],");
                }
                else
                {
                    values.Append($"{attributeName}: '',");
                }
            }

            context.StlPageFoot["SS.Form.Parse.Foot." + vueId] = $@"
<script type=""text/javascript"">
Vue.use(VeeValidate);
new Vue({{
    el: '#{vueId}',
    data: {{
        title: {Utils.JsonSerialize(formInfo.Title)},
        description: {Utils.JsonSerialize(formInfo.Description)},
        isEnabled: {(formInfo.IsTimeout ? $"new Date() > new Date('{formInfo.TimeToStart:yyyy-MM-dd HH:mm}') && new Date() < new Date('{formInfo.TimeToEnd:yyyy-MM-dd HH:mm}')" : true.ToString().ToLower())},
        isCaptcha: {formSettings.IsCaptcha.ToString().ToLower()},
        attributes: {{{values.ToString().Trim()}}},
        code: '',
        imgUrl: '{imgUrl}?' + new Date().getTime(),
        isSuccess: false,
        errorMessage: '',
        schemas: {Utils.JsonSerialize(schemas)}
    }},
    methods: {{
        reload: function (event) {{
            this.imgUrl = '{imgUrl}?' + new Date().getTime();
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
</script>";
        }

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
            var themeUrl = Main.Instance.PluginApi.GetPluginUrl($"themes/{theme}");

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
