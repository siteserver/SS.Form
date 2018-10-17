using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Model;
using SS.Form.Provider;

namespace SS.Form.Pages
{
    public class PageLogs : PageBase
    {
        public Literal LtlMessage;
        public Literal LtlFieldNames;
        public Repeater RptLogs;
        public Button BtnExport;
        public Button BtnSettings;

        private List<FieldInfo> _fieldInfoList;

        public static string GetRedirectUrl(int siteId, int channelId, int contentId, string returnUrl)
        {
            return $"{nameof(PageLogs)}.aspx?siteId={siteId}&channelId={channelId}&contentId={contentId}&returnUrl={HttpUtility.UrlEncode(returnUrl)}";
        }

        public static string GetRedirectUrl(int siteId, int formId, string returnUrl)
        {
            return $"{nameof(PageLogs)}.aspx?siteId={siteId}&formId={formId}&returnUrl={HttpUtility.UrlEncode(returnUrl)}";
        }

        public void Page_Load(object sender, EventArgs e)
        {
            _fieldInfoList = FieldDao.GetFieldInfoList(FormInfo.Id, true);

            if (!string.IsNullOrEmpty(Request.QueryString["delete"]) &&
                !string.IsNullOrEmpty(Request.QueryString["logId"]))
            {
                var logId = Utils.ToInt(Request.QueryString["logId"]);
                LogDao.Delete(logId);
                LtlMessage.Text = Utils.GetMessageHtml("删除成功！", true);
            }

            if (IsPostBack) return;

            foreach (var fieldInfo in _fieldInfoList)
            {
                var settings = new FieldSettings(fieldInfo.Settings);
                if (!settings.IsVisibleInList) continue;

                LtlFieldNames.Text += $@"<th scope=""col"">{fieldInfo.Title}</th>";
            }

            var totalCount = LogDao.GetCount(FormInfo.Id);
            var logs = LogDao.GetFormLogInfoList(FormInfo.Id, totalCount, 30, 0);

            RptLogs.DataSource = logs;
            RptLogs.ItemDataBound += RptLogs_ItemDataBound;
            RptLogs.DataBind();

            BtnSettings.Attributes.Add("onclick", ModalSelectColumns.GetOpenScript(SiteId, FormInfo.Id));
        }

        public void BtnExport_Click(object sender, EventArgs e)
        {
            var logs = LogDao.GetAllFormLogInfoList(FormInfo.Id);

            var head = new List<string> { "序号"};
            foreach (var fieldInfo in _fieldInfoList)
            {
                head.Add(fieldInfo.Title);
            }
            head.Add("提交时间");

            var rows = new List<List<string>>();

            var index = 1;
            foreach (var log in logs)
            {
                var row = new List<string>
                {
                    index++.ToString()
                };
                foreach (var fieldInfo in _fieldInfoList)
                {
                    row.Add(log.GetString(fieldInfo.Title));
                }
                row.Add(log.AddDate.ToString("yyyy-MM-dd HH:mm"));

                rows.Add(row);
            }

            var relatedPath = "表单清单.csv";

            CsvUtils.Export(SiteServer.Plugin.Context.PluginApi.GetPluginPath(Main.PluginId, relatedPath), head, rows);

            HttpContext.Current.Response.Redirect(SiteServer.Plugin.Context.PluginApi.GetPluginUrl(Main.PluginId, relatedPath));
        }

        private void RptLogs_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var logInfo = (LogInfo)e.Item.DataItem;

            var ltlValues = (Literal)e.Item.FindControl("ltlValues");
            var ltlAddDate = (Literal)e.Item.FindControl("ltlAddDate");
            var ltlOperation = (Literal)e.Item.FindControl("ltlOperation");

            foreach (var fieldInfo in _fieldInfoList)
            {
                var settings = new FieldSettings(fieldInfo.Settings);
                if (!settings.IsVisibleInList) continue;
                
                var values = logInfo.GetString(fieldInfo.Title) ?? string.Empty;
                if (FieldManager.IsExtra(fieldInfo))
                {
                    var builder = new StringBuilder();
                    foreach (var item in fieldInfo.Items)
                    {
                        var extras = logInfo.GetString(FieldManager.GetExtrasId(fieldInfo.Id, item.Id));
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
                ltlValues.Text += $@"<td style=""min-width: 120px;"">{values}</td>";
            }

            ltlAddDate.Text = logInfo.AddDate.ToString("yyyy-MM-dd HH:mm");

            ltlOperation.Text =
                $@"
<a href=""javascript:;"" onclick=""{ModalView.GetOpenScript(SiteId, FormInfo.Id, logInfo.Id)}"">查看</a>
<a class=""m-l-10"" href=""javascript:;"" onclick=""{AlertUtils.Warning("删除项目", "本操作将删除此项，确定吗？", "取 消", "删 除", $"location.href='{GetRedirectUrl(SiteId, FormInfo.Id, ReturnUrl)}&delete={true}&logId={logInfo.Id}'")};return false;"">删除</a>";
        }
    }
}
