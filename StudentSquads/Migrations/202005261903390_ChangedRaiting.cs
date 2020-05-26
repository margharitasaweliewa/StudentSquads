namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedRaiting : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RaitingEventInfoFiles", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RaitingEventInfoFiles", "Description");
        }
    }
}
