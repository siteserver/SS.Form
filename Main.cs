using System;
using System.Linq;
using System.Web.UI.WebControls;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Model;
using SS.Form.Pages;
using SS.Form.Parse;
using SS.Form.Provider;
using Menu = SiteServer.Plugin.Menu;

namespace SS.Form
{
    public class Main : PluginBase
    {
        public static Main Instance { get; private set; }

        public FormDao FormDao { get; private set; }
        public LogDao LogDao { get; private set; }
        public FieldDao FieldDao { get; private set; }
        public FieldItemDao FieldItemDao { get; private set; }

        public override void Startup(IService service)
        {
            FormDao = new FormDao(ConnectionString, DataApi);
            LogDao = new LogDao(ConnectionString, DataApi);
            FieldDao = new FieldDao(ConnectionString, DataApi);
            FieldItemDao = new FieldItemDao(ConnectionString, DataApi);

            service
                .AddSiteMenu(siteId =>
                {
                    var formInfoList = FormDao.GetFormInfoListNotInChannel(siteId);
                    var menus = formInfoList.Select(formInfo => new Menu
                    {
                        Text = $"{formInfo.Title}",
                        Href = $"{nameof(PageLogs)}.aspx?formId={formInfo.Id}"
                    }).ToList();

                    menus.Add(new Menu
                    {
                        Text = "表单管理",
                        Href = $"{nameof(PageManagement)}.aspx"
                    });

                    return new Menu
                    {
                        Text = "表单",
                        IconClass = "ion-android-list",
                        Menus = menus
                    };
                })
                .AddContentLink(new HyperLink
                {
                    Text = "表单管理",
                    NavigateUrl = $"{nameof(PageLogs)}.aspx"
                })
                .AddDatabaseTable(FormDao.TableName, FormDao.Columns)
                .AddDatabaseTable(LogDao.TableName, LogDao.Columns)
                .AddDatabaseTable(FieldDao.TableName, FieldDao.Columns)
                .AddDatabaseTable(FieldItemDao.TableName, FieldItemDao.Columns)
                .AddStlElementParser(StlForm.ElementName, StlForm.Parse)
                ;

            service.ContentTranslateCompleted += Service_ContentTranslateCompleted;
            service.ContentDeleteCompleted += Service_ContentDeleteCompleted;

            service.ApiPost += ServiceOnApiPost;
            service.ApiGet += Service_ApiGet;

            Instance = this;
        }

        private object ServiceOnApiPost(object sender, ApiEventArgs args)
        {
            if (Utils.EqualsIgnoreCase(args.Action, nameof(ApiUtils.Submit)))
            {
                return ApiUtils.Submit(args.Request, args.Id);
            }

            throw new Exception("请求的资源不在服务器上");
        }

        private object Service_ApiGet(object sender, ApiEventArgs args)
        {
            if (Utils.EqualsIgnoreCase(args.Action, nameof(ApiUtils.Captcha)))
            {
                return ApiUtils.Captcha(args.Request, args.Id);
            }

            throw new Exception("请求的资源不在服务器上");
        }

        private void Service_ContentTranslateCompleted(object sender, ContentTranslateEventArgs e)
        {
            var formInfo = FormDao.GetFormInfoOrCreateIfNotExists(e.SiteId, e.ChannelId, e.ContentId);

            formInfo.SiteId = e.TargetSiteId;
            formInfo.ChannelId = e.TargetChannelId;
            formInfo.ContentId = e.TargetContentId;
            formInfo.IsTimeout = false;
            formInfo.TimeToStart = DateTime.Now;
            formInfo.TimeToEnd = formInfo.TimeToStart.AddMonths(3);
            FormDao.Insert(formInfo);
        }

        private void Service_ContentDeleteCompleted(object sender, ContentEventArgs e)
        {
            var formId = FormDao.GetFormIdByContentId(e.SiteId, e.ChannelId, e.ContentId);
            FormDao.Delete(formId);
        }

        //public override Func<IRequestContext, string, string, HttpResponseMessage> HttpGetWithNameAndId => StlForm.HttpGetWithNameAndId;
    }
}