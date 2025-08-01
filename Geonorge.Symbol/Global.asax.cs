﻿using Autofac;
using Geonorge.Symbol.App_Start;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Geonorge.Symbol.Models.Translations;
using System.Collections.Specialized;

namespace Geonorge.Symbol
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MvcApplication));

        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = true;

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DependencyConfig.Configure(new ContainerBuilder());

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            log4net.Config.XmlConfigurator.Configure();
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError().GetBaseException();

            log.Error("App_Error", ex);
        }

        protected void Application_BeginRequest()
        {
            ValidateReturnUrl(Context.Request.QueryString);

            var cookie = Context.Request.Cookies["_culture"];
            if (cookie == null)
            {
                cookie = new HttpCookie("_culture", Culture.NorwegianCode);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }

            if (!string.IsNullOrEmpty(cookie.Value))
            {
                var culture = new CultureInfo(cookie.Value);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
        }

        protected void Application_EndRequest()
        {
            try
            {
                var redirectUri = HttpContext.Current.Request.Url.AbsoluteUri;

                var loggedInCookie = Context.Request.Cookies["_loggedIn"];
                if (string.IsNullOrEmpty(Request.QueryString["autologin"]) && loggedInCookie != null && loggedInCookie.Value == "true" && !Request.IsAuthenticated)
                {
                    if (!Request.Path.Contains("/SignOut") && !Request.Path.Contains("/signout-callback-oidc") && Request.QueryString["logout"] != "true" && !Request.Path.Contains("shared-partials-scripts") && !Request.Path.Contains("shared-partials-styles") && !Request.Path.Contains("kartverket-felleskomponenter") && !Request.Path.Contains("local-styles") && !Request.Path.Contains("local-scripts") && !Request.Path.Contains("CartographyList") && !Request.Path.ToLower().Contains("scripts") && !Request.Path.Contains("Content") && !Request.Path.Contains("bundles"))
                    {
                        var returnUrl = VirtualPathUtility.ToAbsolute("~/Files/SignIn") + "?autologin=true&ReturnUrl=" + redirectUri;
                        Response.Redirect(returnUrl);
                    }
                }
            }

            catch (Exception ex)
            {
            }
        }

        void ValidateReturnUrl(NameValueCollection queryString)
        {
            if (queryString != null)
            {
                var returnUrl = queryString.Get("returnUrl");
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    returnUrl = returnUrl.Replace("http://", "");
                    returnUrl = returnUrl.Replace("https://", "");

                    var host = Request.Url.Host;
                    if (returnUrl.StartsWith("localhost:44354"))
                        host = "localhost";

                    if (!returnUrl.StartsWith(host))
                        HttpContext.Current.Response.StatusCode = 400;
                }
            }
        }
    }
}
