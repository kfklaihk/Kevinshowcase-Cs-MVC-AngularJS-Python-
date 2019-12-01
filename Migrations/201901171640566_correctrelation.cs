namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class correctrelation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StkHoldingModelStklistModels", "StkHoldingModel_stkhldgGUID", "dbo.StkHoldingModels");
            DropForeignKey("dbo.StkHoldingModelStklistModels", "StklistModel_StklistGUID", "dbo.StklistModels");
            DropForeignKey("dbo.StkModelStklistModels", "StkModel_stkGUID", "dbo.StkModels");
            DropForeignKey("dbo.StkModelStklistModels", "StklistModel_StklistGUID", "dbo.StklistModels");
            DropForeignKey("dbo.txModelStklistModels", "txModel_txGUID", "dbo.txModels");
            DropForeignKey("dbo.txModelStklistModels", "StklistModel_StklistGUID", "dbo.StklistModels");
            DropForeignKey("dbo.StklistModels", "Pending_txModel_txGUID", "dbo.Pending_txModel");
            DropIndex("dbo.StklistModels", new[] { "Pending_txModel_txGUID" });
            DropIndex("dbo.StkHoldingModelStklistModels", new[] { "StkHoldingModel_stkhldgGUID" });
            DropIndex("dbo.StkHoldingModelStklistModels", new[] { "StklistModel_StklistGUID" });
            DropIndex("dbo.StkModelStklistModels", new[] { "StkModel_stkGUID" });
            DropIndex("dbo.StkModelStklistModels", new[] { "StklistModel_StklistGUID" });
            DropIndex("dbo.txModelStklistModels", new[] { "txModel_txGUID" });
            DropIndex("dbo.txModelStklistModels", new[] { "StklistModel_StklistGUID" });
            AddColumn("dbo.Pending_txModel", "Stklist_StklistGUID", c => c.Guid());
            AddColumn("dbo.StklistModels", "name", c => c.String());
            AddColumn("dbo.StkHoldingModels", "Stklist_StklistGUID", c => c.Guid());
            AddColumn("dbo.StkModels", "Stklist_StklistGUID", c => c.Guid());
            AddColumn("dbo.txModels", "Stklist_StklistGUID", c => c.Guid());
            CreateIndex("dbo.Pending_txModel", "Stklist_StklistGUID");
            CreateIndex("dbo.StkHoldingModels", "Stklist_StklistGUID");
            CreateIndex("dbo.StkModels", "Stklist_StklistGUID");
            CreateIndex("dbo.txModels", "Stklist_StklistGUID");
            AddForeignKey("dbo.Pending_txModel", "Stklist_StklistGUID", "dbo.StklistModels", "StklistGUID");
            AddForeignKey("dbo.StkHoldingModels", "Stklist_StklistGUID", "dbo.StklistModels", "StklistGUID");
            AddForeignKey("dbo.StkModels", "Stklist_StklistGUID", "dbo.StklistModels", "StklistGUID");
            AddForeignKey("dbo.txModels", "Stklist_StklistGUID", "dbo.StklistModels", "StklistGUID");
            DropColumn("dbo.Pending_txModel", "currency");
            DropColumn("dbo.StklistModels", "Pending_txModel_txGUID");
            DropColumn("dbo.StkModels", "currency");
            DropColumn("dbo.StkModels", "market");
            DropColumn("dbo.txModels", "currency");
            DropTable("dbo.StkHoldingModelStklistModels");
            DropTable("dbo.StkModelStklistModels");
            DropTable("dbo.txModelStklistModels");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.txModelStklistModels",
                c => new
                    {
                        txModel_txGUID = c.Guid(nullable: false),
                        StklistModel_StklistGUID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.txModel_txGUID, t.StklistModel_StklistGUID });
            
            CreateTable(
                "dbo.StkModelStklistModels",
                c => new
                    {
                        StkModel_stkGUID = c.Guid(nullable: false),
                        StklistModel_StklistGUID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.StkModel_stkGUID, t.StklistModel_StklistGUID });
            
            CreateTable(
                "dbo.StkHoldingModelStklistModels",
                c => new
                    {
                        StkHoldingModel_stkhldgGUID = c.Guid(nullable: false),
                        StklistModel_StklistGUID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.StkHoldingModel_stkhldgGUID, t.StklistModel_StklistGUID });
            
            AddColumn("dbo.txModels", "currency", c => c.String());
            AddColumn("dbo.StkModels", "market", c => c.String());
            AddColumn("dbo.StkModels", "currency", c => c.String());
            AddColumn("dbo.StklistModels", "Pending_txModel_txGUID", c => c.Guid());
            AddColumn("dbo.Pending_txModel", "currency", c => c.String());
            DropForeignKey("dbo.txModels", "Stklist_StklistGUID", "dbo.StklistModels");
            DropForeignKey("dbo.StkModels", "Stklist_StklistGUID", "dbo.StklistModels");
            DropForeignKey("dbo.StkHoldingModels", "Stklist_StklistGUID", "dbo.StklistModels");
            DropForeignKey("dbo.Pending_txModel", "Stklist_StklistGUID", "dbo.StklistModels");
            DropIndex("dbo.txModels", new[] { "Stklist_StklistGUID" });
            DropIndex("dbo.StkModels", new[] { "Stklist_StklistGUID" });
            DropIndex("dbo.StkHoldingModels", new[] { "Stklist_StklistGUID" });
            DropIndex("dbo.Pending_txModel", new[] { "Stklist_StklistGUID" });
            DropColumn("dbo.txModels", "Stklist_StklistGUID");
            DropColumn("dbo.StkModels", "Stklist_StklistGUID");
            DropColumn("dbo.StkHoldingModels", "Stklist_StklistGUID");
            DropColumn("dbo.StklistModels", "name");
            DropColumn("dbo.Pending_txModel", "Stklist_StklistGUID");
            CreateIndex("dbo.txModelStklistModels", "StklistModel_StklistGUID");
            CreateIndex("dbo.txModelStklistModels", "txModel_txGUID");
            CreateIndex("dbo.StkModelStklistModels", "StklistModel_StklistGUID");
            CreateIndex("dbo.StkModelStklistModels", "StkModel_stkGUID");
            CreateIndex("dbo.StkHoldingModelStklistModels", "StklistModel_StklistGUID");
            CreateIndex("dbo.StkHoldingModelStklistModels", "StkHoldingModel_stkhldgGUID");
            CreateIndex("dbo.StklistModels", "Pending_txModel_txGUID");
            AddForeignKey("dbo.StklistModels", "Pending_txModel_txGUID", "dbo.Pending_txModel", "txGUID");
            AddForeignKey("dbo.txModelStklistModels", "StklistModel_StklistGUID", "dbo.StklistModels", "StklistGUID", cascadeDelete: true);
            AddForeignKey("dbo.txModelStklistModels", "txModel_txGUID", "dbo.txModels", "txGUID", cascadeDelete: true);
            AddForeignKey("dbo.StkModelStklistModels", "StklistModel_StklistGUID", "dbo.StklistModels", "StklistGUID", cascadeDelete: true);
            AddForeignKey("dbo.StkModelStklistModels", "StkModel_stkGUID", "dbo.StkModels", "stkGUID", cascadeDelete: true);
            AddForeignKey("dbo.StkHoldingModelStklistModels", "StklistModel_StklistGUID", "dbo.StklistModels", "StklistGUID", cascadeDelete: true);
            AddForeignKey("dbo.StkHoldingModelStklistModels", "StkHoldingModel_stkhldgGUID", "dbo.StkHoldingModels", "stkhldgGUID", cascadeDelete: true);
        }
    }
}
