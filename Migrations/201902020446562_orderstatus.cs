namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class orderstatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ViewModel_1", "order_status", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ViewModel_1", "order_status");
        }
    }
}
