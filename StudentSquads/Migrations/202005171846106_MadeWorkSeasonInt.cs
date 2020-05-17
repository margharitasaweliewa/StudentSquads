namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadeWorkSeasonInt : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Works", "Season", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Works", "Season", c => c.String());
        }
    }
}
