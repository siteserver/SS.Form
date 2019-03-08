using System;

namespace SS.Form.Core.Model
{
    public class FormSettings : AttributesImpl
    {
        public FormSettings(string str) : base(str)
        {
            
        }

        public bool IsClosed
        {
            get => GetBool(nameof(IsClosed));
            set => Set(nameof(IsClosed), value.ToString());
        }

        public string ListAttributeNames
        {
            get => GetString(nameof(ListAttributeNames), FormManager.DefaultListAttributeNames);
            set => Set(nameof(ListAttributeNames), value);
        }

        public bool IsCaptcha
        {
            get => GetBool(nameof(IsCaptcha));
            set => Set(nameof(IsCaptcha), value.ToString());
        }

        public bool IsTimeout
        {
            get => GetBool(nameof(IsTimeout));
            set => Set(nameof(IsTimeout), value.ToString());
        }

        public DateTime TimeToStart
        {
            get => GetDateTime(nameof(TimeToStart), DateTime.Now);
            set => Set(nameof(TimeToStart), value);
        }

        public DateTime TimeToEnd
        {
            get => GetDateTime(nameof(TimeToEnd), DateTime.Now.AddMonths(3));
            set => Set(nameof(TimeToEnd), value);
        }

        //向管理员发送短信通知
        public bool IsAdministratorSmsNotify
        {
            get => GetBool(nameof(IsAdministratorSmsNotify));
            set => Set(nameof(IsAdministratorSmsNotify), value.ToString());
        }

        public string AdministratorSmsNotifyTplId
        {
            get => GetString(nameof(AdministratorSmsNotifyTplId));
            set => Set(nameof(AdministratorSmsNotifyTplId), value);
        }

        public string AdministratorSmsNotifyKeys
        {
            get => GetString(nameof(AdministratorSmsNotifyKeys));
            set => Set(nameof(AdministratorSmsNotifyKeys), value);
        }

        public string AdministratorSmsNotifyMobile
        {
            get => GetString(nameof(AdministratorSmsNotifyMobile));
            set => Set(nameof(AdministratorSmsNotifyMobile), value);
        }

        //向管理员发送邮件通知
        public bool IsAdministratorMailNotify
        {
            get => GetBool(nameof(IsAdministratorMailNotify));
            set => Set(nameof(IsAdministratorMailNotify), value.ToString());
        }

        public string AdministratorMailNotifyAddress
        {
            get => GetString(nameof(AdministratorMailNotifyAddress));
            set => Set(nameof(AdministratorMailNotifyAddress), value);
        }
    }
}
