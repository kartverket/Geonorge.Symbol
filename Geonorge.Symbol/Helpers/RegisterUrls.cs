using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Geonorge.Symbol.Helpers
{
    public static class RegisterUrls
    {

        public static string GeonorgeUrl(this HtmlHelper helper)
        {
            return WebConfigurationManager.AppSettings["GeonorgeUrl"];
        }

        public static string RegistryUrl(this HtmlHelper helper)
        {
            return WebConfigurationManager.AppSettings["RegistryUrl"];
        }

        public static string UrlActionSignIn(this HtmlHelper helper)
        {
            return WebConfigurationManager.AppSettings["UrlActionSignIn"];
        }

        public static string UrlActionSignOut(this HtmlHelper helper)
        {
            return WebConfigurationManager.AppSettings["UrlActionSignOut"];
        }

        public static string ObjektkatalogUrl(this HtmlHelper helper)
        {
            return WebConfigurationManager.AppSettings["ObjektkatalogUrl"];
        }
        public static string KartkatalogenUrl(this HtmlHelper helper)
        {
            return WebConfigurationManager.AppSettings["KartkatalogenUrl"];
        }

        public static string EditorUrl(this HtmlHelper helper)
        {
            return WebConfigurationManager.AppSettings["EditorUrl"];
        }
    }
}