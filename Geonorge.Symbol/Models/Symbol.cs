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

        /// <summary> Beskrivelse av hva bildet viser eller brukes til</summary>
        [Display(Name = "Beskrivelse")]
        public string Description { get; set; }

        /// <summary> Thumbnail of symbol</summary>
        [Display(Name = "Symbol")]
        public string Thumbnail { get; set; }

        /// <summary> Ekstern ID, eks surfisk3</summary>
        [Display(Name = "Ekstern ID")]
        public string EksternalSymbolID { get; set; }

        /// <summary> Organisasjon som har sendt inn filen.</summary>
        [Display(Name = "Organisasjon")]
        public string Owner { get; set; }

        /// <summary> Editor. Hentes fra pålogget bruker</summary>
        [Display(Name = "Redigert av")]
        public string LastEditedBy { get; set; }

        /// <summary> Punkt, skravur, dropdown</summary>
        [Display(Name = "Symboltype")]
        public string Type { get; set; }

        /// <summary>Dato for når filen/informasjonen i registeret sist ble endret.</summary>
        [Display(Name = "Dato endret")]
        [DisplayFormat(NullDisplayText = "", ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime DateChanged { get; set; } = DateTime.Now;

        /// <summary>Angi om symbol er offisiell eller ikke</summary>
        [Display(Name = "Offisiell")]
        public bool OfficialStatus { get; set; }

        /// <summary>Hovedtema verdi «Annen».</summary>
        [Display(Name = "Tema")]
        public string Theme { get; set; }

        /// <summary>Navn på symbolsamling symbolet hentes fra</summary>
        [Display(Name = "Kilde")]
        public string Source { get; set; }

        /// <summary>URL til symbolsamling symbolet hentes fra.</summary>
        [Display(Name = "Url til symbolsamling")]
        public string SourceUrl { get; set; }

        /// <summary>ulike grafiske forekomster av symbolet (ulike formater eller farger, .. )</summary>
        public List<SymbolFile> SymbolFiles { get; set; }

        public SymbolPackage SymbolPackage { get; set; }

        public string ThumbnailUrl()
        {
            return CurrentDomain() + "/files/" + Thumbnail;
        }

        string CurrentDomain()
        {
            return HttpContext.Current.Request.Url.Scheme + System.Uri.SchemeDelimiter
                 + HttpContext.Current.Request.Url.Host +
                 (HttpContext.Current.Request.Url.IsDefaultPort ? "" : ":" + HttpContext.Current.Request.Url.Port)
                 + (!HttpContext.Current.Request.Url.Host.Contains("localhost") ? "/kartografi" : "");
        }

    }
}