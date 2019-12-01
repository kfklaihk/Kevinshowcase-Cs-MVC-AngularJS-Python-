namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Tables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CashholdingModels",
                c => new
                    {
                        cashhldgGUID = c.Guid(nullable: false, identity: true),
                        datetme = c.String(),
                        freecash = c.Int(nullable: false),
                        margin = c.Int(nullable: false),
                        remark = c.String(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.cashhldgGUID)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.StkHoldingModels",
                c => new
                    {
                        stkhldgGUID = c.Guid(nullable: false, identity: true),
                        datetme = c.String(),
                        shares = c.Int(nullable: false),
                        remark = c.String(),
                        Stk_stkcode = c.String(maxLength: 128),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.stkhldgGUID)
                .ForeignKey("dbo.StkModels", t => t.Stk_stkcode)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.Stk_stkcode)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.StkModels",
                c => new
                    {
                        stkcode = c.String(nullable: false, maxLength: 128),
                        datetme = c.String(),
                        oprice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        hprice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        lprice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        cprice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        dividend = c.Decimal(nullable: false, precision: 18, scale: 2),
                        currency = c.String(),
                        remark = c.String(),
                    })
                .PrimaryKey(t => t.stkcode);
            
            CreateTable(
                "dbo.txModels",
                c => new
                    {
                        txGUID = c.Guid(nullable: false, identity: true),
                        datetme = c.String(),
                        buysell = c.String(),
                        shares = c.Int(nullable: false),
                        Stkcode = c.String(),
                        price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        currency = c.String(),
                        remark = c.String(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.txGUID)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.txModels", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.StkHoldingModels", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.StkHoldingModels", "Stk_stkcode", "dbo.StkModels");
            DropForeignKey("dbo.CashholdingModels", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.txModels", new[] { "User_Id" });
            DropIndex("dbo.StkHoldingModels", new[] { "User_Id" });
            DropIndex("dbo.StkHoldingModels", new[] { "Stk_stkcode" });
            DropIndex("dbo.CashholdingModels", new[] { "User_Id" });
            DropTable("dbo.txModels");
            DropTable("dbo.StkModels");
            DropTable("dbo.StkHoldingModels");
            DropTable("dbo.CashholdingModels");
        }
    }
}
