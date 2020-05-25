namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedParametsinRaitingSection : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RaitingSections", "Coef", c => c.Double(nullable: false));
            AddColumn("dbo.RaitingSections", "Removed", c => c.Boolean(nullable: false));
            DropColumn("dbo.RaitingSections", "CountType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RaitingSections", "CountType", c => c.String());
            DropColumn("dbo.RaitingSections", "Removed");
            DropColumn("dbo.RaitingSections", "Coef");
        }
    }
}
