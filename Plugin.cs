using System.Linq;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Core.Provider;
using Menu = SiteServer.Plugin.Menu;

namespace SS.Form
{
    public class Plugin : PluginBase
    {
        public override void Startup(IService service)
        {
            var formRepository = new FormRepository();
            var fieldRepository = new FieldRepository();
            var fieldItemRepository = new FieldItemRepository();
            var logRepository = new LogRepository();

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
                .AddDatabaseTable(formRepository.TableName, formRepository.TableColumns)
                .AddDatabaseTable(fieldRepository.TableName, fieldRepository.TableColumns)
                .AddDatabaseTable(fieldItemRepository.TableName, fieldItemRepository.TableColumns)
                .AddDatabaseTable(logRepository.TableName, logRepository.TableColumns)
                .AddStlElementParser(StlForm.ElementName, StlForm.Parse)
                ;
            
            service.ContentDeleteCompleted += Service_ContentDeleteCompleted;
        }

        private static void Service_ContentDeleteCompleted(object sender, ContentEventArgs e)
        {
            var formInfo = FormManager.GetFormInfoByContentId(e.SiteId, e.ChannelId, e.ContentId);
            if (formInfo != null)
            {
                FormManager.Repository.Delete(e.SiteId, formInfo.Id);
            }
        }
    }
}