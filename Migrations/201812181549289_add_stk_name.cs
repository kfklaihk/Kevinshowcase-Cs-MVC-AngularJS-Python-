namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_stk_name : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ViewModel_1",
                c => new
                    {
                        CurrentUser = c.String(nullable: false, maxLength: 128),
                        datme = c.String(),
                    })
                .PrimaryKey(t => t.CurrentUser);
            
            AddColumn("dbo.StkModels", "name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StkModels", "name");
            DropTable("dbo.ViewModel_1");
        }
    }
}
