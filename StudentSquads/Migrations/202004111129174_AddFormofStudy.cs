namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFormofStudy : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "FormofStudy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "FormofStudy");
        }
    }
}
