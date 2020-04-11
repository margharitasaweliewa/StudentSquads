namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewFieldsInPerson : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "PlaceofStudy", c => c.String());
            AddColumn("dbo.People", "FIO", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "FIO");
            DropColumn("dbo.People", "PlaceofStudy");
        }
    }
}
