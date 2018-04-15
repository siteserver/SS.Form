using System;
using System.Web.UI.WebControls;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Model;

namespace SS.Form.Pages
{
    public class ModalFieldValidate : PageBase
    {
        public Literal LtlMessage;

        public DropDownList DdlIsRequired;
        public PlaceHolder PhValidateType;
        public DropDownList DdlValidateType;
        public PlaceHolder PhNum;
        public TextBox TbMinNum;
        public TextBox TbMaxNum;

        public static string GetOpenScript(int siteId, int formId, int fieldId)
        {
            return LayerUtils.GetOpenScript("设置验证规则", $"{nameof(ModalFieldValidate)}.aspx?siteId={siteId}&formId={formId}&fieldId={fieldId}");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            var fieldId = Utils.ToInt(Request.QueryString["fieldId"]);
            var fieldInfo = Main.Instance.FieldDao.GetFieldInfo(fieldId, false);

            var settings = new FieldSettings(fieldInfo.Settings);
            Utils.SelectSingleItem(DdlIsRequired, settings.IsRequired.ToString());

            if (Utils.EqualsIgnoreCase(fieldInfo.FieldType, InputType.Text.Value) || Utils.EqualsIgnoreCase(fieldInfo.FieldType, InputType.TextArea.Value))
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
            Utils.SelectSingleItem(DdlValidateType, settings.ValidateType.Value);
        }

        public void BtnValidate_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            var fieldId = Utils.ToInt(Request.QueryString["fieldId"]);
            var fieldInfo = Main.Instance.FieldDao.GetFieldInfo(fieldId, false);
            var settings = new FieldSettings(fieldInfo.Settings)
            {
                IsRequired = Convert.ToBoolean(DdlIsRequired.SelectedValue),
                MinNum = Utils.ToInt(TbMinNum.Text),
                MaxNum = Utils.ToInt(TbMaxNum.Text),
                ValidateType = ValidateTypeUtils.GetEnumType(DdlValidateType.SelectedValue)
            };

            fieldInfo.Settings = settings.ToString();

            try
            {
                Main.Instance.FieldDao.Update(fieldInfo);
                isChanged = true;
            }
            catch (Exception ex)
            {
                LtlMessage.Text = Utils.GetMessageHtml($"设置表单验证失败：{ex.Message}", false);
            }

            if (isChanged)
            {
                LayerUtils.Close(Page);
            }
        }
    }
}
