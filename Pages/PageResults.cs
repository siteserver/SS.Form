using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Model;

namespace SS.Form.Pages
{
    public class PageResults : PageBase
    {
        public Literal LtlCount;
        public Repeater RptFields;

        private List<FieldInfo> _fieldInfoList;
        private List<LogInfo> _logInfoList;
        private FieldInfo _fieldInfo;
        private int _logTotalCount;
        private int _totalCount;

        public void Page_Load(object sender, EventArgs e)
        {
            _fieldInfoList = Main.FieldDao.GetFieldInfoList(FormInfo.Id, true);
            _logInfoList = Main.LogDao.GetAllFormLogInfoList(FormInfo.Id);

            if (IsPostBack) return;

            RptFields.DataSource = _fieldInfoList;
            RptFields.ItemDataBound += RptFields_ItemDataBound;
            RptFields.DataBind();

            LtlCount.Text = $"总人数：{_logInfoList.Count}，总票数：{_totalCount}";
        }

        private void RptFields_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            _fieldInfo = (FieldInfo)e.Item.DataItem;

            if (_fieldInfo.Items == null || _fieldInfo.Items.Count == 0)
            {
                e.Item.Visible = false;
                return;
            }
            if (Utils.IsSelectFieldType(_fieldInfo.FieldType))
            {
                e.Item.Visible = false;
                return;
            }

            var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");
            var rptItems = (Repeater)e.Item.FindControl("rptItems");

            ltlTitle.Text = _fieldInfo.Title;

            _logTotalCount = GetLogTotalCount(_fieldInfo, _logInfoList);

            ltlTitle.Text += $"（总票数：{_logTotalCount}）";
            _totalCount += _logTotalCount;

            rptItems.DataSource = _fieldInfo.Items;
            rptItems.ItemDataBound += RptItems_ItemDataBound;
            rptItems.DataBind();
        }

        private static int GetLogTotalCount(FieldInfo fieldInfo, List<LogInfo> logInfoList)
        {
            var logTotalCount = 0;

            var itemValues = new List<string>();
            foreach (var item in fieldInfo.Items)
            {
                if (!string.IsNullOrEmpty(item.Value))
                {
                    itemValues.Add(item.Value);
                }
            }
            var isMultiple = fieldInfo.FieldType == InputType.CheckBox.Value ||
                             fieldInfo.FieldType == InputType.SelectMultiple.Value;

            
            foreach (var logInfo in logInfoList)
            {
                var value = logInfo.GetString(fieldInfo.Title);
                if (isMultiple)
                {
                    List<string> values = null;
                    try
                    {
                        values = JsonConvert.DeserializeObject<List<string>>(value);
                    }
                    catch
                    {
                        // ignored
                    }
                    if (values != null)
                    {
                        foreach (var itemValue in values)
                        {
                            if (!string.IsNullOrEmpty(itemValue) && itemValues.Contains(itemValue))
                            {
                                logTotalCount++;
                            }
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(value) && itemValues.Contains(value))
                    {
                        logTotalCount++;
                    }
                }
            }

            return logTotalCount;
        }

        private void RptItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var itemInfo = (FieldItemInfo)e.Item.DataItem;
            var isMultiple = _fieldInfo.FieldType == InputType.CheckBox.Value ||
                             _fieldInfo.FieldType == InputType.SelectMultiple.Value;

            var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");
            var ltlSummary = (Literal)e.Item.FindControl("ltlSummary");
            var ltlProgress = (Literal)e.Item.FindControl("ltlProgress");

            var itemCount = 0;
            foreach (var logInfo in _logInfoList)
            {
                var value = logInfo.GetString(_fieldInfo.Title);
                if (isMultiple)
                {
                    List<string> values = null;
                    try
                    {
                        values = JsonConvert.DeserializeObject<List<string>>(value);
                    }
                    catch
                    {
                        // ignored
                    }
                    if (values != null)
                    {
                        if (values.Contains(itemInfo.Value))
                        {
                            itemCount++;
                        }
                    }
                }
                else
                {
                    if (value == itemInfo.Value)
                    {
                        itemCount++;
                    }
                }
            }

            double percent;
            if (_logTotalCount == 0)
            {
                percent = 0;
            }
            else
            {
                var d = Convert.ToDouble(itemCount) / Convert.ToDouble(_logTotalCount) * 100;
                percent = Math.Round(d, 2);
            }

            ltlTitle.Text = itemInfo.Value;
            ltlSummary.Text = $"票数：{itemCount}， 占比：{percent}%";
            ltlProgress.Text = $@"
        <div class=""progress-bar progress-bar-primary"" role=""progressbar"" aria-valuenow=""60"" aria-valuemin=""0"" aria-valuemax=""100"" style=""width: {percent}%;"">
            <span class=""sr-only"">{percent}% Complete</span>
        </div>";
        }

