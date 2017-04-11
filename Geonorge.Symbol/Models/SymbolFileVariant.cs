using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Geonorge.Symbol.Models
{
    public class SymbolFileVariant
    {
        [Key]
        public Guid SystemId { get; set; }

        public string Name { get; set; }
    }
}