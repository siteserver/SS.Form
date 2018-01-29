using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SS.Form.Core;
using SS.Form.Model;

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

        public PlaceHolder PhModalAdd;
        public Literal LtlModalAddTitle;
        public Literal LtlModalAddMessage;
        public TextBox TbTitle;
        public TextBox TbDescription;
        public TextBox TbPlaceHolder;
        public DropDownList DdlFieldType;

        public PlaceHolder PhIsSelectField;
        public DropDownList DdlIsRapid;
        public PlaceHolder PhRapid;
        public TextBox TbRapidValues;
        public PlaceHolder PhItemCount;
        public TextBox TbItemCount;
        public Repeater RptItems;
        public PlaceHolder PhItems;

        public PlaceHolder PhModalValidate;
        public Literal LtlModalValidateMessage;
        public DropDownList DdlIsRequired;
        public PlaceHolder PhValidateType;
        public DropDownList DdlValidateType;
        public PlaceHolder PhNum;
        public TextBox TbMinNum;
        public TextBox TbMaxNum;

        public Literal LtlScript;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            //删除样式
            if (!string.IsNullOrEmpty(Request.QueryString["delete"]))
            {
                var fieldId = Convert.ToInt32(Request.QueryString["fieldId"]);
                Main.FieldDao.Delete(fieldId);
                LtlMessage.Text = Utils.GetMessageHtml("字段删除成功！", true);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["taxis"]))
            {
                var fieldId = Convert.ToInt32(Request.QueryString["fieldId"]);
                var direction = Request.QueryString["direction"];

                switch (direction.ToUpper())
                {
                    case "UP":
                        Main.FieldDao.TaxisUp(fieldId);
                        break;
                    case "DOWN":
                        Main.FieldDao.TaxisDown(fieldId);
                        break;
                }
                LtlMessage.Text = Utils.GetMessageHtml("排序成功！", true);
            }

            var fieldList = Main.FieldDao.GetFieldInfoList(FormInfo.Id, false);

            DgContents.DataSource = fieldList;
            DgContents.ItemDataBound += DgContents_ItemDataBound;
            DgContents.DataBind();

            //BtnReturn.Attributes.Add("onclick",
            //    $"location.href='{PageManagement.GetRedirectUrl(_siteId)}';return false;");
            BtnAddField.Attributes.Add("onclick",
                $"location.href = '{PageFieldsUrl}&addField={true}';return false;");

            if (!string.IsNullOrEmpty(Request.QueryString["addField"]))
            {
                PhModalAdd.Visible = true;
                var fieldId = Convert.ToInt32(Request.QueryString["fieldId"]);
                var fieldInfo = fieldId > 0 ? Main.FieldDao.GetFieldInfo(fieldId, true) : new FieldInfo();

                LtlModalAddTitle.Text = fieldId > 0 ? "编辑字段" : "新增字段";
                LtlScript.Text = @"<script>
setTimeout(function() {
    $('#modalAdd').modal();
}, 100);
</script>";

                FieldTypeUtils.AddListItems(DdlFieldType);

                DdlIsRapid.SelectedValue = fieldInfo.Id != 0 ? false.ToString() : true.ToString();

                TbTitle.Text = fieldInfo.Title;
                TbDescription.Text = fieldInfo.Description;
                TbPlaceHolder.Text = fieldInfo.PlaceHolder;
                Utils.SelectListItems(DdlFieldType, fieldInfo.FieldType);

                TbItemCount.Text = fieldInfo.Items.Count.ToString();

                RptItems.DataSource = GetDataSource(fieldInfo.Items.Count, fieldInfo.Items);
                RptItems.ItemDataBound += RptItems_ItemDataBound;
                RptItems.DataBind();

                var isSelected = false;
                var isExtras = false;
                var list = new List<string>();
                foreach (var item in fieldInfo.Items)
                {
                    list.Add(item.Value);
                    if (item.IsSelected)
                    {
                        isSelected = true;
                    }
                    if (item.IsExtras)
                    {
                        isExtras = true;
                    }
                }

                DdlIsRapid.SelectedValue = (!isSelected && !isExtras).ToString();
                TbRapidValues.Text = string.Join(",", list);

                ReFresh(null, EventArgs.Empty);
            }
            else if (!string.IsNullOrEmpty(Request.QueryString["validateField"]))
            {
                PhModalValidate.Visible = true;
                var fieldId = Convert.ToInt32(Request.QueryString["fieldId"]);
                var fieldInfo = Main.FieldDao.GetFieldInfo(fieldId, false);

                LtlScript.Text = @"<script>
setTimeout(function() {
    $('#modalValidate').modal();
}, 100);
</script>";

                var settings = new FieldSettings(fieldInfo.Settings);
                Utils.SelectListItems(DdlIsRequired, settings.IsRequired.ToString());

                if (Utils.EqualsIgnoreCase(fieldInfo.FieldType, nameof(FieldType.Text)) || Utils.EqualsIgnoreCase(fieldInfo.FieldType, nameof(FieldType.TextArea)))
                {
                    PhNum.Visible = PhValidateType.Visible = true;
                }
                else
                {
                    PhNum.Visible = PhValidateType.Visible = false;
                }

                TbMinNum.Text = settings.MinNum.ToString();
                TbMaxNum.Text = settings.MaxNum.ToString();

                ValidateTypeUtils.AddListItems(DdlValidateType);
                Utils.SelectListItems(DdlValidateType, settings.ValidateType.Value);
            }

            //var redirectUrl = GetRedirectUrl(PublishmentSystemId, _tableStyle, _tableName, _relatedIdentity, _itemId);

            //btnAddStyle.Attributes.Add("onclick", ModalTableStyleAdd.GetOpenWindowString(PublishmentSystemId, 0, _relatedIdentities, _tableName, string.Empty, _tableStyle, redirectUrl));
            //btnAddStyles.Attributes.Add("onclick", ModalTableStylesAdd.GetOpenWindowString(PublishmentSystemId, _relatedIdentities, _tableName, _tableStyle, redirectUrl));

            //btnImport.Attributes.Add("onclick", ModalTableStyleImport.GetOpenWindowString(_tableName, _tableStyle, PublishmentSystemId, _relatedIdentity));
            //btnExport.Attributes.Add("onclick", ModalExportMessage.GetOpenWindowStringToSingleTableStyle(_tableStyle, _tableName, PublishmentSystemId, _relatedIdentity));
        }

        private List<FieldItemInfo> GetDataSource(int count, List<FieldItemInfo> fieldInfoItems)
        {
            var items = new List<FieldItemInfo>();
            for (var i = 0; i < count; i++)
            {
                var itemInfo = fieldInfoItems != null && fieldInfoItems.Count > i ? fieldInfoItems[i] : new FieldItemInfo();
                items.Add(itemInfo);
            }
            return items;
        }

        private void RptItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var item = (FieldItemInfo) e.Item.DataItem;
            var fieldType = FieldTypeUtils.GetEnumType(DdlFieldType.SelectedValue);

            var cbIsSelected = (CheckBox) e.Item.FindControl("CbIsSelected");
            var cbIsExtras = (CheckBox)e.Item.FindControl("CbIsExtras");

            if (fieldType == FieldType.CheckBox || fieldType == FieldType.Radio)
            {
                cbIsExtras.Visible = true;
            }
            else
            {
                cbIsExtras.Visible = false;
            }

            cbIsSelected.Checked = item.IsSelected;
            cbIsExtras.Checked = item.IsExtras;
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
            ltlFieldType.Text = FieldTypeUtils.GetText(FieldTypeUtils.GetEnumType(fieldInfo.FieldType));

            ltlValidate.Text = ValidateTypeUtils.GetValidateInfo(settings.IsRequired,
                settings.MinNum, settings.MaxNum, settings.ValidateType);

            ltlActions.Text = $@"
