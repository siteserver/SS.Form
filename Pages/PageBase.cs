using System;
using System.Web;
using System.Web.UI;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Model;
using SS.Form.Parse;

namespace SS.Form.Pages
{
    public class PageBase : Page
    {
        protected int SiteId { get; private set; }
        protected FormInfo FormInfo { get; private set; }
        protected string ReturnUrl { get; private set; }
        protected string FormTitle { get; private set; }

        public string PageLogsUrl { get; private set; }

        public string PageResultsUrl { get; private set; }

        public string PageFieldsUrl { get; private set; }

        public string PageSettingsUrl { get; private set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SiteId = Utils.ToInt(Request.QueryString["siteId"]);
            var channelId = Utils.ToInt(Request.QueryString["channelId"]);
            var contentId = Utils.ToInt(Request.QueryString["contentId"]);
            var formId = Utils.ToInt(Request.QueryString["formId"]);
            FormInfo = formId > 0 ? Main.FormDao.GetFormInfo(formId) : Main.FormDao.GetFormInfoOrCreateIfNotExists(SiteId, channelId, contentId);
            ReturnUrl = HttpUtility.UrlDecode(Request.QueryString["returnUrl"]);
            FormTitle = FormInfo.Title;

            PageLogsUrl = $"{nameof(PageLogs)}.aspx?siteId={SiteId}&formId={FormInfo.Id}&returnUrl={HttpUtility.UrlEncode(ReturnUrl)}";
            PageResultsUrl = $"{nameof(PageResults)}.aspx?siteId={SiteId}&formId={FormInfo.Id}&returnUrl={HttpUtility.UrlEncode(ReturnUrl)}";
            PageFieldsUrl = $"{nameof(PageFields)}.aspx?siteId={SiteId}&formId={FormInfo.Id}&returnUrl={HttpUtility.UrlEncode(ReturnUrl)}";
            PageSettingsUrl = $"{nameof(PageSettings)}.aspx?siteId={SiteId}&formId={FormInfo.Id}&returnUrl={HttpUtility.UrlEncode(ReturnUrl)}";

            if (!SiteServer.Plugin.Context.Request.AdminPermissions.HasSitePermissions(SiteId, Main.PluginId))
            {
                HttpContext.Current.Response.Write("<h1>未授权访问</h1>");
                HttpContext.Current.Response.End();
            }
        }

        public void LbTemplate_Click(object sender, EventArgs e)
        {
            CacheUtils.InsertMinutes("SiteServer.BackgroundPages.Cms.PageTemplatePreview", SiteServer.Plugin.Context.UtilsApi.Encrypt(ParseUtils.GetFormStlElement(FormInfo)), 5);
            var url =
                SiteServer.Plugin.Context.UtilsApi.GetAdminDirectoryUrl(
                    $"cms/pageTemplatePreview.aspx?siteId={SiteId}&fromCache={true}&returnUrl={SiteServer.Plugin.Context.UtilsApi.Encrypt(SiteServer.Plugin.Context.PluginApi.GetPluginUrl(PageLogsUrl))}");

            Response.Redirect(SiteServer.Plugin.Context.UtilsApi.GetAdminDirectoryUrl($"loading.aspx?redirectUrl={SiteServer.Plugin.Context.UtilsApi.Encrypt(url)}"));
        }
    }
}
