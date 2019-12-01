namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class amend_txModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.txModels", "Stk_stkcode", c => c.String(maxLength: 128));
            CreateIndex("dbo.txModels", "Stk_stkcode");
            AddForeignKey("dbo.txModels", "Stk_stkcode", "dbo.StkModels", "stkcode");
            DropColumn("dbo.txModels", "Stkcode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.txModels", "Stkcode", c => c.String());
            DropForeignKey("dbo.txModels", "Stk_stkcode", "dbo.StkModels");
            DropIndex("dbo.txModels", new[] { "Stk_stkcode" });
            DropColumn("dbo.txModels", "Stk_stkcode");
        }
    }
}
