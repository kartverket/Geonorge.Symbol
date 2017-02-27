namespace Geonorge.Symbol.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Symbols",
                c => new
                    {
                        SystemId = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Thumbnail = c.String(),
                        EksternalSymbolID = c.String(),
                        Owner = c.String(),
                        LastEditedBy = c.String(),
                        Type = c.String(),
                        DateChanged = c.DateTime(nullable: false),
                        OfficialStatus = c.Boolean(nullable: false),
                        Theme = c.String(),
                        Source = c.String(),
                        SourceUrl = c.String(),
                        SymbolPackage_SystemId = c.Guid(),
                    })
                .PrimaryKey(t => t.SystemId)
                .ForeignKey("dbo.SymbolPackages", t => t.SymbolPackage_SystemId)
                .Index(t => t.SymbolPackage_SystemId);
            
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
                    })
                .PrimaryKey(t => t.SystemId)
                .ForeignKey("dbo.Symbols", t => t.Symbol_SystemId, cascadeDelete: true)
                .Index(t => t.Symbol_SystemId);
            
            CreateTable(
                "dbo.SymbolPackages",
                c => new
                    {
                        SystemId = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.SystemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Symbols", "SymbolPackage_SystemId", "dbo.SymbolPackages");
            DropForeignKey("dbo.SymbolFiles", "Symbol_SystemId", "dbo.Symbols");
            DropIndex("dbo.SymbolFiles", new[] { "Symbol_SystemId" });
            DropIndex("dbo.Symbols", new[] { "SymbolPackage_SystemId" });
            DropTable("dbo.SymbolPackages");
            DropTable("dbo.SymbolFiles");
            DropTable("dbo.Symbols");
        }
    }
}
