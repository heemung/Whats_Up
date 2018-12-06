namespace Whats_Up.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SatelliteN2YOs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Category = c.String(nullable: false),
                        TransactionsCount = c.Int(nullable: false),
                        SatCount = c.Int(nullable: false),
                        SatId = c.Int(),
                        SatName = c.String(),
                        Designator = c.String(),
                        LaunchDate = c.String(),
                        SatLat = c.Double(),
                        SatLng = c.Double(),
                        SatAlt = c.Double(),
                        AtTime = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SatelliteN2YOs");
        }
    }
}