<a class=""m-r-10"" href=""{PageFieldsUrl}&taxis={true}&direction=Up&fieldId={fieldInfo.Id}"">上升</a>
<a class=""m-r-10"" href=""{PageFieldsUrl}&taxis={true}&direction=Down&fieldId={fieldInfo.Id}"">下降</a>
<a class=""m-r-10"" href=""{PageFieldsUrl}&addField={true}&fieldId={fieldInfo.Id}"">编辑</a>
<a class=""m-r-10"" href=""{PageFieldsUrl}&validateField={true}&fieldId={fieldInfo.Id}"">验证规则</a>
<a class=""m-r-10"" href=""{PageFieldsUrl}&delete={true}&fieldId={fieldInfo.Id}"">删除</a>";
        }

        public void ReFresh(object sender, EventArgs e)
        {
            PhIsSelectField.Visible = PhItems.Visible = PhRapid.Visible = PhItemCount.Visible = false;

            var fieldType = DdlFieldType.SelectedValue;
            if (!Utils.IsSelectFieldType(fieldType)) return;

            PhIsSelectField.Visible = true;
            var isRapid = Convert.ToBoolean(DdlIsRapid.SelectedValue);
            if (isRapid)
            {
                PhRapid.Visible = true;
            }
            else
            {
                PhItemCount.Visible = true;
                SetCount_OnClick(sender, e);
            }
        }

        public void SetCount_OnClick(object sender, EventArgs e)
        {
            var count = Convert.ToInt32(TbItemCount.Text);
            if (count > 0)
            {
                PhItems.Visible = true;
                var fieldId = Convert.ToInt32(Request.QueryString["fieldId"]);

                List<FieldItemInfo> items = null;
                if (fieldId > 0)
                {
                    items = Main.FieldItemDao.GetItemInfoList(fieldId);
                }
                RptItems.DataSource = GetDataSource(count, items);
                RptItems.DataBind();
            }
            else
            {
                PhItems.Visible = false;
            }
        }

        public void Add_OnClick(object sender, EventArgs e)
        {
            var fieldType = DdlFieldType.SelectedValue;

            if (Utils.IsSelectFieldType(fieldType))
            {
                var isRapid = Convert.ToBoolean(DdlIsRapid.SelectedValue);
                if (!isRapid)
                {
                    var itemCount = Convert.ToInt32(TbItemCount.Text);
                    if (itemCount == 0)
                    {
                        LtlModalAddMessage.Text = Utils.GetMessageHtml("操作失败，选项数目必须大于0！", false);
                        return;
                    }
                }
            }

            var fieldId = Convert.ToInt32(Request.QueryString["fieldId"]);
            var fieldInfo = fieldId > 0
                ? Main.FieldDao.GetFieldInfo(fieldId, true)
                : new FieldInfo
                {
                    FormId = FormInfo.Id
                };
            fieldInfo.FieldType = fieldType;

            var isChanged = InsertUpdateFieldInfo(fieldInfo);

            if (isChanged)
            {
                Response.Redirect(PageFieldsUrl);
            }
        }

        private bool InsertUpdateFieldInfo(FieldInfo fieldInfo)
        {
            if (fieldInfo.Id == 0)
            {
                if (Main.FieldDao.IsTitleExists(FormInfo.Id, TbTitle.Text))
                {
                    LtlModalAddMessage.Text = Utils.GetMessageHtml($@"操作失败，字段名""{TbTitle.Text}""已存在", false);
                    return false;
                }
            }
            else
            {
                if (fieldInfo.Title != TbTitle.Text &&
                Main.FieldDao.IsTitleExists(FormInfo.Id, TbTitle.Text))
                {
                    LtlModalAddMessage.Text = Utils.GetMessageHtml($@"操作失败，字段名""{TbTitle.Text}""已存在", false);
                    return false;
                }
            }

            fieldInfo.Title = TbTitle.Text;
            fieldInfo.Description = TbDescription.Text;
            fieldInfo.PlaceHolder = TbPlaceHolder.Text;

            List<FieldItemInfo> fieldItems = null;

            if (Utils.IsSelectFieldType(fieldInfo.FieldType))
            {
                fieldItems = new List<FieldItemInfo>();

                var isRapid = Convert.ToBoolean(DdlIsRapid.SelectedValue);
                if (isRapid)
                {
                    var itemArray = TbRapidValues.Text.Split(',');
                    if (itemArray.Length > 0)
                    {
                        foreach (var itemValue in itemArray)
                        {
                            fieldItems.Add(new FieldItemInfo
                            {
                                Id = 0,
                                IsSelected = false,
                                Value = itemValue
                            });
                        }
                    }
                }
                else
                {
                    var isHasSelected = false;
                    foreach (RepeaterItem item in RptItems.Items)
                    {
                        var tbValue = (TextBox) item.FindControl("TbValue");
                        var cbIsSelected = (CheckBox) item.FindControl("CbIsSelected");
                        var cbIsExtras = (CheckBox)item.FindControl("CbIsExtras");

                        if ((Utils.EqualsIgnoreCase(fieldInfo.FieldType, nameof(FieldType.Radio)) ||
                             Utils.EqualsIgnoreCase(fieldInfo.FieldType, nameof(FieldType.SelectOne))) && isHasSelected &&
                            cbIsSelected.Checked)
                        {
                            LtlModalAddMessage.Text = Utils.GetMessageHtml("操作失败，只能有一个初始化时选定项！", false);
                            return false;
                        }
                        if (cbIsSelected.Checked) isHasSelected = true;

                        fieldItems.Add(new FieldItemInfo
                        {
                            Id = 0,
                            Value = tbValue.Text,
                            IsSelected = cbIsSelected.Checked,
                            IsExtras = cbIsExtras.Checked
                        });
                    }
                }
            }

            try
            {
                if (fieldInfo.Id > 0)
                {
                    Main.FieldDao.Update(fieldInfo);
                    Main.FieldItemDao.DeleteByFieldId(fieldInfo.Id);
                }
                else
                {
                    fieldInfo.Id = Main.FieldDao.Insert(fieldInfo);
                }
                
                Main.FieldItemDao.InsertItems(FormInfo.Id, fieldInfo.Id, fieldItems);

                return true;
            }
            catch (Exception ex)
            {
                LtlModalAddMessage.Text = Utils.GetMessageHtml($"操作失败，：{ex.Message}！", false);
                return false;
            }
        }

        public void BtnValidate_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            var fieldId = Convert.ToInt32(Request.QueryString["fieldId"]);
            var fieldInfo = Main.FieldDao.GetFieldInfo(fieldId, false);
            var settings = new FieldSettings(fieldInfo.Settings)
            {
                IsRequired = Convert.ToBoolean(DdlIsRequired.SelectedValue),
                MinNum = Convert.ToInt32(TbMinNum.Text),
                MaxNum = Convert.ToInt32(TbMaxNum.Text),
                ValidateType = ValidateTypeUtils.GetEnumType(DdlValidateType.SelectedValue)
            };

            fieldInfo.Settings = settings.ToString();

            try
            {
                Main.FieldDao.Update(fieldInfo);
                isChanged = true;
            }
            catch (Exception ex)
            {
                LtlModalValidateMessage.Text = Utils.GetMessageHtml($"设置表单验证失败：{ex.Message}", false);
            }

            if (isChanged)
            {
                Response.Redirect(PageFieldsUrl);
            }
        }
    }
}
