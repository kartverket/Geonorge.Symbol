using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Geonorge.Symbol.Models
{
    public class SymbolPackage
    {
        [Key]
        public Guid SystemId { get; set; }

        /// <summary> Beskrivende navn på pakke.</summary>
        [Display(Name = "Navn")]
        public string Name { get; set; }

        /// <summary>Angi om symbolpakke er offisiell eller ikke</summary>
        [Display(Name = "Offisiell")]
        public bool OfficialStatus { get; set; }

        /// <summary> Organisasjon som har sendt inn pakken.</summary>
        [Display(Name = "Eier")]
        public string Owner { get; set; }

        /// <summary>Hovedtema (hentes fra liste) + verdi «Annen».</summary>
        [Display(Name = "Tema")]
        public string Theme { get; set; }

    }
}