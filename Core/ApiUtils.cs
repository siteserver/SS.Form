using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using SiteServer.Plugin;
using SS.Form.Model;
using SS.SMS;

namespace SS.Form.Core
{
    public static class ApiUtils
    {
        public static HttpResponseMessage Captcha(IRequest request, string id)
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

        public static object Submit(IRequest request, string id)
        {
            var formId = Convert.ToInt32(id);

            var formInfo = Main.Instance.FormDao.GetFormInfo(formId);
            if (formInfo == null) return null;

            var settings = new FormSettings(formInfo.Settings);

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

            var fieldInfoList = Main.Instance.FieldDao.GetFieldInfoList(formInfo.Id, true);
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

            logInfo.Id = Main.Instance.LogDao.Insert(logInfo);

            if (settings.IsAdministratorSmsNotify && !string.IsNullOrEmpty(settings.AdministratorSmsNotifyTplId) && !string.IsNullOrEmpty(settings.AdministratorSmsNotifyKeys) && !string.IsNullOrEmpty(settings.AdministratorSmsNotifyMobile))
            {
                var smsPlugin = SmsPlugin.Instance;
                if (smsPlugin != null && smsPlugin.IsReady)
                {
                    string errorMessage;
                    var parameters = new Dictionary<string, string>();
                    var keys = settings.AdministratorSmsNotifyKeys.Split(',');
                    foreach (var key in keys)
                    {
                        if (key == nameof(LogInfo.Id))
                        {
                            parameters.Add(key, logInfo.Id.ToString());
                        }
                        else if (key == nameof(LogInfo.AddDate))
                        {
                            parameters.Add(key, logInfo.AddDate.ToString("yyyy-MM-dd HH:mm"));
                        }
                        else
                        {
                            parameters.Add(key, logInfo.GetString(key));
                        }
                    }
                    smsPlugin.Send(settings.AdministratorSmsNotifyMobile, settings.AdministratorSmsNotifyTplId, parameters, out errorMessage);
                }
            }

            return new
            {

            };
        }
    }
}
