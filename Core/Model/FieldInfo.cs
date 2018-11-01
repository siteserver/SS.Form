using System.Collections.Generic;
using SiteServer.Plugin;

namespace SS.Form.Core.Model
{
	public class FieldInfo
	{
	    public FieldInfo()
	    {
	        FieldType = InputType.Text.Value;
            Items = new List<FieldItemInfo>();
	        Additional = new FieldSettings(string.Empty);
        }

		public int Id { get; set; }

	    public int FormId { get; set; }

        public int Taxis { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string PlaceHolder { get; set; }

        public string FieldType { get; set; }

	    public string Validate { get; set; }

        public string Settings { get; set; }

        // not in database

        public List<FieldItemInfo> Items { get; set; }

	    public FieldSettings Additional { get; set; }

	    public object Value { get; set; }
    }
}
