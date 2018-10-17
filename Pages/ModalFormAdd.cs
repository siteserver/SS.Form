using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SS.Form.Core;
using SS.Form.Model;

namespace SS.Form.Pages
{
	public class ModalFormAdd : Page
	{
	    public Literal LtlMessage;

        public TextBox TbTitle;
        public TextBox TbDescription;

        private int _siteId;

        public static string GetOpenScript(int siteId)
        {
            return LayerUtils.GetOpenScript("添加表单", $"{nameof(ModalFormAdd)}.aspx?siteId={siteId}", 400, 320);
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
            try
            {
                if (Main.FormDao.IsTitleExists(_siteId, TbTitle.Text))
                {
                    LtlMessage.Text = Utils.GetMessageHtml("表单添加失败，表单名称已存在！", false);
                    return;
                }

                var formInfo = new FormInfo
                {
                    Title = TbTitle.Text,
                    Description = TbDescription.Text,
                    SiteId = _siteId,
                    IsTimeout = false,
                    TimeToStart = DateTime.Now,
                    TimeToEnd = DateTime.Now.AddMonths(3)
                };

                Main.FormDao.Insert(formInfo);

                LayerUtils.Close(Page);
            }
            catch (Exception ex)
            {
                LtlMessage.Text = Utils.GetMessageHtml($"提交失败：{ex.Message}！", false);
            }
        }
    }
}
