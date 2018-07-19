using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SS.Form.Core;
using SS.Form.Model;
using SS.Form.Parse;

namespace SS.Form.Pages
{
	public class PageManagement : Page
	{
	    public Literal LtlMessage;
        public DataGrid DgContents;
        public Button BtnAdd;
        public Button BtnImport;

        private int _siteId;
        private int _formId;
        private string _returnUrl;

        public static string GetRedirectUrl(int siteId)
        {
            return $"{nameof(PageManagement)}.aspx?siteId={siteId}";
        }

        public void Page_Load(object sender, EventArgs e)
        {
            _siteId = Utils.ToInt(Request.QueryString["siteId"]);
            _formId = Utils.ToInt(Request.QueryString["formId"]);
            _returnUrl = GetRedirectUrl(_siteId);

            if (!Main.Instance.AdminApi.HasSitePermissions(_siteId, Main.Instance.Id))
            {
                Response.Write("<h1>未授权访问</h1>");
                Response.End();
                return;
            }

            if (_formId > 0)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["up"]) || !string.IsNullOrEmpty(Request.QueryString["down"]))
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["down"]))
                    {
                        Main.Instance.FormDao.UpdateTaxisToDown(_siteId, _formId);
                    }
                    else
                    {
                        Main.Instance.FormDao.UpdateTaxisToUp(_siteId, _formId);
                    }
                }
                if (!string.IsNullOrEmpty(Request.QueryString["delete"]))
                {
                    Main.Instance.FormDao.Delete(_formId);

                    LtlMessage.Text = Utils.GetMessageHtml("表单删除成功！", true);
                }
                if (!string.IsNullOrEmpty(Request.QueryString["template"]))
                {
                    var formInfo = Main.Instance.FormDao.GetFormInfo(_formId);
                    CacheUtils.InsertMinutes("SiteServer.BackgroundPages.Cms.PageTemplatePreview",
                        Main.Instance.DataApi.Encrypt(ParseUtils.GetFormStlElement(formInfo)),
                        5);
                    Response.Redirect(Main.Instance.FilesApi.GetAdminDirectoryUrl($"cms/pageTemplatePreview.aspx?siteId={_siteId}&fromCache={true}&returnUrl={Main.Instance.DataApi.Encrypt(Main.Instance.PluginApi.GetPluginUrl(_returnUrl))}"));
                    return;
                }
            }

            if (IsPostBack) return;

            DgContents.DataSource = Main.Instance.FormDao.GetFormInfoListNotInChannel(_siteId);
            DgContents.ItemDataBound += DgContents_ItemDataBound;
            DgContents.DataBind();

            BtnAdd.Attributes.Add("onclick", ModalFormAdd.GetOpenScript(_siteId));
            BtnImport.Attributes.Add("onclick", ModalFormImport.GetOpenScript(_siteId));

            //if (!string.IsNullOrEmpty(Request.QueryString["export"]))
            //{
            //    string fileName;
            //    if (Utils.ExportInput(_formId, out fileName))
            //    {
            //        LtlScript.Text = Utils.SwalSuccess("导出成功", "点击按钮下载导出文件", "下 载", $"location.href = '{Main.Instance.Context.FilesApi.GetRootUrl($"sitefiles/temporaryfiles/{fileName}")}'");
            //    }
            //}
        }

        private void DgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var formInfo = (FormInfo) e.Item.DataItem;

            var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");
            var ltlDescription = (Literal)e.Item.FindControl("ltlDescription");
            var ltlActions = (Literal)e.Item.FindControl("ltlActions");

            ltlTitle.Text = $@"<a href=""{PageLogs.GetRedirectUrl(_siteId, formInfo.Id, _returnUrl)}"">{formInfo.Title}</a>";
            ltlDescription.Text = formInfo.Description;

            ltlActions.Text = $@"
<a class=""m-r-10"" href=""{_returnUrl}&formId={formInfo.Id}&up={true}"">上升</a>
<a class=""m-r-10"" href=""{_returnUrl}&formId={formInfo.Id}&down={true}"">下降</a>
<a class=""m-r-10"" href=""{_returnUrl}&formId={formInfo.Id}&export={true}"">导出</a>
<a class=""m-r-10"" href=""{_returnUrl}&formId={formInfo.Id}&delete={true}"" onClick=""javascript:return confirm('此操作将删除表单“{formInfo.Title}”及相关数据，确认吗？');"">删除</a>
";
        }
    }
}
