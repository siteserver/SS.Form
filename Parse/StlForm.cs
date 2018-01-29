using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

        public static string Parse(IParseContext context)
        {
            var title = string.Empty;

            foreach (var name in context.Attributes.Keys)
            {
                var value = context.Attributes[name];

                if (Utils.EqualsIgnoreCase(name, AttributeTitle))
                {
                    title = value;
                }
            }

            var formId = !string.IsNullOrEmpty(title) ? Main.FormDao.GetFormIdByTitle(context.SiteId, title) : Main.FormDao.GetFormIdByContentId(context.SiteId, context.ChannelId, context.ContentId);
            if (formId <= 0) return string.Empty;

            var formInfo = Main.FormDao.GetFormInfo(formId);

            var fieldInfoList = Main.FieldDao.GetFieldInfoList(formId, true);

            var stlElements = Main.ParseApi.GetStlElements(context.InnerXml, new List<string>
            {
                "stl:template",
                "stl:yes"
            });
            string template;
            stlElements.TryGetValue("stl:template", out template);
            string yes;
            stlElements.TryGetValue("stl:yes", out yes);

            var vueId = $"stl_form_{formInfo.Id}";

            if (string.IsNullOrEmpty(template))
            {
                template = GetDefaultTemplate(formInfo, fieldInfoList);
            }
            if (string.IsNullOrEmpty(yes))
            {
                yes = GetDefaultYes(formInfo, fieldInfoList);
            }

            template = Main.ParseApi.ParseInnerXml(template, context);

            var pluginUrl = Main.FilesApi.GetPluginUrl();
            var imgUrl = Main.FilesApi.GetApiHttpUrl(nameof(ApiGetCode), formInfo.Id.ToString());
            var apiUrlSubmit = Main.FilesApi.GetApiJsonUrl(nameof(ApiSubmit), formInfo.Id.ToString());

            var values = new StringBuilder();
            foreach (var fieldInfo in fieldInfoList)
            {
                var fieldType = FieldTypeUtils.GetEnumType(fieldInfo.FieldType);
                var attributeName = FieldManager.GetAttributeId(fieldInfo.Id);
                if (fieldType == FieldType.SelectMultiple || fieldType == FieldType.CheckBox)
                {
                    values.Append($"{attributeName}: [],");
                }
                else
                {
                    values.Append($"{attributeName}: '',");
                }
            }

            var templateBuilder = new StringBuilder();
            templateBuilder.Append($@"
<link rel=""stylesheet"" type=""text/css"" href=""{pluginUrl}/assets/001/css/style.css"" />
<div id=""{vueId}"">
    {template}
    {yes}
</div>
<script src=""{pluginUrl}/assets/js/vue-2.1.10.min.js"" type=""text/javascript""></script>
<script src=""{pluginUrl}/assets/js/vee-validate.js"" type=""text/javascript""></script>
<script src=""{pluginUrl}/assets/js/jquery.min.js"" type=""text/javascript""></script>
<script type=""text/javascript"">
Vue.use(VeeValidate);
new Vue({{
    el: '#{vueId}',
    data: {{
        isTimeout: {formInfo.IsTimeout.ToString().ToLower()},
        timeToStart: new Date('{formInfo.TimeToStart:yyyy-MM-dd HH:mm}'),
        timeToEnd: new Date('{formInfo.TimeToEnd:yyyy-MM-dd HH:mm}'),
        attributes: {{{values.ToString().Trim()}}},
        code: '',
        imgUrl: '{imgUrl}?' + new Date().getTime(),
        isSuccess: false,
        errorMessage: ''
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
</script>
");

            var formElements = Utils.GetHtmlFormElements(templateBuilder.ToString());
            if (formElements != null && formElements.Count > 0)
            {
                foreach (var formElement in formElements)
                {
                    string tagName;
                    string innerXml;
                    NameValueCollection attributes;
                    Utils.ParseHtmlElement(formElement, out tagName, out innerXml, out attributes);

                    if (string.IsNullOrEmpty(attributes["id"])) continue;

                    foreach (var fieldInfo in fieldInfoList)
                    {
                        if (!Utils.EqualsIgnoreCase(fieldInfo.Title, attributes["id"])) continue;

                        //var fieldSettings = new FieldSettings(fieldInfo.Settings);

                        attributes["id"] = fieldInfo.Title;
                        attributes["name"] = fieldInfo.Title;

                        var replace = Utils.EqualsIgnoreCase(tagName, "input")
                            ? $@"<{tagName} {Utils.ToAttributesString(attributes)} />"
                            : $@"<{tagName} {Utils.ToAttributesString(attributes)} >{innerXml}</{tagName}>";

                        templateBuilder.Replace(formElement, replace);
                    }
                }
            }

            //Utils.RewriteSubmitButton(templateBuilder, $"inputSubmit(this, '{stlFormId}', '{stlContainerId}', [{Utils.ToStringWithQuote(attributeNames)}]);return false;");

            return Main.ParseApi.ParseInnerXml(templateBuilder.ToString(), context);
        }

        public static HttpResponseMessage ApiGetCode(IRequest request, string id)
        {
            var response = new HttpResponseMessage();

            var random = new Random();
            var validateCode = "";

            char[] s = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            for (var i = 0; i < 4; i++)
            {
                validateCode += s[random.Next(0, s.Length)].ToString();
            }

            var validateimage = new Bitmap(105, 35, PixelFormat.Format32bppRgb);

            var colors = Utils.Colors[random.Next(0, 5)];

            var g = Graphics.FromImage(validateimage);
            g.FillRectangle(new SolidBrush(Color.FromArgb(240, 243, 248)), 0, 0, 105, 105); //矩形框
            g.DrawString(validateCode, new Font(FontFamily.GenericSerif, 24, FontStyle.Bold | FontStyle.Italic), new SolidBrush(colors), new PointF(10, 0));//字体/颜色

            for (var i = 0; i < 100; i++)
            {
                var x = random.Next(validateimage.Width);
                var y = random.Next(validateimage.Height);

                validateimage.SetPixel(x, y, Color.FromArgb(random.Next()));
            }

            g.Save();
            var ms = new MemoryStream();
            validateimage.Save(ms, ImageFormat.Png);

            request.SetCookie("ss-form:" + id, validateCode, DateTime.Now.AddDays(1));

            response.Content = new ByteArrayContent(ms.ToArray());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            response.StatusCode = HttpStatusCode.OK;

            return response;
        }

        public static object ApiSubmit(IRequest request, string id)
        {
            var formId = Convert.ToInt32(id);

            var formInfo = Main.FormDao.GetFormInfo(formId);
            if (formInfo == null) return null;

            var code = request.GetPostString("code");
            var cookie = request.GetCookie("ss-form:" + id);
            if (string.IsNullOrEmpty(cookie) || !Utils.EqualsIgnoreCase(cookie, code))
            {
                throw new Exception("提交失败，验证码不正确！");
            }

            var logInfo = new LogInfo
            {
                FormId = formId,
                AddDate = DateTime.Now
            };

            var attributes = request.GetPostObject<Dictionary<string, object>>("attributes");

            var fieldInfoList = Main.FieldDao.GetFieldInfoList(formInfo.Id, true);
            foreach (var fieldInfo in fieldInfoList)
            {
                object value;
                attributes.TryGetValue(FieldManager.GetAttributeId(fieldInfo.Id), out value);
                if (value != null && value.ToString() != "[]")
                {
                    logInfo.Set(fieldInfo.Title, value.ToString());
                    if (FieldManager.IsExtra(fieldInfo))
                    {
                        foreach (var item in fieldInfo.Items)
                        {
                            var extrasId = FieldManager.GetExtrasId(fieldInfo.Id, item.Id);
                            object extras;
                            attributes.TryGetValue(extrasId, out extras);
                            if (!string.IsNullOrEmpty(extras?.ToString()))
                            {
                                logInfo.Set(extrasId, extras.ToString());
                            }
                        }
                    }
                }
            }

            Main.LogDao.Insert(logInfo);

            return new
            {

            };
        }

        private static string GetDefaultTemplate(FormInfo formInfo, List<FieldInfo> fieldInfoList)
        {
            if (fieldInfoList == null) return string.Empty;

            var header = !string.IsNullOrEmpty(formInfo.Title) ? $@"<h1 class=""ss_form_head_title"">{formInfo.Title}</h1>" : string.Empty;
            if (!string.IsNullOrEmpty(formInfo.Description))
            {
                header += $@"<div class=""ss_form_head_desc"">{formInfo.Description}</div>";
            }

            var form = string.Empty;
            foreach (var fieldInfo in fieldInfoList)
            {
                var attributeName = FieldManager.GetAttributeId(fieldInfo.Id);
                var settings = new FieldSettings(fieldInfo.Settings);
                var title = fieldInfo.Title;
                if (settings.IsRequired)
                {
                    title += "<i>*</i>";
                }
                if (!string.IsNullOrEmpty(fieldInfo.Description))
                {
                    title += $"<div>{fieldInfo.Description}</div>";
                }
                var body = FieldTypeParser.Parse(fieldInfo, settings);

                form += $@"
<li>
  <div class=""ss_form_title"">
    {title}
    <div class=""error"" v-show=""errors.has('{attributeName}')"">请填写此项</div>
  </div>
  <div class=""ss_form_body"">
    {body}
  </div>
</li>
";
            }

            return $@"
<div class=""ss_form_wrapper"" v-show=""!isSuccess"">
	<div class=""ss_form_main"">
		{header}
		<div class=""ss_form_bg""></div>

        <ul class=""ss_form_ul"">
            {form}
        </ul>
        <div class=""ss_form_failure"" v-show=""errorMessage"" v-html=""errorMessage"" style=""display: none""></div>
        <div>
            <div class=""ss_form_code"">
			    <input type=""text"" v-model=""code"" />
			    <img :src=""imgUrl"" class=""ss_form_code_img"" @click=""reload"" style=""width: 105px; height: 35px"" />
			    <span>请输入验证码</span>
		    </div>
            <a @click=""submit"" href=""javascript:;"" class=""ss_form_btn"">提 交</a>
        </div>
	</div>
</div>
";
        }

        private static string GetDefaultYes(FormInfo formInfo, List<FieldInfo> fieldInfoList)
        {
            if (fieldInfoList == null) return string.Empty;

            var header = !string.IsNullOrEmpty(formInfo.Title) ? $@"<h1 class=""ss_form_head_title"">{formInfo.Title}</h1>" : string.Empty;
            if (!string.IsNullOrEmpty(formInfo.Description))
            {
                header += $@"<div class=""ss_form_head_desc"">{formInfo.Description}</div>";
            }

            return $@"
<div class=""ss_form_wrapper"" v-show=""isSuccess"">
	<div class=""ss_form_main"">
		{header}
		<div class=""ss_form_bg""></div>
        <div class=""ss_form_success"" v-show=""isSuccess"" style=""display: none"">恭喜，表单提供成功。</div>
	</div>
</div>
";
        }

        public static string GetPostMessageScript(int formId, bool isSuccess)
        {
            var containerId = $"stl_input_{formId}";
            return $"<script>window.parent.postMessage({{containerId: '{containerId}', isSuccess: {isSuccess.ToString().ToLower()}}}, '*');</script>";
        }

        public static string GetDefaultStlFormStlElement(FormInfo formInfo)
        {
            var fieldInfoList = Main.FieldDao.GetFieldInfoList(formInfo.Id, true);
            var template = GetDefaultTemplate(formInfo, fieldInfoList);
            var yes = GetDefaultYes(formInfo, fieldInfoList);

            var titleAttr = formInfo.ContentId == 0 && !string.IsNullOrEmpty(formInfo.Title)
                ? $@" title=""{formInfo.Title}"""
                : string.Empty;

            return $@" <stl:form{titleAttr}>
    <stl:template>
        {template}
    </stl:template>

    <stl:yes>
        {yes}
    </stl:yes>
</stl:form>";
        }
    }
}
