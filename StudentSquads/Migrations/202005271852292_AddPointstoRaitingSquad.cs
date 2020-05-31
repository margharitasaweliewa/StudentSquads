namespace StudentSquads.Migrations
{
    using DocumentFormat.OpenXml.Bibliography;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPointstoRaitingSquad : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RaitingPlaces", "Points", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RaitingPlaces", "Points");
        }
    }
}
