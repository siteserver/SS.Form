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
        }

		public int Id { get; set; }

	    public int FormId { get; set; }

        public int Taxis { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string PlaceHolder { get; set; }

        public string FieldType { get; set; }

	    public string Validate { get; set; }

	    public int Columns { get; set; }

	    public int Height { get; set; }

        // not in database

        public List<FieldItemInfo> Items { get; set; }

	    public object Value { get; set; }
    }
}
