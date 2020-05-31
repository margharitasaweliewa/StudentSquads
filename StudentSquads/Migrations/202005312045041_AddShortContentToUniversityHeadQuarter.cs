namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddShortContentToUniversityHeadQuarter : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UniversityHeadquarters", "ShortContent", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UniversityHeadquarters", "ShortContent");
        }
    }
}
