using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace myhw
{
    public class RouteConfig
    {
       
            public static void RegisterRoutes(RouteCollection routes)
            {
                routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

                
                routes.MapRoute(
                    name: "Register",
                    url: "Account/Register",
                    defaults: new { controller = "Account", action = "Register", id = UrlParameter.Optional }
                );
               
            }
            public static void LogRoutes(RouteCollection routes)
            {
                routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

                routes.MapRoute(
                    name: "Log",
                    url: "Account/Log",
                    defaults: new { controller = "Account", action = "Log", id = UrlParameter.Optional }
                );

                routes.MapRoute(
                    name: "Front",
                    url: "Account/Front",
                    defaults: new { controller = "Account", action = "Front", id = UrlParameter.Optional }
                );
            }




    }
}
