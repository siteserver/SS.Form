using System.Linq;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Core.Parse;
using SS.Form.Core.Provider;
using Menu = SiteServer.Plugin.Menu;

namespace SS.Form
{
    public class Main : PluginBase
    {
        public override void Startup(IService service)
        {
            service
                .AddSiteMenu(siteId =>
                {
                    var formInfoList = FormManager.GetFormInfoList(siteId, 0);
                    var menus = formInfoList.Where(formInfo => !string.IsNullOrEmpty(formInfo.Title)).Select(formInfo => new Menu
                    {
                        Text = FormManager.GetFormTitle(formInfo),
                        Href = $"pages/logs.html?formId={formInfo.Id}"
                    }).ToList();

                    menus.Add(new Menu
                    {
                        Text = "表单管理",
                        Href = "pages/forms.html"
                    });
                    menus.Add(new Menu
                    {
                        Text = "表单模板",
                        Href = "pages/templates.html"
                    });

                    return new Menu
                    {
                        Text = "表单",
                        IconClass = "ion-android-list",
                        Menus = menus
                    };
                })
                .AddContentMenu(contentInfo =>
                {
                    var formInfo =
                        FormManager.GetFormInfoByContentId(contentInfo.SiteId, contentInfo.ChannelId, contentInfo.Id);

                    var menu = new Menu
                    {
                        Text = FormManager.GetFormTitle(formInfo),
                        Href = "pages/logs.html"
                    };
                    
                    return menu;
                })
                .AddDatabaseTable(FormDao.TableName, FormDao.Columns)
                .AddDatabaseTable(LogDao.TableName, LogDao.Columns)
                .AddDatabaseTable(FieldDao.TableName, FieldDao.Columns)
                .AddDatabaseTable(FieldItemDao.TableName, FieldItemDao.Columns)
                .AddStlElementParser(StlForm.ElementName, StlForm.Parse)
                ;
            
            service.ContentDeleteCompleted += Service_ContentDeleteCompleted;
        }

        private static void Service_ContentDeleteCompleted(object sender, ContentEventArgs e)
        {
            var formInfo = FormManager.GetFormInfoByContentId(e.SiteId, e.ChannelId, e.ContentId);
            if (formInfo != null)
            {
                FormDao.Delete(e.SiteId, formInfo.Id);
            }   
        }
    }
}