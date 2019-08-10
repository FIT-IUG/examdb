namespace exam_db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateReportTable : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Reports", new[] { "User_Id" });
            DropColumn("dbo.Reports", "UserId");
            RenameColumn(table: "dbo.Reports", name: "User_Id", newName: "UserId");
            AlterColumn("dbo.Reports", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Reports", "UserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Reports", new[] { "UserId" });
            AlterColumn("dbo.Reports", "UserId", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.Reports", name: "UserId", newName: "User_Id");
            AddColumn("dbo.Reports", "UserId", c => c.Int(nullable: false));
            CreateIndex("dbo.Reports", "User_Id");
        }
    }
}
