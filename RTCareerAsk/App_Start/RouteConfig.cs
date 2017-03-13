using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RTCareerAsk
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Message",
                url: "Message",
                defaults: new { controller = "Message", action = "Index" }
            );

            routes.MapRoute(
                name: "Question",
                url: "Question",
                defaults: new { controller = "Question", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Article",
                url: "Article",
                defaults: new { controller = "Article", action = "Index" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Feeds", id = UrlParameter.Optional }
            );
        }
    }
}