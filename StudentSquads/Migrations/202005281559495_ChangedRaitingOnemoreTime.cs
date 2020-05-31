namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedRaitingOnemoreTime : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RaitingPlaces", "SquadId", "dbo.Squads");
            DropIndex("dbo.RaitingPlaces", new[] { "SquadId" });
            AlterColumn("dbo.RaitingPlaces", "SquadId", c => c.Guid());
            CreateIndex("dbo.RaitingPlaces", "SquadId");
            AddForeignKey("dbo.RaitingPlaces", "SquadId", "dbo.Squads", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RaitingPlaces", "SquadId", "dbo.Squads");
            DropIndex("dbo.RaitingPlaces", new[] { "SquadId" });
            AlterColumn("dbo.RaitingPlaces", "SquadId", c => c.Guid(nullable: false));
            CreateIndex("dbo.RaitingPlaces", "SquadId");
            AddForeignKey("dbo.RaitingPlaces", "SquadId", "dbo.Squads", "Id", cascadeDelete: true);
        }
    }
}
