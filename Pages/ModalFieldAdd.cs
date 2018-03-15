using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SS.Form.Core;
using SS.Form.Model;

namespace SS.Form.Pages
{
    public class ModalFieldAdd : PageBase
    {
        public Literal LtlMessage;

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

        public static string GetOpenScript(int siteId, int formId, int fieldId)
        {
            return LayerUtils.GetOpenScript(fieldId > 0 ? "编辑字段" : "新增字段", $"{nameof(ModalFieldAdd)}.aspx?siteId={siteId}&formId={formId}&fieldId={fieldId}");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            var fieldId = Utils.ToInt(Request.QueryString["fieldId"]);
            var fieldInfo = fieldId > 0 ? Main.Instance.FieldDao.GetFieldInfo(fieldId, true) : new FieldInfo();

            FieldTypeUtils.AddListItems(DdlFieldType);

            DdlIsRapid.SelectedValue = fieldInfo.Id != 0 ? false.ToString() : true.ToString();

            TbTitle.Text = fieldInfo.Title;
            TbDescription.Text = fieldInfo.Description;
            TbPlaceHolder.Text = fieldInfo.PlaceHolder;
            Utils.SelectSingleItem(DdlFieldType, fieldInfo.FieldType);

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
            var count = Utils.ToInt(TbItemCount.Text);
            if (count > 0)
            {
                PhItems.Visible = true;
                var fieldId = Utils.ToInt(Request.QueryString["fieldId"]);

                List<FieldItemInfo> items = null;
                if (fieldId > 0)
                {
                    items = Main.Instance.FieldItemDao.GetItemInfoList(fieldId);
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
                    var itemCount = Utils.ToInt(TbItemCount.Text);
                    if (itemCount == 0)
                    {
                        LtlMessage.Text = Utils.GetMessageHtml("操作失败，选项数目必须大于0！", false);
                        return;
                    }
                }
            }

            var fieldId = Utils.ToInt(Request.QueryString["fieldId"]);
            var fieldInfo = fieldId > 0
                ? Main.Instance.FieldDao.GetFieldInfo(fieldId, true)
                : new FieldInfo
                {
                    FormId = FormInfo.Id
                };
            fieldInfo.FieldType = fieldType;

            var isChanged = InsertUpdateFieldInfo(fieldInfo);

            if (isChanged)
            {
                LayerUtils.Close(Page);
            }
        }

        private bool InsertUpdateFieldInfo(FieldInfo fieldInfo)
        {
            if (fieldInfo.Id == 0)
            {
                if (Main.Instance.FieldDao.IsTitleExists(FormInfo.Id, TbTitle.Text))
                {
                    LtlMessage.Text = Utils.GetMessageHtml($@"操作失败，字段名""{TbTitle.Text}""已存在", false);
                    return false;
                }
            }
            else
            {
                if (fieldInfo.Title != TbTitle.Text &&
                Main.Instance.FieldDao.IsTitleExists(FormInfo.Id, TbTitle.Text))
                {
                    LtlMessage.Text = Utils.GetMessageHtml($@"操作失败，字段名""{TbTitle.Text}""已存在", false);
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
                            LtlMessage.Text = Utils.GetMessageHtml("操作失败，只能有一个初始化时选定项！", false);
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
                    Main.Instance.FieldDao.Update(fieldInfo);
                    Main.Instance.FieldItemDao.DeleteByFieldId(fieldInfo.Id);
                }
                else
                {
                    fieldInfo.Id = Main.Instance.FieldDao.Insert(fieldInfo);
                }

                Main.Instance.FieldItemDao.InsertItems(FormInfo.Id, fieldInfo.Id, fieldItems);

                return true;
            }
            catch (Exception ex)
            {
                LtlMessage.Text = Utils.GetMessageHtml($"操作失败，：{ex.Message}！", false);
                return false;
            }
        }
    }
}
