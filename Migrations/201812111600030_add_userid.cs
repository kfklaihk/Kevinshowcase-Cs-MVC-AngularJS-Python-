namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_userid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "userid", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "userid");
        }
    }
}
