namespace Geonorge.Symbol.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSymbolId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Symbols", "SymbolId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Symbols", "SymbolId");
        }
    }
}
