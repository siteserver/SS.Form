using SiteServer.Plugin;
using SS.Form.Core;

namespace SS.Form.Model
{
    public class FieldSettings : AttributesImpl
    {
        public FieldSettings()
        {

        }

        public FieldSettings(string extendValues): base(extendValues) { }

        public bool IsVisibleInList
        {
            get { return GetBool("IsVisibleInList", true); }
            set { Set("IsVisibleInList", value.ToString()); }
        }

        public bool IsRequired
        {
            get { return GetBool("IsRequired"); }
            set { Set("IsRequired", value.ToString()); }
        }

        public ValidateType ValidateType
        {
            get { return ValidateTypeUtils.GetEnumType(GetString("ValidateType")); }
            set { Set("ValidateType", value.Value); }
        }

        public int MinNum
        {
            get { return GetInt("MinNum"); }
            set { Set("MinNum", value.ToString()); }
        }

        public int MaxNum
        {
            get { return GetInt("MaxNum"); }
            set { Set("MaxNum", value.ToString()); }
        }
    }
}