        public void BtnExport_Click(object sender, EventArgs e)
        {
            var head = new List<string> {"标题", "序号", "选项", "票数", "占比"};

            var rows = new List<List<string>>();

            var index = 1;
            foreach (var fieldInfo in _fieldInfoList)
            {
                if (fieldInfo.Items == null || fieldInfo.Items.Count == 0) continue;

                if (Utils.IsSelectFieldType(fieldInfo.FieldType)) continue;

                rows.Add(new List<string>
                {
                    fieldInfo.Title,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty
                });

                var logTotalCount = GetLogTotalCount(fieldInfo, _logInfoList);

                var isMultiple = fieldInfo.FieldType == InputType.CheckBox.Value ||
                             fieldInfo.FieldType == InputType.SelectMultiple.Value;

                foreach (var itemInfo in fieldInfo.Items)
                {
                    var itemCount = 0;
                    foreach (var logInfo in _logInfoList)
                    {
                        var value = logInfo.GetString(fieldInfo.Title);
                        if (isMultiple)
                        {
                            List<string> values = null;
                            try
                            {
                                values = JsonConvert.DeserializeObject<List<string>>(value);
                            }
                            catch
                            {
                                // ignored
                            }
                            if (values != null)
                            {
                                if (values.Contains(itemInfo.Value))
                                {
                                    itemCount++;
                                }
                            }
                        }
                        else
                        {
                            if (value == itemInfo.Value)
                            {
                                itemCount++;
                            }
                        }
                    }

                    double percent;
                    if (logTotalCount == 0)
                    {
                        percent = 0;
                    }
                    else
                    {
                        var d = Convert.ToDouble(itemCount) / Convert.ToDouble(logTotalCount) * 100;
                        percent = Math.Round(d, 2);
                    }

                    rows.Add(new List<string>
                    {
                        string.Empty,
                        index++.ToString(),
                        itemInfo.Value,
                        itemCount.ToString(),
                        percent + "%"
                    });
                }
            }

            var relatedPath = "数据统计.csv";

            CsvUtils.Export(PluginContext.PluginApi.GetPluginPath(relatedPath), head, rows);

            HttpContext.Current.Response.Redirect(PluginContext.PluginApi.GetPluginUrl(relatedPath));
        }

        //        private void RptItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        //        {
        //            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

        //            var itemInfo = (ItemInfo)e.Item.DataItem;

        //            var ltlImage = (Literal)e.Item.FindControl("ltlImage");
        //            var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");
        //            var ltlSubTitle = (Literal)e.Item.FindControl("ltlSubTitle");
        //            var ltlSummary = (Literal)e.Item.FindControl("ltlSummary");
        //            var ltlProgress = (Literal)e.Item.FindControl("ltlProgress");

        //            double percent;
        //            if (_totalCount == 0)
        //            {
        //                percent = 0;
        //            }
        //            else
        //            {
        //                var d = Convert.ToDouble(itemInfo.Count) / Convert.ToDouble(_totalCount) * 100;
        //                percent = Math.Round(d, 2);
        //            }

        //            if (_pollInfo.IsImage)
        //            {
        //                ltlImage.Text = $@"<img src=""{itemInfo.ImageUrl}"" class=""img-responsive img-circle"" style=""height: 72px;width: 72px;float: left;"">";
        //            }
        //            ltlTitle.Text = itemInfo.Title;
        //            ltlSubTitle.Text = itemInfo.SubTitle;
        //            ltlSummary.Text = $"票数：{itemInfo.Count}， 占比：{percent}%";
        //            ltlProgress.Text = $@"
        //<div class=""progress-bar progress-bar-primary"" role=""progressbar"" aria-valuenow=""60"" aria-valuemin=""0"" aria-valuemax=""100"" style=""width: {percent}%;"">
        //    <span class=""sr-only"">{percent}% Complete</span>
        //</div>";
        //        }
    }
}
