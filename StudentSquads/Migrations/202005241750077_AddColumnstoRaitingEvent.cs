namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnstoRaitingEvent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EventLevels", "Name", c => c.String());
            AddColumn("dbo.RaitingEvents", "Approved", c => c.Boolean());
            AddColumn("dbo.RaitingEvents", "DateofBegin", c => c.DateTime(nullable: false));
            AddColumn("dbo.RaitingEvents", "DateofEnd", c => c.DateTime(nullable: false));
            AddColumn("dbo.RaitingEvents", "DocumentPath", c => c.String());
            DropColumn("dbo.EventLevels", "NewName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EventLevels", "NewName", c => c.String());
            DropColumn("dbo.RaitingEvents", "DocumentPath");
            DropColumn("dbo.RaitingEvents", "DateofEnd");
            DropColumn("dbo.RaitingEvents", "DateofBegin");
            DropColumn("dbo.RaitingEvents", "Approved");
            DropColumn("dbo.EventLevels", "Name");
        }
    }
}
