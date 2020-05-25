namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadeEventLevelIdInt1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RaitingSectionLevels", "EventLevel_Id", "dbo.EventLevels");
            DropIndex("dbo.RaitingSectionLevels", new[] { "EventLevel_Id" });
            DropColumn("dbo.RaitingSectionLevels", "EventLevelId");
            RenameColumn(table: "dbo.RaitingSectionLevels", name: "EventLevel_Id", newName: "EventLevelId");
            AlterColumn("dbo.RaitingSectionLevels", "EventLevelId", c => c.Int(nullable: false));
            AlterColumn("dbo.RaitingSectionLevels", "EventLevelId", c => c.Int(nullable: false));
            CreateIndex("dbo.RaitingSectionLevels", "EventLevelId");
            AddForeignKey("dbo.RaitingSectionLevels", "EventLevelId", "dbo.EventLevels", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RaitingSectionLevels", "EventLevelId", "dbo.EventLevels");
            DropIndex("dbo.RaitingSectionLevels", new[] { "EventLevelId" });
            AlterColumn("dbo.RaitingSectionLevels", "EventLevelId", c => c.Int());
            AlterColumn("dbo.RaitingSectionLevels", "EventLevelId", c => c.Guid(nullable: false));
            RenameColumn(table: "dbo.RaitingSectionLevels", name: "EventLevelId", newName: "EventLevel_Id");
            AddColumn("dbo.RaitingSectionLevels", "EventLevelId", c => c.Guid(nullable: false));
            CreateIndex("dbo.RaitingSectionLevels", "EventLevel_Id");
            AddForeignKey("dbo.RaitingSectionLevels", "EventLevel_Id", "dbo.EventLevels", "Id");
        }
    }
}
