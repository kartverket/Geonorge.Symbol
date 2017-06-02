using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Geonorge.Symbol.Models.Api
{
    public class Symbol
    {
        public string SymbolUuid { get; set; }
        public string SymbolName { get; set; }
        public string Owner { get; set; }
        public string Theme { get; set; }
        public string PreviewImageUrl { get; set; }
        public string Format { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string Variant { get; set; }
        public string SymbolType { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string PackageUuid { get; set; }
        public string PackageName { get; set; }
    }
}