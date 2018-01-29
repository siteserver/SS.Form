using System.Collections.Generic;

namespace SS.Form.Model
{
	public class FieldInfo
	{
		public int Id { get; set; }

	    public int FormId { get; set; }

        public int Taxis { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string PlaceHolder { get; set; }

        public string FieldType { get; set; }

        public string Settings { get; set; }

        // not in database

        public List<FieldItemInfo> Items { get; set; } = new List<FieldItemInfo>();

        public string Value { get; set; }
    }
}
