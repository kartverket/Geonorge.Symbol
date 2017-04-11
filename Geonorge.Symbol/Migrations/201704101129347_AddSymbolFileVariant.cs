namespace Geonorge.Symbol.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSymbolFileVariant : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SymbolFileVariants",
                c => new
                    {
                        SystemId = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.SystemId);
            
            AddColumn("dbo.SymbolFiles", "SymbolFileVariant_SystemId", c => c.Guid());
            CreateIndex("dbo.SymbolFiles", "SymbolFileVariant_SystemId");
            AddForeignKey("dbo.SymbolFiles", "SymbolFileVariant_SystemId", "dbo.SymbolFileVariants", "SystemId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SymbolFiles", "SymbolFileVariant_SystemId", "dbo.SymbolFileVariants");
            DropIndex("dbo.SymbolFiles", new[] { "SymbolFileVariant_SystemId" });
            DropColumn("dbo.SymbolFiles", "SymbolFileVariant_SystemId");
            DropTable("dbo.SymbolFileVariants");
        }
    }
}
