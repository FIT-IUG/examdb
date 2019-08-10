namespace exam_db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateFavorite : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Favorites", new[] { "User_Id" });
            DropColumn("dbo.Favorites", "UserId");
            RenameColumn(table: "dbo.Favorites", name: "User_Id", newName: "UserId");
            AlterColumn("dbo.Favorites", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Favorites", "UserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Favorites", new[] { "UserId" });
            AlterColumn("dbo.Favorites", "UserId", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.Favorites", name: "UserId", newName: "User_Id");
            AddColumn("dbo.Favorites", "UserId", c => c.Int(nullable: false));
            CreateIndex("dbo.Favorites", "User_Id");
        }
    }
}
