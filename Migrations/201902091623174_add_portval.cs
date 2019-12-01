namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_portval : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MM_Model",
                c => new
                    {
                        MMGUID = c.Guid(nullable: false, identity: true),
                        datetme = c.String(),
                        stk_val = c.Int(nullable: false),
                        stk_val_h = c.Int(nullable: false),
                        stk_val_l = c.Int(nullable: false),
                        freecash = c.Int(nullable: false),
                        margin = c.Int(nullable: false),
                        HSI = c.Int(nullable: false),
                        remark = c.String(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.MMGUID)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MM_Model", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.MM_Model", new[] { "User_Id" });
            DropTable("dbo.MM_Model");
        }
    }
}
