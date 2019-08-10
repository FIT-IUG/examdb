namespace exam_db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateItemTableToAdd_description : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Items", "description");
        }
    }
}
