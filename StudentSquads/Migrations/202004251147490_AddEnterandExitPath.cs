namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEnterandExitPath : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "FIOinGenetiv", c => c.String());
            AddColumn("dbo.People", "EnterDocumentPath", c => c.String());
            AddColumn("dbo.People", "ExitDocumentPath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "ExitDocumentPath");
            DropColumn("dbo.People", "EnterDocumentPath");
            DropColumn("dbo.People", "FIOinGenetiv");
        }
    }
}
