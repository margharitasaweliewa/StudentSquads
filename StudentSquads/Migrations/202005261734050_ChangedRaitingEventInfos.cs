namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedRaitingEventInfos : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RaitingEventInfoes", "MembershipCount", c => c.Int(nullable: false));
            AddColumn("dbo.RaitingEventInfoes", "CreateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.RaitingEventInfoes", "Approved", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RaitingEventInfoes", "Approved");
            DropColumn("dbo.RaitingEventInfoes", "CreateTime");
            DropColumn("dbo.RaitingEventInfoes", "MembershipCount");
        }
    }
}
