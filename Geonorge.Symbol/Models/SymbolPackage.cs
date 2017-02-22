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
        public string Name { get; set; }
    }
}