using System.Collections.Generic;
using System.Text;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Model;

namespace SS.Form.Parse
{
    public class StlForm
    {
        private StlForm() { }
        public const string ElementName = "stl:form";

        public const string AttributeTitle = "title";
        public const string AttributeTheme = "theme";

        public static string Parse(IParseContext context)
        {
            var title = string.Empty;
            var theme = string.Empty;

            foreach (var name in context.StlElementAttributes.Keys)
            {
                var value = context.StlElementAttributes[name];

                if (Utils.EqualsIgnoreCase(name, AttributeTitle))
                {
                    title = Main.Instance.ParseApi.ParseAttributeValue(value, context);
                }
                else if (Utils.EqualsIgnoreCase(name, AttributeTheme))
                {
                    theme = value;
                }
            }

            var formId = !string.IsNullOrEmpty(title) ? Main.Instance.FormDao.GetFormIdByTitle(context.SiteId, title) : Main.Instance.FormDao.GetFormIdByContentId(context.SiteId, context.ChannelId, context.ContentId);
            if (formId <= 0) return string.Empty;

            var formInfo = Main.Instance.FormDao.GetFormInfo(formId);
            var formSettings = new FormSettings(formInfo.Settings);

            if (string.IsNullOrEmpty(theme))
            {
                theme = formSettings.DefaultTheme;
            }
            theme = ParseUtils.GetTheme(theme);

            var fieldInfoList = Main.Instance.FieldDao.GetFieldInfoList(formId, true);

            var vueId = "vm_" + System.Guid.NewGuid().ToString().Replace("-", string.Empty);

            string template;
            if (!string.IsNullOrWhiteSpace(context.StlElementInnerXml))
            {
                template = Main.Instance.ParseApi.ParseInnerXml(context.StlElementInnerXml, context);
            }
            else
            {
                string styleUrl;
                template = ParseUtils.GetTemplateHtml(theme, out styleUrl);
                if (!context.HeadCodes.ContainsKey(nameof(StlForm)))
                {
                    context.HeadCodes.Add(nameof(StlForm), $@"<link rel=""stylesheet"" type=""text/css"" href=""{styleUrl}"" />");
                }
            }

            var pluginUrl = Main.Instance.PluginApi.GetPluginUrl();
            var imgUrl = Main.Instance.PluginApi.GetPluginApiUrl(nameof(ApiUtils.Captcha), formInfo.Id.ToString());
            var apiUrlSubmit = Main.Instance.PluginApi.GetPluginApiUrl(nameof(ApiUtils.Submit), formInfo.Id.ToString());

            if (!context.FootCodes.ContainsKey(nameof(StlForm)))
            {
                context.FootCodes.Add(nameof(StlForm), $@"
<script src=""{pluginUrl}/assets/js/vue-2.1.10.min.js"" type=""text/javascript""></script>
<script src=""{pluginUrl}/assets/js/vee-validate.js"" type=""text/javascript""></script>
<script src=""{pluginUrl}/assets/js/jquery.min.js"" type=""text/javascript""></script>");
            }

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

            context.FootCodes[nameof(StlForm) + vueId] = $@"
<script type=""text/javascript"">
Vue.use(VeeValidate);
new Vue({{
    el: '#{vueId}',
    data: {{
        title: {Utils.JsonSerialize(formInfo.Title)},
        description: {Utils.JsonSerialize(formInfo.Description)},
        isTimeout: {formInfo.IsTimeout.ToString().ToLower()},
        timeToStart: new Date('{formInfo.TimeToStart:yyyy-MM-dd HH:mm}'),
        timeToEnd: new Date('{formInfo.TimeToEnd:yyyy-MM-dd HH:mm}'),
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

            return $@"
<div id=""{vueId}"">
    {template}
</div>
";

            //var formElements = Utils.GetHtmlFormElements(templateBuilder.ToString());
            //if (formElements != null && formElements.Count > 0)
            //{
            //    foreach (var formElement in formElements)
            //    {
            //        string tagName;
            //        string innerXml;
            //        NameValueCollection attributes;
            //        Utils.ParseHtmlElement(formElement, out tagName, out innerXml, out attributes);

            //        if (string.IsNullOrEmpty(attributes["id"])) continue;

            //        foreach (var fieldInfo in fieldInfoList)
            //        {
            //            if (!Utils.EqualsIgnoreCase(fieldInfo.Title, attributes["id"])) continue;

            //            //var fieldSettings = new FieldSettings(fieldInfo.Settings);

            //            attributes["id"] = fieldInfo.Title;
            //            attributes["name"] = fieldInfo.Title;

            //            var replace = Utils.EqualsIgnoreCase(tagName, "input")
            //                ? $@"<{tagName} {Utils.ToAttributesString(attributes)} />"
            //                : $@"<{tagName} {Utils.ToAttributesString(attributes)} >{innerXml}</{tagName}>";

            //            templateBuilder.Replace(formElement, replace);
            //        }
            //    }
            //}

            //Utils.RewriteSubmitButton(templateBuilder, $"inputSubmit(this, '{stlFormId}', '{stlContainerId}', [{Utils.ToStringWithQuote(attributeNames)}]);return false;");

            //return Main.Instance.ParseApi.ParseInnerXml(templateBuilder.ToString(), context);
        }

        //        private static string GetDefaultTemplate(FormInfo formInfo, List<FieldInfo> fieldInfoList)
        //        {
        //            if (fieldInfoList == null) return string.Empty;

        //            var header = !string.IsNullOrEmpty(formInfo.Title) ? $@"<h1 class=""ss_form_head_title"">{formInfo.Title}</h1>" : string.Empty;
        //            if (!string.IsNullOrEmpty(formInfo.Description))
        //            {
        //                header += $@"<div class=""ss_form_head_desc"">{formInfo.Description}</div>";
        //            }

        //            var form = string.Empty;
        //            foreach (var fieldInfo in fieldInfoList)
        //            {
        //                var attributeName = FieldManager.GetAttributeId(fieldInfo.Id);
        //                var settings = new FieldSettings(fieldInfo.Settings);
        //                var title = fieldInfo.Title;
        //                if (settings.IsRequired)
        //                {
        //                    title += "<i>*</i>";
        //                }
        //                if (!string.IsNullOrEmpty(fieldInfo.Description))
        //                {
        //                    title += $"<div>{fieldInfo.Description}</div>";
        //                }
        //                var body = FieldTypeParser.Parse(fieldInfo, settings);

        //                form += $@"
        //<li>
        //  <div class=""ss_form_title"">
        //    {title}
        //    <div class=""error"" v-show=""errors.has('{attributeName}')"">请填写此项</div>
        //  </div>
        //  <div class=""ss_form_body"">
        //    {body}
        //  </div>
        //</li>
        //";
        //            }

        //            return $@"
        //<div class=""ss_form_wrapper"">
        //	<div class=""ss_form_main"">
        //		{header}
        //		<div class=""ss_form_bg""></div>

        //        <ul class=""ss_form_ul"">
        //            {form}
        //        </ul>
        //        <div class=""ss_form_failure"" v-show=""errorMessage"" v-html=""errorMessage"" style=""display: none""></div>
        //        <div>
        //            <div class=""ss_form_code"">
        //			    <input type=""text"" v-model=""code"" />
        //			    <img :src=""imgUrl"" class=""ss_form_code_img"" @click=""reload"" style=""width: 105px; height: 35px"" />
        //			    <span>请输入验证码</span>
        //		    </div>
        //            <a @click=""submit"" href=""javascript:;"" class=""ss_form_btn"">提 交</a>
        //        </div>
        //	</div>
        //</div>
        //";
        //        }

        //        private static string GetDefaultYes(FormInfo formInfo, List<FieldInfo> fieldInfoList)
        //        {
        //            if (fieldInfoList == null) return string.Empty;

        //            var header = !string.IsNullOrEmpty(formInfo.Title) ? $@"<h1 class=""ss_form_head_title"">{formInfo.Title}</h1>" : string.Empty;
        //            if (!string.IsNullOrEmpty(formInfo.Description))
        //            {
        //                header += $@"<div class=""ss_form_head_desc"">{formInfo.Description}</div>";
        //            }

        //            return $@"
        //<div class=""ss_form_wrapper"">
        //	<div class=""ss_form_main"">
        //		{header}
        //		<div class=""ss_form_bg""></div>
        //        <div class=""ss_form_success"">表单提交成功</div>
        //	</div>
        //</div>
        //";
        //        }

        //        public static string GetPostMessageScript(int formId, bool isSuccess)
        //        {
        //            var containerId = $"stl_input_{formId}";
        //            return $"<script>window.parent.postMessage({{containerId: '{containerId}', isSuccess: {isSuccess.ToString().ToLower()}}}, '*');</script>";
        //        }
    }
}
