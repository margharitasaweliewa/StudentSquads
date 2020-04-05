namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedNamesandSexNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.People", "Sex", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.People", "Sex", c => c.Boolean());
        }
    }
}
