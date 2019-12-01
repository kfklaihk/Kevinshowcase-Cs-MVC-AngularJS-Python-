namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nolazyloading : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ViewModel_1", "mkt_price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.ViewModel_1", "xrate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ViewModel_1", "xrate");
            DropColumn("dbo.ViewModel_1", "mkt_price");
        }
    }
}
