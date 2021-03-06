﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Geonorge.Symbol
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute("OIDC-callback-signout", "signout-callback-oidc", new { controller = "Files", action = "SignOutCallback" });
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{SystemId}",
                defaults: new { controller = "Files", action = "Index", SystemId = UrlParameter.Optional }
            );
        }
    }
}
