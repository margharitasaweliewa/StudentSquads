namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAllTables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Works", "CreateTime", c => c.DateTime(nullable: false));
            DropColumn("dbo.Works", "CreateTima");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Works", "CreateTima", c => c.DateTime(nullable: false));
            DropColumn("dbo.Works", "CreateTime");
        }
    }
}
