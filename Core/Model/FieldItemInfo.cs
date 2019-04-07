
using Datory;

namespace SS.Form.Core.Model
{
    [Table("ss_form_field_item")]
	public class FieldItemInfo : Entity
	{
	    [TableColumn]
	    public int FormId { get; set; }

	    [TableColumn]
        public int FieldId { get; set; }

	    [TableColumn]
        public string Value { get; set; }

	    [TableColumn]
        public bool IsSelected { get; set; }

	    [TableColumn]
        public bool IsExtras { get; set; }
    }
}
