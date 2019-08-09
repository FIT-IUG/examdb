namespace exam_db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateidentity2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "firstname", c => c.String(nullable: false));
            AddColumn("dbo.AspNetUsers", "lastname", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "lastname", c => c.String());
            AddColumn("dbo.AspNetUsers", "firstname", c => c.String());
        }
    }
}
