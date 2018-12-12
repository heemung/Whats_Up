namespace Whats_Up.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ty : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "PostalCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "PostalCode", c => c.String());
        }
    }
}
