using System;
using Datory;

namespace SS.Form.Core.Model
{
    [Table("ss_form")]
    public class FormInfo : Entity
    {
        [TableColumn]
        public int SiteId { get; set; }

        [TableColumn]
        public int ChannelId { get; set; }

        [TableColumn]
        public int ContentId { get; set; }

        [TableColumn]
        public string Title { get; set; }

        [TableColumn(Length = 2000)]
        public string Description { get; set; }

        [TableColumn]
        public int Taxis { get; set; }

        [TableColumn]
        public bool IsReply { get; set; }

        [TableColumn]
        public int RepliedCount { get; set; }

        [TableColumn]
        public int TotalCount { get; set; }

        [TableColumn]
        public DateTime? AddDate { get; set; }

        [TableColumn(Text = true, Extend = true)]
        public string Settings { get; set; }

        public bool IsClosed { get; set; }

        public string ListAttributeNames { get; set; }

        public bool IsCaptcha { get; set; }

        public bool IsTimeout { get; set; }

        public DateTime TimeToStart { get; set; }

        public DateTime TimeToEnd { get; set; }

        //向管理员发送短信通知
        public bool IsAdministratorSmsNotify { get; set; }

        public string AdministratorSmsNotifyTplId { get; set; }

        public string AdministratorSmsNotifyKeys { get; set; }

        public string AdministratorSmsNotifyMobile { get; set; }

        //向管理员发送邮件通知
        public bool IsAdministratorMailNotify { get; set; }

        public string AdministratorMailNotifyAddress { get; set; }

        //向用户发送短信通知
        public bool IsUserSmsNotify { get; set; }

        public string UserSmsNotifyTplId { get; set; }

        public string UserSmsNotifyKeys { get; set; }

        public string UserSmsNotifyMobileName { get; set; }
    }
}
