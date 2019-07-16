namespace exam_db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Items", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Items", new[] { "UserId" });
            AddColumn("dbo.Items", "ApplicationUser_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Items", "UserId", c => c.String());
            CreateIndex("dbo.Items", "ApplicationUser_Id");
            AddForeignKey("dbo.Items", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Items", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Items", new[] { "ApplicationUser_Id" });
            AlterColumn("dbo.Items", "UserId", c => c.String(maxLength: 128));
            DropColumn("dbo.Items", "ApplicationUser_Id");
            CreateIndex("dbo.Items", "UserId");
            AddForeignKey("dbo.Items", "UserId", "dbo.AspNetUsers", "Id");
        }
    }
}
