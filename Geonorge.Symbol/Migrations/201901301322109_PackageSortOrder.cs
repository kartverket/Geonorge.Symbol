namespace Geonorge.Symbol.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PackageSortOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SymbolPackages", "SortOrder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SymbolPackages", "SortOrder");
        }
    }
}
