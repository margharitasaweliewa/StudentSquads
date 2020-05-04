namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddApplicationStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Members", "ApplicationStatus", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Members", "ApplicationStatus");
        }
    }
}
