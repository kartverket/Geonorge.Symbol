﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Geonorge.Symbol.Models
{
    public class SymbolFile
    {
        [Key]
        public Guid SystemId { get; set; }

        /// <summary>Opplasting av filen i form av gif, png, svg. Filen lagres og URL til fila lagres i registeret</summary>
        public string FileName { get; set; } //Filnavn Autogenerert fra symbolnavn + «symbolgrafikk» + «format» + «farge» + «størrelse» + «filnavn». Eks: «fisk_{n,p,u}_{eps,svg,ai,tiff,png,gif}_{ro,bl,gr,gu,sv,gr,or,fi}_{l,m,s}

        /// <summary>Ai/jpg/pdf/tiff/eps/gif/png/svg</summary>
        public string Format { get; set; }

        /// <summary>Positiv, negativ, utenramme, nedtrekksliste</summary>
        [Display(Name = "Symbolgrafikk")]
        public string Type { get; set; }

        /// <summary>Rød, grønn, blå, gul, svart, oransje, fiolett, grå, annen, nedtrekksliste, tekstfelt</summary>
        [Display(Name = "Farge")]
        public string Color { get; set; }

        /// <summary>Angi størrelse på symbol. (Stor >1000 px, middels 250 px - 999, liten <250)</summary>
        [Display(Name = "Størrelse")]
        public string Size { get; set; }

        public virtual SymbolFileVariant SymbolFileVariant { get; set; }

        public virtual Symbol Symbol { get; set; }

        public string FileUrl()
        {
            var folder = Symbol.SymbolPackages.FirstOrDefault()?.Folder;
            if (!string.IsNullOrEmpty(folder))
                folder = folder + "/";

            return CurrentDomain() + "/files/" + folder + FileName;
        }

        string CurrentDomain()
        {
            return "https" + System.Uri.SchemeDelimiter
                 + HttpContext.Current.Request.Url.Host +
                 (HttpContext.Current.Request.Url.IsDefaultPort ? "" : ":" + HttpContext.Current.Request.Url.Port)
                 + (!HttpContext.Current.Request.Url.Host.Contains("localhost") ? "/symbol" : "");
        }

    }
}