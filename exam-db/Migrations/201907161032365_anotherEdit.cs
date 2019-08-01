namespace exam_db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class anotherEdit : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Items", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.Items", "UserId");
            RenameColumn(table: "dbo.Items", name: "ApplicationUser_Id", newName: "UserId");
            AlterColumn("dbo.Items", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Items", "UserId");
            CreateIndex("dbo.AspNetUsers", "departmentId");
            AddForeignKey("dbo.AspNetUsers", "departmentId", "dbo.Departments", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "departmentId", "dbo.Departments");
            DropIndex("dbo.AspNetUsers", new[] { "departmentId" });
            DropIndex("dbo.Items", new[] { "UserId" });
            AlterColumn("dbo.Items", "UserId", c => c.String());
            RenameColumn(table: "dbo.Items", name: "UserId", newName: "ApplicationUser_Id");
            AddColumn("dbo.Items", "UserId", c => c.String());
            CreateIndex("dbo.Items", "ApplicationUser_Id");
        }
    }
}
