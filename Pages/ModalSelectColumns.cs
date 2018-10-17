using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SS.Form.Core;
using SS.Form.Model;
using SS.Form.Provider;

namespace SS.Form.Pages
{
    public class ModalSelectColumns : PageBase
    {
        public CheckBoxList CblDisplayAttributes;

        private List<FieldInfo> _fieldInfoList;

        public static string GetOpenScript(int siteId, int formId)
        {
            return LayerUtils.GetOpenScript("选择需要显示的项", $"{nameof(ModalSelectColumns)}.aspx?siteId={siteId}&formId={formId}", 520, 550);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            _fieldInfoList = FieldDao.GetFieldInfoList(FormInfo.Id, false);

            if (IsPostBack) return;

            foreach (var fieldInfo in _fieldInfoList)
            {
                var listitem = new ListItem(fieldInfo.Title, fieldInfo.Title);
                var settings = new FieldSettings(fieldInfo.Settings);
                listitem.Selected = settings.IsVisibleInList;

                CblDisplayAttributes.Items.Add(listitem);
            }
        }

        public void Submit_OnClick(object sender, EventArgs e)
        {
            foreach (ListItem item in CblDisplayAttributes.Items)
            {
                foreach (var fieldInfo in _fieldInfoList)
                {
                    if (fieldInfo.Title != item.Value) continue;

                    var settings = new FieldSettings(fieldInfo.Settings);
                    if (settings.IsVisibleInList != item.Selected)
                    {
                        settings.IsVisibleInList = item.Selected;
                        fieldInfo.Settings = settings.ToString();
                        FieldDao.Update(fieldInfo);
                    }

                    break;
                }
            }

            LayerUtils.Close(Page);
        }

    }
}
