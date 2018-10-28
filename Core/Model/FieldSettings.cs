namespace SS.Form.Core.Model
{
    public class FieldSettings : AttributesImpl
    {
        public FieldSettings(string json): base(json) { }

        public int Height
        {
            get { return GetInt("Height"); }
            set { Set("Height", value.ToString()); }
        }

        public int Columns
        {
            get { return GetInt("Columns"); }
            set { Set("Columns", value.ToString()); }
        }
    }
}
