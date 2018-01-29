using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SS.Form.Core;

namespace SS.Form.Controls
{
    public class DateTimeTextBox : TextBox
    {
        public const string OnFocus = "WdatePicker({isShowClear:false,readOnly:true,dateFmt:'yyyy-MM-dd HH:mm:ss'});";
        public const string FormatString = "yyyy-MM-dd HH:mm:ss";

        public const string OnFocusDateOnly = "WdatePicker({isShowClear:false,readOnly:true,dateFmt:'yyyy-MM-dd'});";
        public const string FormatStringDateOnly = "yyyy-MM-dd";

        public bool Now
        {
            get
            {
                var o = ViewState["Now"];
                if (o == null)
                {
                    return false;
                }
                return (bool)o;
            }
            set { ViewState["Now"] = value; }
        }

        public bool ShowTime
        {
            get
            {
                var o = ViewState["ShowTime"];
                if (o == null)
                {
                    return true;
                }
                return (bool)o;
            }
            set { ViewState["ShowTime"] = value; }
        }

        public override string Text
        {
            get
            {
                var formatString = ShowTime ? FormatString : FormatStringDateOnly;
                if (Now && string.IsNullOrEmpty(base.Text))
                {
                    base.Text = DateTime.Now.ToString(formatString);
                }
                if (!string.IsNullOrEmpty(base.Text))
                {
                    base.Text = Utils.ToDateTime(base.Text).ToString(formatString);
                }
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        public virtual DateTime DateTime
        {
            get
            {
                return Utils.ToDateTime(Text);
            }
            set
            {
                var formatString = ShowTime ? FormatString : FormatStringDateOnly;
                Text = value.ToString(formatString);
            }
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            var onfocus = (ShowTime) ? OnFocus : OnFocusDateOnly;
            Attributes.Add("onfocus", onfocus);
            base.AddAttributesToRender(writer);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (Page != null)
            {
                if (!Page.ClientScript.IsStartupScriptRegistered("DateTimeTextBox_Calendar"))
                {
                    Page.ClientScript.RegisterStartupScript(GetType(), "DateTimeTextBox_Calendar", @"<script language=""javascript"" src=""assets/datepicker/wdatepicker.js""></script>");
                }
            }
            base.OnLoad(e);
        }
    }
}
