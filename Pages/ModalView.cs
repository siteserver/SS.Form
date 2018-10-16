using System;
using System.Text;
using System.Web.UI.WebControls;
using SS.Form.Core;
using SS.Form.Model;

namespace SS.Form.Pages
{
    public class ModalView : PageBase
    {
        public Repeater RptContents;
        public Literal LtlAddDate;

        private LogInfo _logInfo;

        public static string GetOpenScript(int siteId, int formId, int logId)
        {
            return LayerUtils.GetOpenScript("查看", $"{nameof(ModalView)}.aspx?siteId={siteId}&formId={formId}&logId={logId}");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            _logInfo = Main.LogDao.GetLogInfo(Utils.ToInt(Request.QueryString["logId"]));

            if (IsPostBack) return;

            RptContents.DataSource = Main.FieldDao.GetFieldInfoList(FormInfo.Id, false);
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            LtlAddDate.Text = _logInfo.AddDate.ToString("yyyy-MM-dd HH:mm");
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var fieldInfo = (FieldInfo)e.Item.DataItem;

            var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");
            var ltlValue = (Literal)e.Item.FindControl("ltlValue");

            ltlTitle.Text = fieldInfo.Title;

            var values = _logInfo.GetString(fieldInfo.Title) ?? string.Empty;
            if (FieldManager.IsExtra(fieldInfo))
            {
                var builder = new StringBuilder();
                foreach (var item in fieldInfo.Items)
                {
                    var extras = _logInfo.GetString(FieldManager.GetExtrasId(fieldInfo.Id, item.Id));
                    if (!string.IsNullOrEmpty(extras))
                    {
                        builder.Append($@"<li class=""list-group-item""><label>{item.Value}：</label>{extras}</li>");
                    }
                }
                if (builder.Length > 0)
                {
                    values += $@"<ul class=""list-group m-t-10"">{builder}</ul>";
                }
            }
            ltlValue.Text = values;
        }
    }
}
