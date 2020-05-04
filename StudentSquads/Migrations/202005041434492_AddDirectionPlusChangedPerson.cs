namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDirectionPlusChangedPerson : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Directions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ShortName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.People", "PassportSerie", c => c.String());
            AddColumn("dbo.People", "PassportGiven", c => c.String());
            AddColumn("dbo.Squads", "DirectionId", c => c.Int());
            AlterColumn("dbo.People", "DateofIssue", c => c.DateTime(nullable: false));
            CreateIndex("dbo.Squads", "DirectionId");
            AddForeignKey("dbo.Squads", "DirectionId", "dbo.Directions", "Id");
            DropColumn("dbo.People", "PasportSerie");
        }
        
        public override void Down()
        {
            AddColumn("dbo.People", "PasportSerie", c => c.String());
            DropForeignKey("dbo.Squads", "DirectionId", "dbo.Directions");
            DropIndex("dbo.Squads", new[] { "DirectionId" });
            AlterColumn("dbo.People", "DateofIssue", c => c.DateTime());
            DropColumn("dbo.Squads", "DirectionId");
            DropColumn("dbo.People", "PassportGiven");
            DropColumn("dbo.People", "PassportSerie");
            DropTable("dbo.Directions");
        }
    }
}
