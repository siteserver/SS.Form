using System;
using System.Web.UI.WebControls;
using SS.Form.Core;
using SS.Form.Model;
using SS.Form.Provider;

namespace SS.Form.Pages
{
    public class PageFields : PageBase
    {
        public Literal LtlMessage;
        public DataGrid DgContents;
        public Button BtnAddField;
        public Button BtnAddFields;
        public Button BtnImport;
        public Button BtnExport;
        public Button BtnReturn;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            //删除样式
            if (!string.IsNullOrEmpty(Request.QueryString["delete"]))
            {
                var fieldId = Utils.ToInt(Request.QueryString["fieldId"]);
                FieldDao.Delete(fieldId);
                LtlMessage.Text = Utils.GetMessageHtml("字段删除成功！", true);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["taxis"]))
            {
                var fieldId = Utils.ToInt(Request.QueryString["fieldId"]);
                var direction = Request.QueryString["direction"];

                switch (direction.ToUpper())
                {
                    case "UP":
                        FieldDao.TaxisUp(fieldId);
                        break;
                    case "DOWN":
                        FieldDao.TaxisDown(fieldId);
                        break;
                }
                LtlMessage.Text = Utils.GetMessageHtml("排序成功！", true);
            }

            var fieldList = FieldDao.GetFieldInfoList(FormInfo.Id, false);

            DgContents.DataSource = fieldList;
            DgContents.ItemDataBound += DgContents_ItemDataBound;
            DgContents.DataBind();

            //BtnReturn.Attributes.Add("onclick",
            //    $"location.href='{PageManagement.GetRedirectUrl(_siteId)}';return false;");

            BtnAddField.Attributes.Add("onclick", ModalFieldAdd.GetOpenScript(SiteId, FormInfo.Id, 0));

            //var redirectUrl = GetRedirectUrl(SiteId, _tableStyle, _tableName, _relatedIdentity, _itemId);

            //btnAddStyle.Attributes.Add("onclick", ModalTableStyleAdd.GetOpenWindowString(SiteId, 0, _relatedIdentities, _tableName, string.Empty, _tableStyle, redirectUrl));
            //btnAddStyles.Attributes.Add("onclick", ModalTableStylesAdd.GetOpenWindowString(SiteId, _relatedIdentities, _tableName, _tableStyle, redirectUrl));

            //btnImport.Attributes.Add("onclick", ModalTableStyleImport.GetOpenWindowString(_tableName, _tableStyle, SiteId, _relatedIdentity));
            //btnExport.Attributes.Add("onclick", ModalExportMessage.GetOpenWindowStringToSingleTableStyle(_tableStyle, _tableName, SiteId, _relatedIdentity));
        }

        private void DgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var fieldInfo = (FieldInfo) e.Item.DataItem;
            var settings = new FieldSettings(fieldInfo.Settings);

            var ltlTitle = (Literal) e.Item.FindControl("ltlTitle");
            var ltlDescription = (Literal)e.Item.FindControl("ltlDescription");
            var ltlFieldType = (Literal) e.Item.FindControl("ltlFieldType");
            var ltlValidate = (Literal) e.Item.FindControl("ltlValidate");
            var ltlActions = (Literal) e.Item.FindControl("ltlActions");

            ltlTitle.Text = fieldInfo.Title;
            ltlDescription.Text = fieldInfo.Description;
            ltlFieldType.Text = Utils.GetFieldTypeText(fieldInfo.FieldType);

            ltlValidate.Text = ValidateTypeUtils.GetValidateInfo(settings.IsRequired,
                settings.MinNum, settings.MaxNum, settings.ValidateType);

            ltlActions.Text = $@"
<a class=""m-r-10"" href=""{PageFieldsUrl}&taxis={true}&direction=Up&fieldId={fieldInfo.Id}"">上升</a>
<a class=""m-r-10"" href=""{PageFieldsUrl}&taxis={true}&direction=Down&fieldId={fieldInfo.Id}"">下降</a>
<a class=""m-r-10"" href=""javascript:;"" onclick=""{ModalFieldAdd.GetOpenScript(SiteId, FormInfo.Id, fieldInfo.Id)}"">编辑</a>
<a class=""m-r-10"" href=""javascript:;"" onclick=""{ModalFieldValidate.GetOpenScript(SiteId, FormInfo.Id, fieldInfo.Id)}"">验证规则</a>
<a class=""m-r-10"" href=""{PageFieldsUrl}&delete={true}&fieldId={fieldInfo.Id}"">删除</a>";
        }
    }
}
