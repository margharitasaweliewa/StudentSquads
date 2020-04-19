namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUniandRegionHeadQuarters : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RegionalHeadquarters",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Region = c.String(),
                        RegionNumber = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UniversityHeadquarters",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        University = c.String(),
                        UniversityNumber = c.Int(nullable: false),
                        RegionalHeadquarterId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RegionalHeadquarters", t => t.RegionalHeadquarterId, cascadeDelete: true)
                .Index(t => t.RegionalHeadquarterId);
            
            AddColumn("dbo.HeadsOfStudentSquads", "UniversityHeadquarterId", c => c.Guid());
            AddColumn("dbo.HeadsOfStudentSquads", "RegionalHeadquarterId", c => c.Guid());
            AddColumn("dbo.Squads", "UniversityHeadquarterId", c => c.Guid());
            CreateIndex("dbo.HeadsOfStudentSquads", "UniversityHeadquarterId");
            CreateIndex("dbo.HeadsOfStudentSquads", "RegionalHeadquarterId");
            CreateIndex("dbo.Squads", "UniversityHeadquarterId");
            AddForeignKey("dbo.HeadsOfStudentSquads", "RegionalHeadquarterId", "dbo.RegionalHeadquarters", "Id");
            AddForeignKey("dbo.Squads", "UniversityHeadquarterId", "dbo.UniversityHeadquarters", "Id");
            AddForeignKey("dbo.HeadsOfStudentSquads", "UniversityHeadquarterId", "dbo.UniversityHeadquarters", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HeadsOfStudentSquads", "UniversityHeadquarterId", "dbo.UniversityHeadquarters");
            DropForeignKey("dbo.Squads", "UniversityHeadquarterId", "dbo.UniversityHeadquarters");
            DropForeignKey("dbo.UniversityHeadquarters", "RegionalHeadquarterId", "dbo.RegionalHeadquarters");
            DropForeignKey("dbo.HeadsOfStudentSquads", "RegionalHeadquarterId", "dbo.RegionalHeadquarters");
            DropIndex("dbo.UniversityHeadquarters", new[] { "RegionalHeadquarterId" });
            DropIndex("dbo.Squads", new[] { "UniversityHeadquarterId" });
            DropIndex("dbo.HeadsOfStudentSquads", new[] { "RegionalHeadquarterId" });
            DropIndex("dbo.HeadsOfStudentSquads", new[] { "UniversityHeadquarterId" });
            DropColumn("dbo.Squads", "UniversityHeadquarterId");
            DropColumn("dbo.HeadsOfStudentSquads", "RegionalHeadquarterId");
            DropColumn("dbo.HeadsOfStudentSquads", "UniversityHeadquarterId");
            DropTable("dbo.UniversityHeadquarters");
            DropTable("dbo.RegionalHeadquarters");
        }
    }
}
