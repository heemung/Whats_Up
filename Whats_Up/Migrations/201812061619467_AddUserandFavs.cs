namespace Whats_Up.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserandFavs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Favorites",
                c => new
                    {
                        FavID = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false),
                        SatId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FavID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Email = c.String(nullable: false, maxLength: 128),
                        AddressLine = c.String(),
                        PostalCode = c.String(),
                    })
                .PrimaryKey(t => t.Email);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
            DropTable("dbo.Favorites");
        }
    }
}
