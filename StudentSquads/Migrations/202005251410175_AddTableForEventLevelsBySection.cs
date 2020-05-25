namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableForEventLevelsBySection : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RaitingSections", "EventLevel_Id", "dbo.EventLevels");
            DropIndex("dbo.RaitingSections", new[] { "EventLevel_Id" });
            CreateTable(
                "dbo.RaitingSectionLevels",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RaitingSectionId = c.Guid(nullable: false),
                        EventLevelId = c.Guid(nullable: false),
                        EventLevel_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EventLevels", t => t.EventLevel_Id)
                .ForeignKey("dbo.RaitingSections", t => t.RaitingSectionId, cascadeDelete: true)
                .Index(t => t.RaitingSectionId)
                .Index(t => t.EventLevel_Id);
            
            AddColumn("dbo.RaitingSections", "CountType", c => c.String());
            DropColumn("dbo.RaitingSections", "CountTyoe");
            DropColumn("dbo.RaitingSections", "EventLevelId");
            DropColumn("dbo.RaitingSections", "EventLevel_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RaitingSections", "EventLevel_Id", c => c.Int());
            AddColumn("dbo.RaitingSections", "EventLevelId", c => c.Guid(nullable: false));
            AddColumn("dbo.RaitingSections", "CountTyoe", c => c.String());
            DropForeignKey("dbo.RaitingSectionLevels", "RaitingSectionId", "dbo.RaitingSections");
            DropForeignKey("dbo.RaitingSectionLevels", "EventLevel_Id", "dbo.EventLevels");
            DropIndex("dbo.RaitingSectionLevels", new[] { "EventLevel_Id" });
            DropIndex("dbo.RaitingSectionLevels", new[] { "RaitingSectionId" });
            DropColumn("dbo.RaitingSections", "CountType");
            DropTable("dbo.RaitingSectionLevels");
            CreateIndex("dbo.RaitingSections", "EventLevel_Id");
            AddForeignKey("dbo.RaitingSections", "EventLevel_Id", "dbo.EventLevels", "Id");
        }
    }
}
