namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class stklistxrate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StkHoldingModels", "Stk_stkcode", "dbo.StkModels");
            DropForeignKey("dbo.txModels", "Stk_stkcode", "dbo.StkModels");
            DropIndex("dbo.StkHoldingModels", new[] { "Stk_stkcode" });
            DropIndex("dbo.txModels", new[] { "Stk_stkcode" });
            DropPrimaryKey("dbo.StkModels");
            CreateTable(
                "dbo.Pending_txModel",
                c => new
                    {
                        txGUID = c.Guid(nullable: false, identity: true),
                        datetme = c.String(),
                        hrmmtme = c.String(),
                        buysell = c.String(),
                        shares = c.Int(nullable: false),
                        price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        currency = c.String(),
                        remark = c.String(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.txGUID)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.StklistModels",
                c => new
                    {
                        StklistGUID = c.Guid(nullable: false, identity: true),
                        stkcode = c.String(),
                        market = c.String(),
                        currency = c.String(),
                        remark = c.String(),
                        Pending_txModel_txGUID = c.Guid(),
                    })
                .PrimaryKey(t => t.StklistGUID)
                .ForeignKey("dbo.Pending_txModel", t => t.Pending_txModel_txGUID)
                .Index(t => t.Pending_txModel_txGUID);
            
            CreateTable(
                "dbo.StkHoldingModelStklistModels",
                c => new
                    {
                        StkHoldingModel_stkhldgGUID = c.Guid(nullable: false),
                        StklistModel_StklistGUID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.StkHoldingModel_stkhldgGUID, t.StklistModel_StklistGUID })
                .ForeignKey("dbo.StkHoldingModels", t => t.StkHoldingModel_stkhldgGUID, cascadeDelete: true)
                .ForeignKey("dbo.StklistModels", t => t.StklistModel_StklistGUID, cascadeDelete: true)
                .Index(t => t.StkHoldingModel_stkhldgGUID)
                .Index(t => t.StklistModel_StklistGUID);
            
            CreateTable(
                "dbo.StkModelStklistModels",
                c => new
                    {
                        StkModel_stkGUID = c.Guid(nullable: false),
                        StklistModel_StklistGUID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.StkModel_stkGUID, t.StklistModel_StklistGUID })
                .ForeignKey("dbo.StkModels", t => t.StkModel_stkGUID, cascadeDelete: true)
                .ForeignKey("dbo.StklistModels", t => t.StklistModel_StklistGUID, cascadeDelete: true)
                .Index(t => t.StkModel_stkGUID)
                .Index(t => t.StklistModel_StklistGUID);
            
            CreateTable(
                "dbo.txModelStklistModels",
                c => new
                    {
                        txModel_txGUID = c.Guid(nullable: false),
                        StklistModel_StklistGUID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.txModel_txGUID, t.StklistModel_StklistGUID })
                .ForeignKey("dbo.txModels", t => t.txModel_txGUID, cascadeDelete: true)
                .ForeignKey("dbo.StklistModels", t => t.StklistModel_StklistGUID, cascadeDelete: true)
                .Index(t => t.txModel_txGUID)
                .Index(t => t.StklistModel_StklistGUID);
            
            AddColumn("dbo.StkModels", "stkGUID", c => c.Guid(nullable: false, identity: true));
            AddColumn("dbo.StkModels", "market", c => c.String());
            AddColumn("dbo.StkModels", "xrate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.txModels", "hrmmtme", c => c.String());
            AddColumn("dbo.txModels", "xrate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.StkModels", "stkcode", c => c.String());
            AddPrimaryKey("dbo.StkModels", "stkGUID");
            DropColumn("dbo.StkHoldingModels", "Stk_stkcode");
            DropColumn("dbo.txModels", "Stk_stkcode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.txModels", "Stk_stkcode", c => c.String(maxLength: 128));
            AddColumn("dbo.StkHoldingModels", "Stk_stkcode", c => c.String(maxLength: 128));
            DropForeignKey("dbo.Pending_txModel", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.StklistModels", "Pending_txModel_txGUID", "dbo.Pending_txModel");
            DropForeignKey("dbo.txModelStklistModels", "StklistModel_StklistGUID", "dbo.StklistModels");
            DropForeignKey("dbo.txModelStklistModels", "txModel_txGUID", "dbo.txModels");
            DropForeignKey("dbo.StkModelStklistModels", "StklistModel_StklistGUID", "dbo.StklistModels");
            DropForeignKey("dbo.StkModelStklistModels", "StkModel_stkGUID", "dbo.StkModels");
            DropForeignKey("dbo.StkHoldingModelStklistModels", "StklistModel_StklistGUID", "dbo.StklistModels");
            DropForeignKey("dbo.StkHoldingModelStklistModels", "StkHoldingModel_stkhldgGUID", "dbo.StkHoldingModels");
            DropIndex("dbo.txModelStklistModels", new[] { "StklistModel_StklistGUID" });
            DropIndex("dbo.txModelStklistModels", new[] { "txModel_txGUID" });
            DropIndex("dbo.StkModelStklistModels", new[] { "StklistModel_StklistGUID" });
            DropIndex("dbo.StkModelStklistModels", new[] { "StkModel_stkGUID" });
            DropIndex("dbo.StkHoldingModelStklistModels", new[] { "StklistModel_StklistGUID" });
            DropIndex("dbo.StkHoldingModelStklistModels", new[] { "StkHoldingModel_stkhldgGUID" });
            DropIndex("dbo.StklistModels", new[] { "Pending_txModel_txGUID" });
            DropIndex("dbo.Pending_txModel", new[] { "User_Id" });
            DropPrimaryKey("dbo.StkModels");
            AlterColumn("dbo.StkModels", "stkcode", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.txModels", "xrate");
            DropColumn("dbo.txModels", "hrmmtme");
            DropColumn("dbo.StkModels", "xrate");
            DropColumn("dbo.StkModels", "market");
            DropColumn("dbo.StkModels", "stkGUID");
            DropTable("dbo.txModelStklistModels");
            DropTable("dbo.StkModelStklistModels");
            DropTable("dbo.StkHoldingModelStklistModels");
            DropTable("dbo.StklistModels");
            DropTable("dbo.Pending_txModel");
            AddPrimaryKey("dbo.StkModels", "stkcode");
            CreateIndex("dbo.txModels", "Stk_stkcode");
            CreateIndex("dbo.StkHoldingModels", "Stk_stkcode");
            AddForeignKey("dbo.txModels", "Stk_stkcode", "dbo.StkModels", "stkcode");
            AddForeignKey("dbo.StkHoldingModels", "Stk_stkcode", "dbo.StkModels", "stkcode");
        }
    }
}
