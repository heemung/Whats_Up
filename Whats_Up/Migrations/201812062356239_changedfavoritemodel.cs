namespace Whats_Up.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedfavoritemodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Favorites", "Category", c => c.String(nullable: false));
            DropColumn("dbo.Favorites", "SatId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Favorites", "SatId", c => c.Int(nullable: false));
            DropColumn("dbo.Favorites", "Category");
        }
    }
}
