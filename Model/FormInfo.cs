using System;

namespace SS.Form.Model
{
    public class FormInfo
    {
        public int Id { get; set; }
        public int PublishmentSystemId { get; set; }
        public int ChannelId { get; set; }
        public int ContentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Taxis { get; set; }
        public bool IsTimeout { get; set; }
        public DateTime TimeToStart { get; set; }
        public DateTime TimeToEnd { get; set; }
        public string Settings { get; set; }
    }
}
