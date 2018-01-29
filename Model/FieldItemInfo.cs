namespace SS.Form.Model
{
	public class FieldItemInfo
	{
        public int Id { get; set; }

	    public int FormId { get; set; }

        public int FieldId { get; set; }

        public string Value { get; set; }

	    public bool IsSelected { get; set; }

        public bool IsExtras { get; set; }
    }
}
