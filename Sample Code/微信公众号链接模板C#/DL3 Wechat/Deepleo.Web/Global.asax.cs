using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Deepleo.Weixin.SDK.Helpers;
using Deepleo.Web.Services;
using System.Text;
using System.IO;

namespace Deepleo.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {

        private const int Caching = 10;
        private static Dictionary<string, string> Meg = new Dictionary<string, string>();


        public static string GetCache(string CacheKey)
        {
            if (CacheKey != null && Meg.ContainsKey(CacheKey))
            {
                return Meg[CacheKey];
            }
            else
            {
                return null;
            }
            //System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            //return objCache[CacheKey];
        }

        public static void SetCache(string cacheKey, string objObject)
        {
            if (objObject == null || cacheKey == null)
                return;

            if (cacheKey.Length >= Caching)
                cacheKey.Remove(0);
            Meg[cacheKey] = objObject;


            //System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            //objCache.Insert(cacheKey, objObject);
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            WeixinConfig.Register();
            log4net.Config.XmlConfigurator.Configure(new FileInfo("LogWriterConfig.xml"));
            LogWriter.Default.WriteWarning("app started.");
        }

        protected void Application_End()
        {
            LogWriter.Default.WriteWarning("app stopped.");
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError().GetBaseException();
            StringBuilder str = new StringBuilder();
            str.Append("\r\n.客户信息：");
            string ip = "";
            if (Request.ServerVariables.Get("HTTP_X_FORWARDED_FOR") != null)
            {
                ip = Request.ServerVariables.Get("HTTP_X_FORWARDED_FOR").ToString().Trim();
            }
            else
            {
                ip = Request.ServerVariables.Get("Remote_Addr").ToString().Trim();
            }
            str.Append("\r\n\tIp:" + ip);
            str.Append("\r\n\t浏览器:" + Request.Browser.Browser.ToString());
            str.Append("\r\n\t浏览器版本:" + Request.Browser.MajorVersion.ToString());
            str.Append("\r\n\t操作系统:" + Request.Browser.Platform.ToString());
            str.Append("\r\n.错误信息：");
            str.Append("\r\n\t页面：" + Request.Url.ToString());
            str.Append("\r\n\t错误信息：" + ex.Message);
            str.Append("\r\n\t错误源：" + ex.Source);
            str.Append("\r\n\t异常方法：" + ex.TargetSite);
            str.Append("\r\n\t堆栈信息：" + ex.StackTrace);
            str.Append("\r\n--------------------------------------------------------------------------------------------------");
            //创建路径 
            LogWriter.Default.WriteError(str.ToString());
        }
    }
}