using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Geonorge.Symbol.Models
{
    public class SymbolDbContext : DbContext
    {
        public SymbolDbContext() : base("SymbolDbContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<SymbolDbContext, Geonorge.Symbol.Migrations.Configuration>("SymbolDbContext"));
        }

        public virtual DbSet<Symbol> Symbols { get; set; }
        public virtual DbSet<SymbolPackage> SymbolPackages { get; set; }

    }
}