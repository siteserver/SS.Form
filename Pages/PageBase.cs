using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SS.Form.Core;
using SS.Form.Model;
using SS.Form.Parse;

namespace SS.Form.Pages
{
    public class PageBase : Page
    {
        protected int PublishmentSystemId { get; private set; }
        protected FormInfo FormInfo { get; private set; }
        protected string ReturnUrl { get; private set; }

        public string PageLogsUrl { get; private set; }

        public string PageResultsUrl { get; private set; }

        public string PageFieldsUrl { get; private set; }

        public string PageSettingsUrl { get; private set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PublishmentSystemId = Convert.ToInt32(Request.QueryString["siteId"]);
            var channelId = Convert.ToInt32(Request.QueryString["channelId"]);
            var contentId = Convert.ToInt32(Request.QueryString["contentId"]);
            var formId = Convert.ToInt32(Request.QueryString["formId"]);
            FormInfo = formId > 0 ? Main.FormDao.GetFormInfo(formId) : Main.FormDao.GetFormInfoOrCreateIfNotExists(PublishmentSystemId, channelId, contentId);
            ReturnUrl = HttpUtility.UrlDecode(Request.QueryString["returnUrl"]);

            PageLogsUrl = Main.FilesApi.GetPluginUrl(
                $"{nameof(PageLogs)}.aspx?siteId={PublishmentSystemId}&formId={FormInfo.Id}&returnUrl={HttpUtility.UrlEncode(ReturnUrl)}");
            PageResultsUrl = Main.FilesApi.GetPluginUrl(
                $"{nameof(PageResults)}.aspx?siteId={PublishmentSystemId}&formId={FormInfo.Id}&returnUrl={HttpUtility.UrlEncode(ReturnUrl)}");
            PageFieldsUrl = Main.FilesApi.GetPluginUrl(
                $"{nameof(PageFields)}.aspx?siteId={PublishmentSystemId}&formId={FormInfo.Id}&returnUrl={HttpUtility.UrlEncode(ReturnUrl)}");
            PageSettingsUrl = Main.FilesApi.GetPluginUrl(
                $"{nameof(PageSettings)}.aspx?siteId={PublishmentSystemId}&formId={FormInfo.Id}&returnUrl={HttpUtility.UrlEncode(ReturnUrl)}");

            if (!Main.AdminApi.IsSiteAuthorized(PublishmentSystemId))
            {
                HttpContext.Current.Response.Write("<h1>未授权访问</h1>");
                HttpContext.Current.Response.End();
            }
        }

        public void LbTemplate_Click(object sender, EventArgs e)
        {
            CacheUtils.InsertMinutes("SiteServer.BackgroundPages.Cms.PageTemplatePreview", Main.DataApi.Encrypt(StlForm.GetDefaultStlFormStlElement(FormInfo)), 5);
            var url =
                Main.FilesApi.GetAdminDirectoryUrl(
                    $"cms/pageTemplatePreview.aspx?siteId={PublishmentSystemId}&fromCache={true}&returnUrl={Main.DataApi.Encrypt(PageLogsUrl)}");

            Response.Redirect(Main.FilesApi.GetAdminDirectoryUrl($"loading.aspx?redirectUrl={Main.DataApi.Encrypt(url)}"));
        }
    }
}
