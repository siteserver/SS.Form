using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Pages;
using SS.Form.Parse;
using SS.Form.Provider;
using Menu = SiteServer.Plugin.Menu;

namespace SS.Form
{
    public class Main : IPlugin
    {
        public static IDataApi DataApi { get; private set; }
        public static IParseApi ParseApi { get; private set; }
        public static IFilesApi FilesApi { get; private set; }
        public static IAdminApi AdminApi { get; private set; }

        public static FormDao FormDao { get; private set; }
        public static LogDao LogDao { get; private set; }
        public static FieldDao FieldDao { get; private set; }
        public static FieldItemDao FieldItemDao { get; private set; }

        public void Startup(IContext context, IService service)
        {
            DataApi = context.DataApi;
            ParseApi = context.ParseApi;
            FilesApi = context.FilesApi;
            AdminApi = context.AdminApi;

            FormDao = new FormDao(context.Environment.ConnectionString, DataApi);
            LogDao = new LogDao(context.Environment.ConnectionString, DataApi);
            FieldDao = new FieldDao(context.Environment.ConnectionString, DataApi);
            FieldItemDao = new FieldItemDao(context.Environment.ConnectionString, DataApi);

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
                .AddContentLinks(new List<HyperLink>
                {
                    new HyperLink
                    {
                        Text = "表单管理",
                        NavigateUrl = $"{nameof(PageLogs)}.aspx"
                    }
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
        }

        private object ServiceOnApiPost(object sender, ApiEventArgs args)
        {
            if (Utils.EqualsIgnoreCase(args.Name, nameof(StlForm.ApiSubmit)))
            {
                return StlForm.ApiSubmit(args.Request, args.Id);
            }

            throw new Exception("请求的资源不在服务器上");
        }

        private object Service_ApiGet(object sender, ApiEventArgs args)
        {
            if (Utils.EqualsIgnoreCase(args.Name, nameof(StlForm.ApiGetCode)))
            {
                return StlForm.ApiGetCode(args.Request, args.Id);
            }

            throw new Exception("请求的资源不在服务器上");
        }

        private static void Service_ContentTranslateCompleted(object sender, ContentTranslateEventArgs e)
        {
            var formInfo = FormDao.GetFormInfoOrCreateIfNotExists(e.SiteId, e.ChannelId, e.ContentId);

            formInfo.PublishmentSystemId = e.TargetSiteId;
            formInfo.ChannelId = e.TargetChannelId;
            formInfo.ContentId = e.TargetContentId;
            FormDao.Insert(formInfo);
        }

        private static void Service_ContentDeleteCompleted(object sender, ContentEventArgs e)
        {
            var formId = FormDao.GetFormIdByContentId(e.SiteId, e.ChannelId, e.ContentId);
            FormDao.Delete(formId);
        }

        

        //public override Func<IRequestContext, string, string, HttpResponseMessage> HttpGetWithNameAndId => StlForm.HttpGetWithNameAndId;
    }
}