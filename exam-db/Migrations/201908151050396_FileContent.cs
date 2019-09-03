namespace exam_db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FileContent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "FileContent", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Items", "FileContent");
        }
    }
}
