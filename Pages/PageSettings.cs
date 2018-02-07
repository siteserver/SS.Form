using System;
using System.Web;
using System.Web.UI.WebControls;
using SS.Form.Controls;
using SS.Form.Core;
using SS.Form.Model;
using SS.Form.Parse;

namespace SS.Form.Pages
{
    public class PageSettings : PageBase
    {
        public Literal LtlMessage;

        public TextBox TbTitle;
        public TextBox TbDescription;
        public DropDownList DdlDefaultTheme;
        public CheckBox CbIsTimeout;
        public PlaceHolder PhTimeout;
        public DateTimeTextBox TbTimeToStart;
        public DateTimeTextBox TbTimeToEnd;
        public CheckBox CbIsCaptcha;
        public DropDownList DdlIsAdministratorSmsNotify;
        public PlaceHolder PhIsAdministratorSmsNotify;
        public TextBox TbAdministratorSmsNotifyTplId;
        public ListBox LbAdministratorSmsNotifyKeys;
        public TextBox TbAdministratorSmsNotifyMobile;

        public Literal LtlScript;

        public static string GetRedirectUrl(int siteId, int formId, string returnUrl)
        {
            return
                Main.Instance.PluginApi.GetPluginUrl(
                    $"{nameof(PageSettings)}.aspx?siteId={siteId}&formId={formId}&returnUrl={HttpUtility.UrlEncode(returnUrl)}");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            var settings = new FormSettings(FormInfo.Settings);

            TbTitle.Text = FormInfo.Title;
            TbDescription.Text = FormInfo.Description;

            var themeList = ParseUtils.GetThemeList();
            foreach (var theme in themeList)
            {
                DdlDefaultTheme.Items.Add(new ListItem(theme, theme));
            }
            Utils.SelectSingleItem(DdlDefaultTheme, settings.DefaultTheme);

            CbIsTimeout.Checked = FormInfo.IsTimeout;
            PhTimeout.Visible = FormInfo.IsTimeout;
            TbTimeToStart.DateTime = FormInfo.TimeToStart;
            TbTimeToEnd.DateTime = FormInfo.TimeToEnd;

            CbIsCaptcha.Checked = settings.IsCaptcha;

            Utils.SelectSingleItem(DdlIsAdministratorSmsNotify, settings.IsAdministratorSmsNotify.ToString());
            TbAdministratorSmsNotifyTplId.Text = settings.AdministratorSmsNotifyTplId;

            var keys = settings.AdministratorSmsNotifyKeys.Split(',');
            LbAdministratorSmsNotifyKeys.Items.Add(new ListItem(nameof(LogInfo.Id), nameof(LogInfo.Id)));
            LbAdministratorSmsNotifyKeys.Items.Add(new ListItem(nameof(LogInfo.AddDate), nameof(LogInfo.AddDate)));
            var fieldInfoList = Main.Instance.FieldDao.GetFieldInfoList(FormInfo.Id, false);
            foreach (var fieldInfo in fieldInfoList)
            {
                LbAdministratorSmsNotifyKeys.Items.Add(new ListItem(fieldInfo.Title, fieldInfo.Title));
            }
            Utils.SelectMultiItems(LbAdministratorSmsNotifyKeys, keys);

            TbAdministratorSmsNotifyMobile.Text = settings.AdministratorSmsNotifyMobile;

            PhIsAdministratorSmsNotify.Visible = Convert.ToBoolean(DdlIsAdministratorSmsNotify.SelectedValue);
        }

        public void CbIsTimeout_CheckedChanged(object sender, EventArgs e)
        {
            PhTimeout.Visible = CbIsTimeout.Checked;
        }

        public void DdlIsAdministratorSmsNotify_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhIsAdministratorSmsNotify.Visible = Convert.ToBoolean(DdlIsAdministratorSmsNotify.SelectedValue);
        }

        public void BtnSubmit_Click(object sender, EventArgs e)
        {
            var settings = new FormSettings(FormInfo.Settings)
            {
                DefaultTheme = DdlDefaultTheme.SelectedValue,
                IsCaptcha = CbIsCaptcha.Checked,
                IsAdministratorSmsNotify = Convert.ToBoolean(DdlIsAdministratorSmsNotify.SelectedValue),
                AdministratorSmsNotifyTplId = TbAdministratorSmsNotifyTplId.Text,
                AdministratorSmsNotifyKeys =
                    Utils.GetSelectedListControlValueCollection(LbAdministratorSmsNotifyKeys),
                AdministratorSmsNotifyMobile = TbAdministratorSmsNotifyMobile.Text
            };

            FormInfo.Title = TbTitle.Text;
            FormInfo.Description = TbDescription.Text;
            FormInfo.IsTimeout = CbIsTimeout.Checked;
            FormInfo.TimeToStart = TbTimeToStart.DateTime;
            FormInfo.TimeToEnd = TbTimeToEnd.DateTime;
            
            FormInfo.Settings = settings.ToString();

            Main.Instance.FormDao.Update(FormInfo);

            LtlMessage.Text = Utils.GetMessageHtml("设置保存成功！", true);
        }
    }
}
