namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSquadtoSquadWork : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SquadWorks",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Season = c.Int(nullable: false),
                        Affirmed = c.Boolean(nullable: false),
                        Count = c.Int(nullable: false),
                        AlternativeCount = c.Int(nullable: false),
                        BadgesCount = c.Int(nullable: false),
                        SquadId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Squads", t => t.SquadId)
                .Index(t => t.SquadId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SquadWorks", "SquadId", "dbo.Squads");
            DropIndex("dbo.SquadWorks", new[] { "SquadId" });
            DropTable("dbo.SquadWorks");
        }
    }
}
