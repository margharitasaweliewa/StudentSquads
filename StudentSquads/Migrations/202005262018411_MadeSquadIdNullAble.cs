namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadeSquadIdNullAble : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RaitingEventInfoes", "SquadId", "dbo.Squads");
            DropIndex("dbo.RaitingEventInfoes", new[] { "SquadId" });
            AlterColumn("dbo.RaitingEventInfoes", "SquadId", c => c.Guid());
            CreateIndex("dbo.RaitingEventInfoes", "SquadId");
            AddForeignKey("dbo.RaitingEventInfoes", "SquadId", "dbo.Squads", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RaitingEventInfoes", "SquadId", "dbo.Squads");
            DropIndex("dbo.RaitingEventInfoes", new[] { "SquadId" });
            AlterColumn("dbo.RaitingEventInfoes", "SquadId", c => c.Guid(nullable: false));
            CreateIndex("dbo.RaitingEventInfoes", "SquadId");
            AddForeignKey("dbo.RaitingEventInfoes", "SquadId", "dbo.Squads", "Id", cascadeDelete: true);
        }
    }
}
