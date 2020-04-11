namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadeDateofBirthRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.People", "DateofBirth", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.People", "DateofBirth", c => c.DateTime());
        }
    }
}
