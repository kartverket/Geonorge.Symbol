using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Geonorge.Symbol.Services
{
    public class CodeList
    {
        public static readonly Dictionary<string, string> SymbolTypes = new Dictionary<string, string>()
        {
            {"punkt", "Punkt"},
            {"skravur", "Skravur"}
        };


        public static readonly Dictionary<string, string> Themes = new Dictionary<string, string>()
        {
            {"generell", "Generell"}
        };

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

    }
}