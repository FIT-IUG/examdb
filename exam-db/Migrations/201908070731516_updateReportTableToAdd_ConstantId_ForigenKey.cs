namespace exam_db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateReportTableToAdd_ConstantId_ForigenKey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reports", "ConstantId", c => c.Int(nullable: false));
            CreateIndex("dbo.Reports", "ConstantId");
            AddForeignKey("dbo.Reports", "ConstantId", "dbo.Constants", "Id", cascadeDelete: true);
            DropColumn("dbo.Reports", "desc");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reports", "desc", c => c.String());
            DropForeignKey("dbo.Reports", "ConstantId", "dbo.Constants");
            DropIndex("dbo.Reports", new[] { "ConstantId" });
            DropColumn("dbo.Reports", "ConstantId");
        }
    }
}
