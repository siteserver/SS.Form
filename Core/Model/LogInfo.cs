using System;
using Datory;

namespace SS.Form.Core.Model
{
    [Table("ss_form_log")]
    public class LogInfo : Entity
    {
        [TableColumn]
        public DateTime AddDate { get; set; }

        [TableColumn]
        public int FormId { get; set; }

        [TableColumn]
        public bool IsReplied { get; set; }

        [TableColumn]
        public DateTime ReplyDate { get; set; }

        [TableColumn(Text = true)]
        public string ReplyContent { get; set; }

        [TableColumn(Text = true, Extend = true)]
        public string AttributeValues { get; set; }
    }
}
