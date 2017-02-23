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
        public string Description { get; set; }

        /// <summary> Thumbnail of symbol</summary>
        [Display(Name = "Symbol")]
        public string Thumbnail { get; set; }

        /// <summary> Ekstern ID, eks surfisk3</summary>
        public string EksternalSymbolID { get; set; }

        /// <summary> Organisasjon som har sendt inn filen.</summary>
        [Display(Name = "Organisasjon")]
        public string Owner { get; set; }

        /// <summary> Editor. Hentes fra pålogget bruker</summary>
        public string LastEditedBy { get; set; }

        /// <summary> Punkt, skravur, dropdown</summary>
        [Display(Name = "Symboltype")]
        public string Type { get; set; }

        /// <summary>Dato for når filen/informasjonen i registeret sist ble endret.</summary>
        public DateTime DateChanged { get; set; }

        /// <summary>Viser på samme måte som andre registre i geonorge om filen er godkjent eller ikke. Settes av administrator. (Ikke synlig før den er godkjent)</summary>
        public string Status { get; set; }

        /// <summary>Dato for når filen ble godkjent. Settes av administrator.</summary>
        public DateTime DateAccepted { get; set; }

        /// <summary>Angi om kartografi er levert som offisielt tilbud eller som et alternativ til offisiell kartografi, radioknapp</summary>
        [Display(Name = "Offisiell")]
        public bool OfficialStatus { get; set; }

        /// <summary>Hovedtema (hentes fra liste) + verdi «generell».</summary>
        [Display(Name = "Tema")]
        public string Theme { get; set; }

        /// <summary>Navn på symbolsamling symbolet hentes fra. Tekstfelt</summary>
        [Display(Name = "Kilde")]
        public string Source { get; set; } 

        /// <summary>URL til symbolsamling symbolet hentes fra.</summary>
        public string SourceUrl { get; set; }

        /// <summary>ulike grafiske forekomster av symbolet (ulike formater eller farger, .. )</summary>
        public List<SymbolFile> SymbolFiles { get; set; }

        public SymbolPackage SymbolPackage { get; set; }
}
}