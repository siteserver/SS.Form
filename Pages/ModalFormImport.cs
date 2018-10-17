using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SS.Form.Core;

namespace SS.Form.Pages
{
	public class ModalFormImport : Page
	{
	    public Literal LtlMessage;
        public HtmlInputFile HifImport;

        private int _siteId;

        public static string GetOpenScript(int siteId)
        {
            return LayerUtils.GetOpenScript("导入表单", $"{nameof(ModalFormImport)}.aspx?siteId={siteId}", 400, 320);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            _siteId = Utils.ToInt(Request.QueryString["siteId"]);

            if (!SiteServer.Plugin.Context.Request.AdminPermissions.HasSitePermissions(_siteId, Main.PluginId))
            {
                Response.Write("<h1>未授权访问</h1>");
                Response.End();
            }
        }

        public void Submit_OnClick(object sender, EventArgs e)
        {
            if (HifImport.PostedFile == null || "" == HifImport.PostedFile.FileName) return;

            var filePath = HifImport.PostedFile.FileName;
            if (!Utils.EqualsIgnoreCase(Path.GetExtension(filePath), ".zip"))
            {
                LtlMessage.Text = Utils.GetMessageHtml("表单导入失败，必须上传ZIP文件！", false);
                return;
            }

            try
            {
                var localFilePath = SiteServer.Plugin.Context.UtilsApi.GetTemporaryFilesPath(Path.GetFileName(filePath));

                HifImport.PostedFile.SaveAs(localFilePath);

                //Utils.ImportInput(_siteId, localFilePath);

                LayerUtils.Close(Page);
            }
            catch (Exception ex)
            {
                LtlMessage.Text = Utils.GetMessageHtml($"导入错误：{ex.Message}", false);
            }
        }
    }
}
