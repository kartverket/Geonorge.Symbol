using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Geonorge.Symbol.Models
{
    public class Symbol
    {
        [Key]
        public Guid SystemId { get; set; }

        [Required]
        /// <summary> Beskrivende navn på symbol.</summary>
        [Display(Name = "Symbolnavn")]
        public string Name { get; set; }

        /// <summary>Ekstern ID</summary>
        [Display(Name = "Id")]
        public string SymbolId { get; set; }

        /// <summary> Beskrivelse av hva bildet viser eller brukes til</summary>
        [Display(Name = "Beskrivelse")]
        public string Description { get; set; }

        /// <summary> Thumbnail of symbol</summary>
        [Display(Name = "Symbol")]
        public string Thumbnail { get; set; }

        /// <summary> Organisasjon som har sendt inn filen.</summary>
        [Display(Name = "Organisasjon")]
        public string Owner { get; set; }

        /// <summary> Editor. Hentes fra pålogget bruker</summary>
        [Display(Name = "Redigert av")]
        public string LastEditedBy { get; set; }

        /// <summary> Punkt, skravur</summary>
        [Obsolete]
        [Display(Name = "Symboltype")]
        public string Type { get; set; }

        /// <summary>Dato for når filen/informasjonen i registeret sist ble endret.</summary>
        [Display(Name = "Dato endret")]
        [DisplayFormat(NullDisplayText = "", ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime DateChanged { get; set; } = DateTime.Now;

        /// <summary>Hovedtema verdi «Annen».</summary>
        [Display(Name = "Tema")]
        public string Theme { get; set; }

        /// <summary>Navn på symbolsamling symbolet hentes fra</summary>
        [Display(Name = "Kilde")]
        public string Source { get; set; }

        /// <summary>URL til symbolsamling symbolet hentes fra.</summary>
        [Display(Name = "Kilde-URL")]
        public string SourceUrl { get; set; }

        /// <summary>ulike grafiske forekomster av symbolet (ulike formater eller farger, .. )</summary>
        public virtual List<SymbolFile> SymbolFiles { get; set; }

        [Display(Name = "Pakke")]
        public virtual List<SymbolPackage> SymbolPackages { get; set; }

        public string ThumbnailUrl()
        {
            return CurrentDomain() + "/files/thumbnail/" + Thumbnail;
        }

        string CurrentDomain()
        {
            return (HttpContext.Current.Request.Url.Host.Contains("localhost") ? "http" : "https")
                 + System.Uri.SchemeDelimiter + HttpContext.Current.Request.Url.Host +
                 (HttpContext.Current.Request.Url.IsDefaultPort ? "" : ":" + HttpContext.Current.Request.Url.Port)
                 + (!HttpContext.Current.Request.Url.Host.Contains("localhost") ? "/symbol" : "");
        }

    }
}