namespace Geonorge.Symbol.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SymbolFiles",
                c => new
                    {
                        SystemId = c.Guid(nullable: false),
                        FileName = c.String(),
                        Format = c.String(),
                        Type = c.String(),
                        Color = c.String(),
                        Size = c.String(),
                        Symbol_SystemId = c.Guid(),
                        SymbolFileVariant_SystemId = c.Guid(),
                    })
                .PrimaryKey(t => t.SystemId)
                .ForeignKey("dbo.Symbols", t => t.Symbol_SystemId)
                .ForeignKey("dbo.SymbolFileVariants", t => t.SymbolFileVariant_SystemId)
                .Index(t => t.Symbol_SystemId)
                .Index(t => t.SymbolFileVariant_SystemId);
            
            CreateTable(
                "dbo.Symbols",
                c => new
                    {
                        SystemId = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Thumbnail = c.String(),
                        Owner = c.String(),
                        LastEditedBy = c.String(),
                        Type = c.String(),
                        DateChanged = c.DateTime(nullable: false),
                        Theme = c.String(),
                        Source = c.String(),
                        SourceUrl = c.String(),
                    })
                .PrimaryKey(t => t.SystemId);
            
            CreateTable(
                "dbo.SymbolPackages",
                c => new
                    {
                        SystemId = c.Guid(nullable: false),
                        Name = c.String(),
                        OfficialStatus = c.Boolean(nullable: false),
                        Owner = c.String(),
                        Theme = c.String(),
                    })
                .PrimaryKey(t => t.SystemId);
            
            CreateTable(
                "dbo.SymbolFileVariants",
                c => new
                    {
                        SystemId = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.SystemId);
            
            CreateTable(
                "dbo.SymbolPackageSymbols",
                c => new
                    {
                        SymbolPackage_SystemId = c.Guid(nullable: false),
                        Symbol_SystemId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.SymbolPackage_SystemId, t.Symbol_SystemId })
                .ForeignKey("dbo.SymbolPackages", t => t.SymbolPackage_SystemId, cascadeDelete: true)
                .ForeignKey("dbo.Symbols", t => t.Symbol_SystemId, cascadeDelete: true)
                .Index(t => t.SymbolPackage_SystemId)
                .Index(t => t.Symbol_SystemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SymbolFiles", "SymbolFileVariant_SystemId", "dbo.SymbolFileVariants");
            DropForeignKey("dbo.SymbolPackageSymbols", "Symbol_SystemId", "dbo.Symbols");
            DropForeignKey("dbo.SymbolPackageSymbols", "SymbolPackage_SystemId", "dbo.SymbolPackages");
            DropForeignKey("dbo.SymbolFiles", "Symbol_SystemId", "dbo.Symbols");
            DropIndex("dbo.SymbolPackageSymbols", new[] { "Symbol_SystemId" });
            DropIndex("dbo.SymbolPackageSymbols", new[] { "SymbolPackage_SystemId" });
            DropIndex("dbo.SymbolFiles", new[] { "SymbolFileVariant_SystemId" });
            DropIndex("dbo.SymbolFiles", new[] { "Symbol_SystemId" });
            DropTable("dbo.SymbolPackageSymbols");
            DropTable("dbo.SymbolFileVariants");
            DropTable("dbo.SymbolPackages");
            DropTable("dbo.Symbols");
            DropTable("dbo.SymbolFiles");
        }
    }
}
