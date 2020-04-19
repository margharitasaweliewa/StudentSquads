namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadeNumbersNullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UniversityHeadquarters", "RegionalHeadquarterId", "dbo.RegionalHeadquarters");
            DropIndex("dbo.UniversityHeadquarters", new[] { "RegionalHeadquarterId" });
            AlterColumn("dbo.RegionalHeadquarters", "RegionNumber", c => c.Int());
            AlterColumn("dbo.UniversityHeadquarters", "UniversityNumber", c => c.Int());
            AlterColumn("dbo.UniversityHeadquarters", "RegionalHeadquarterId", c => c.Guid());
            CreateIndex("dbo.UniversityHeadquarters", "RegionalHeadquarterId");
            AddForeignKey("dbo.UniversityHeadquarters", "RegionalHeadquarterId", "dbo.RegionalHeadquarters", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UniversityHeadquarters", "RegionalHeadquarterId", "dbo.RegionalHeadquarters");
            DropIndex("dbo.UniversityHeadquarters", new[] { "RegionalHeadquarterId" });
            AlterColumn("dbo.UniversityHeadquarters", "RegionalHeadquarterId", c => c.Guid(nullable: false));
            AlterColumn("dbo.UniversityHeadquarters", "UniversityNumber", c => c.Int(nullable: false));
            AlterColumn("dbo.RegionalHeadquarters", "RegionNumber", c => c.Int(nullable: false));
            CreateIndex("dbo.UniversityHeadquarters", "RegionalHeadquarterId");
            AddForeignKey("dbo.UniversityHeadquarters", "RegionalHeadquarterId", "dbo.RegionalHeadquarters", "Id", cascadeDelete: true);
        }
    }
}
