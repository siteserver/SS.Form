using System;

namespace SS.Form.Core.Model
{
    public class FormInfo
    {
        public int Id { get; set; }

        public int SiteId { get; set; }

        public int ChannelId { get; set; }

        public int ContentId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Taxis { get; set; }

        public bool IsReply { get; set; }

        public int RepliedCount { get; set; }

        public int TotalCount { get; set; }

        public DateTime AddDate { get; set; }

        public string Settings { get; set; }

        // not in database

        public FormSettings Additional { get; set; }
    }
}
