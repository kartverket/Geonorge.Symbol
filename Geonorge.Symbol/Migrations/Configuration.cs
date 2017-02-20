namespace Geonorge.Symbol.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Geonorge.Symbol.Models.SymbolDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Geonorge.Symbol.Models.SymbolDbContext context)
        {
            //  This method will be called after migrating to the latest version.
        }
    }
}
