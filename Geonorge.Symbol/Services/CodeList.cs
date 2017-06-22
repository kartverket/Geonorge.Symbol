using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Geonorge.Symbol.Services
{
    public class CodeList
    {
        public static Dictionary<string, string> Themes()
        {
            Dictionary<string, string> themes = GetCodeList("42CECF70-0359-49E6-B8FF-0D6D52EBC73F");
            themes.Add("Sport", "Sport");
            themes.Add("Annen", "Annen");

            return themes;
        }


        public static readonly Dictionary<string, string> SymbolGraphics = new Dictionary<string, string>()
        {
            {"positiv", "Positiv"},
            {"negativ", "Negativ"},
            {"utenramme", "Utenramme"},
        };

        public static readonly Dictionary<string, string> Size = new Dictionary<string, string>()
        {
            {"liten", "Liten < 250px"},
            {"middels", "Middels 250px - 999px"},
            {"stor", "Stor >1000 px"},
        };

        public static Dictionary<string, string> GetCodeList(string systemid)
        {
            Dictionary<string, string> CodeValues = new Dictionary<string, string>();

            string url = System.Web.Configuration.WebConfigurationManager.AppSettings["RegistryUrl"] + "api/kodelister/" + systemid;
            System.Net.WebClient c = new System.Net.WebClient();
            c.Encoding = System.Text.Encoding.UTF8;
            var data = c.DownloadString(url);
            var response = Newtonsoft.Json.Linq.JObject.Parse(data);

            var codeList = response["containeditems"];

            foreach (var code in codeList)
            {
                JToken codevalueToken = code["codevalue"];
                string codevalue = codevalueToken?.ToString();

                if (string.IsNullOrWhiteSpace(codevalue))
                    codevalue = code["label"].ToString();

                if (!CodeValues.ContainsKey(codevalue))
                {
                    CodeValues.Add(codevalue, code["label"].ToString());
                }
            }

        CodeValues = CodeValues.OrderBy(o => o.Value).ToDictionary(o => o.Key, o => o.Value);

        return CodeValues;

        }

    }
}