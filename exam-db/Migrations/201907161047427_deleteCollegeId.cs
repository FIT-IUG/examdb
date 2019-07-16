namespace exam_db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deleteCollegeId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "collegeId", "dbo.Colleges");
            DropIndex("dbo.AspNetUsers", new[] { "collegeId" });
            DropColumn("dbo.AspNetUsers", "collegeId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "collegeId", c => c.Int(nullable: false));
            CreateIndex("dbo.AspNetUsers", "collegeId");
            AddForeignKey("dbo.AspNetUsers", "collegeId", "dbo.Colleges", "Id", cascadeDelete: true);
        }
    }
}
