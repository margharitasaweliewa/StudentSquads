namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadeEventLevelIdInt : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RaitingEvents", "EventLevel_Id", "dbo.EventLevels");
            DropIndex("dbo.RaitingEvents", new[] { "EventLevel_Id" });
            DropColumn("dbo.RaitingEvents", "EventLevelId");
            RenameColumn(table: "dbo.RaitingEvents", name: "EventLevel_Id", newName: "EventLevelId");
            AlterColumn("dbo.RaitingEvents", "EventLevelId", c => c.Int(nullable: false));
            AlterColumn("dbo.RaitingEvents", "EventLevelId", c => c.Int(nullable: false));
            CreateIndex("dbo.RaitingEvents", "EventLevelId");
            AddForeignKey("dbo.RaitingEvents", "EventLevelId", "dbo.EventLevels", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RaitingEvents", "EventLevelId", "dbo.EventLevels");
            DropIndex("dbo.RaitingEvents", new[] { "EventLevelId" });
            AlterColumn("dbo.RaitingEvents", "EventLevelId", c => c.Int());
            AlterColumn("dbo.RaitingEvents", "EventLevelId", c => c.Guid(nullable: false));
            RenameColumn(table: "dbo.RaitingEvents", name: "EventLevelId", newName: "EventLevel_Id");
            AddColumn("dbo.RaitingEvents", "EventLevelId", c => c.Guid(nullable: false));
            CreateIndex("dbo.RaitingEvents", "EventLevel_Id");
            AddForeignKey("dbo.RaitingEvents", "EventLevel_Id", "dbo.EventLevels", "Id");
        }
    }
}
