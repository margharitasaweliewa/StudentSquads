namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedHeadsOgStudentSquads : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HeadsOfStudentSquads",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Position = c.String(),
                        SquadId = c.Guid(),
                        PersonId = c.Guid(nullable: false),
                        DateofBegin = c.DateTime(),
                        DateofEnd = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.People", t => t.PersonId, cascadeDelete: true)
                .ForeignKey("dbo.Squads", t => t.SquadId)
                .Index(t => t.SquadId)
                .Index(t => t.PersonId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HeadsOfStudentSquads", "SquadId", "dbo.Squads");
            DropForeignKey("dbo.HeadsOfStudentSquads", "PersonId", "dbo.People");
            DropIndex("dbo.HeadsOfStudentSquads", new[] { "PersonId" });
            DropIndex("dbo.HeadsOfStudentSquads", new[] { "SquadId" });
            DropTable("dbo.HeadsOfStudentSquads");
        }
    }
}
