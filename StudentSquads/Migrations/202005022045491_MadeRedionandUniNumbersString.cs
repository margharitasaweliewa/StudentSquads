namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadeRedionandUniNumbersString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RegionalHeadquarters", "RegionNumber", c => c.String());
            AlterColumn("dbo.UniversityHeadquarters", "UniversityNumber", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UniversityHeadquarters", "UniversityNumber", c => c.Int());
            AlterColumn("dbo.RegionalHeadquarters", "RegionNumber", c => c.Int());
        }
    }
}
