using System;
using System.Collections.Generic;

namespace SS.Form.Core.Model
{
    public class LogInfo : AttributesImpl
    {
        public int Id { get; set; }

        public DateTime AddDate { get; set; }

        public int FormId { get; set; }

        public bool IsReplied { get; set; }

        public DateTime ReplyDate { get; set; }

        public string ReplyContent { get; set; }

        public string AttributeValues { get; set; }

        public override Dictionary<string, object> ToDictionary()
        {
            var dict = base.ToDictionary();
            dict[nameof(Id)] = Id;
            dict[nameof(AddDate)] = AddDate;
            dict[nameof(IsReplied)] = IsReplied;
            dict[nameof(ReplyDate)] = ReplyDate;
            dict[nameof(ReplyContent)] = ReplyContent;

            return dict;
        }
    }
}
