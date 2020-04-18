namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddExitReasontoPerson : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "ExitReason", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "ExitReason");
        }
    }
}
