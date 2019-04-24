using System;
using System.Collections.Generic;
using Datory;
using SiteServer.Plugin;

namespace SS.Form.Core.Model
{
    [Table("ss_form_field")]
    public class FieldInfo : Entity, ICloneable
    {
        public FieldInfo()
	    {
	        FieldType = InputType.Text.Value;
            Items = new List<FieldItemInfo>();
        }

        [TableColumn]
        public int FormId { get; set; }

        [TableColumn]
        public int Taxis { get; set; }

        [TableColumn]
        public string Title { get; set; }

        [TableColumn(Length = 2000)]
        public string Description { get; set; }

        [TableColumn]
        public string PlaceHolder { get; set; }

        [TableColumn]
        public string FieldType { get; set; }

        [TableColumn]
        public string Validate { get; set; }

        [TableColumn]
        public int Columns { get; set; }

        [TableColumn]
        public int Height { get; set; }

        // not in database

        public List<FieldItemInfo> Items { get; set; }

	    public object Value { get; set; }

        public object Clone()
        {
            return (FieldInfo)MemberwiseClone();
        }
    }
}
