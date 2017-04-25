namespace Geonorge.Symbol.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PackageFolder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SymbolPackages", "Folder", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SymbolPackages", "Folder");
        }
    }
}
