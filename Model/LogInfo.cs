using System;

namespace SS.Form.Model
{
    public class LogInfo : ExtendedAttributes
    {
        public int Id { get; set; }

        public int FormId { get; set; }

        public string ItemIds { get; set; }

        public string UniqueId { get; set; }

        public DateTime AddDate { get; set; }

        public string AttributeValues { get; set; }
    }
}
