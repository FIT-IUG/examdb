namespace exam_db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateConstantTableToAdd_flag_and_parentId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Constants", "parentId", c => c.Int(nullable: false));
            AddColumn("dbo.Constants", "isParent", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Constants", "isParent");
            DropColumn("dbo.Constants", "parentId");
        }
    }
}
