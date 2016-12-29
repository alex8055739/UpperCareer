using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AVOSCloud;

namespace RTCareerAsk
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            AVClient.Initialize("5ptNj5fF9TplwYYNYo34Ujmi-gzGzoHsz", "oxEMyVyz3XmlI8URg87Xp1l5");
            LeanCloud.AVClient.Initialize("5ptNj5fF9TplwYYNYo34Ujmi-gzGzoHsz", "oxEMyVyz3XmlI8URg87Xp1l5");
        }

        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    Exception ex = Server.GetLastError();

        //    if (ex is HttpRequestValidationException)
        //    {

        //        Response.Write("User input contains invalid characters.");

        //        Server.ClearError(); // 如果不ClearError()这个异常会继续传到Application_Error()。

        //    }
        //}
    }
}