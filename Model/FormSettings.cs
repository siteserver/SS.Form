namespace SS.Form.Model
{
    public class FormSettings : AttributesImpl
    {
        public FormSettings(string str) : base(str)
        {
            
        }

        public string DefaultTheme
        {
            get { return GetString("DefaultTheme"); }
            set { Set("DefaultTheme", value); }
        }

        public bool IsCaptcha
        {
            get { return GetBool("IsCaptcha"); }
            set { Set("IsCaptcha", value.ToString()); }
        }

        //向管理员发送短信通知
        public bool IsAdministratorSmsNotify
        {
            get { return GetBool("IsAdministratorSmsNotify"); }
            set { Set("IsAdministratorSmsNotify", value.ToString()); }
        }

        public string AdministratorSmsNotifyTplId
        {
            get { return GetString("AdministratorSmsNotifyTplId"); }
            set { Set("AdministratorSmsNotifyTplId", value); }
        }

        public string AdministratorSmsNotifyKeys
        {
            get { return GetString("AdministratorSmsNotifyKeys"); }
            set { Set("AdministratorSmsNotifyKeys", value); }
        }

        public string AdministratorSmsNotifyMobile
        {
            get { return GetString("AdministratorSmsNotifyMobile"); }
            set { Set("AdministratorSmsNotifyMobile", value); }
        }
    }
}
