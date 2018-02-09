using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
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
        public Button BtnAddInput;
        public Button BtnImport;

	    public PlaceHolder PhModalAdd;
        public Literal LtlModalAddTitle;
        public Literal LtlModalAddMessage;
        public TextBox TbTitle;
        public TextBox TbDescription;

        public PlaceHolder PhModalImport;
        public HtmlInputFile HifImport;

	    private string _apiUrl;
        private int _siteId;
        private int _formId;
	    private string _returnUrl;

        public static string GetRedirectUrl(string apiUrl, int siteId)
        {
            return Main.Instance.PluginApi.GetPluginUrl($"{nameof(PageManagement)}.aspx?apiUrl={HttpUtility.UrlEncode(apiUrl)}&siteId={siteId}");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            _apiUrl = HttpUtility.UrlDecode(Request.QueryString["apiUrl"]);
            _siteId = Convert.ToInt32(Request.QueryString["siteId"]);
            _formId = Convert.ToInt32(Request.QueryString["formId"]);
            _returnUrl = GetRedirectUrl(_apiUrl, _siteId);

            if (!Main.Instance.AdminApi.IsSiteAuthorized(_siteId))
            {
                Response.Write("<h1>未授权访问</h1>");
                Response.End();
                return;
            }

            if (_formId > 0)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["up"]) || !string.IsNullOrEmpty(Request.QueryString["down"]))
                {
                    if (string.IsNullOrEmpty(Request.QueryString["down"]))
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
                    Response.Redirect(Main.Instance.FilesApi.GetAdminDirectoryUrl($"cms/pageTemplatePreview.aspx?siteId={_siteId}&fromCache={true}&returnUrl={Main.Instance.DataApi.Encrypt(_returnUrl)}"));
                    return;
                }
            }

            if (IsPostBack) return;

            DgContents.DataSource = Main.Instance.FormDao.GetFormInfoListNotInChannel(_siteId);
            DgContents.ItemDataBound += DgContents_ItemDataBound;
            DgContents.DataBind();

            BtnAddInput.Attributes.Add("onclick", $"location.href = '{_returnUrl}&add={true}';return false;");
            BtnImport.Attributes.Add("onclick", $"location.href = '{_returnUrl}&import={true}';return false;");

            if (!string.IsNullOrEmpty(Request.QueryString["add"]))
            {
                PhModalAdd.Visible = true;
                if (_formId > 0)
                {
                    LtlModalAddTitle.Text = "编辑表单";

                    var formInfo = Main.Instance.FormDao.GetFormInfo(_formId);

                    TbTitle.Text = formInfo.Title;
                    TbDescription.Text = formInfo.Description;
                }
                else
                {
                    LtlModalAddTitle.Text = "添加表单";
                }
            }
            else if (!string.IsNullOrEmpty(Request.QueryString["import"]))
            {
                PhModalImport.Visible = true;
            }
            //else if (!string.IsNullOrEmpty(Request.QueryString["export"]))
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

        //public void Import_OnClick(object sender, EventArgs e)
        //{
        //    if (HifImport.PostedFile != null && "" != HifImport.PostedFile.FileName)
        //    {
        //        var filePath = HifImport.PostedFile.FileName;
        //        if (!Utils.EqualsIgnoreCase(Path.GetExtension(filePath), ".zip"))
        //        {
        //            LtlScript.Text = Utils.SwalError("导入失败", "表单导入失败，必须上传ZIP文件！");
        //            return;
        //        }

        //        try
        //        {
        //            var localFilePath = Main.Instance.Context.FilesApi.GetTemporaryFilesPath(Path.GetFileName(filePath));

        //            HifImport.PostedFile.SaveAs(localFilePath);

        //            Utils.ImportInput(_siteId, localFilePath);

        //            LtlScript.Text = Utils.SwalSuccess("导入成功", "恭喜，成功导入表单", "关 闭", $"location.href = '{GetRedirectUrl(_siteId)}'");
        //        }
        //        catch (Exception ex)
        //        {
        //            LtlScript.Text = Utils.SwalError("导入错误", ex.Message);
        //        }
        //    }
        //}

        public void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                FormInfo formInfo;

                if (!string.IsNullOrEmpty(Request.QueryString["formId"]))
                {
                    var formId = Convert.ToInt32(Request.QueryString["formId"]);
                    formInfo = Main.Instance.FormDao.GetFormInfo(formId);
                    if (formInfo != null)
                    {
                        if (formInfo.Title != TbTitle.Text)
                        {
                            if (Main.Instance.FormDao.IsTitleExists(_siteId, TbTitle.Text))
                            {
                                LtlModalAddMessage.Text = Utils.GetMessageHtml("表单修改失败，表单名称已存在！", false);

                                return;
                            }
                            formInfo.Title = TbTitle.Text;
                        }

                        formInfo.Description = TbDescription.Text;

                        Main.Instance.FormDao.Update(formInfo);
                    }
                }
                else
                {
                    if (Main.Instance.FormDao.IsTitleExists(_siteId, TbTitle.Text))
                    {
                        LtlModalAddMessage.Text = Utils.GetMessageHtml("表单添加失败，表单名称已存在！", false);
                        return;
                    }

                    formInfo = new FormInfo
                    {
                        Title = TbTitle.Text,
                        Description = TbDescription.Text,
                        SiteId = _siteId,
                        IsTimeout = false,
                        TimeToStart = DateTime.Now,
                        TimeToEnd = DateTime.Now.AddMonths(3)
                    };

                    Main.Instance.FormDao.Insert(formInfo);
                }

                Response.Redirect(_returnUrl);
            }
            catch (Exception ex)
            {
                LtlModalAddMessage.Text = Utils.GetMessageHtml($"提交失败：{ex.Message}！", false);
            }
        }
    }
}
