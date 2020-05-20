namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRemovedinWork : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Works", "Removed", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Works", "Removed");
        }
    }
}
