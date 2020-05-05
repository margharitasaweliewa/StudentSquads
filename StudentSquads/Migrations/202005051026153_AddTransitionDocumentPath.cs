namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTransitionDocumentPath : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Members", "TransitionDocumentPath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Members", "TransitionDocumentPath");
        }
    }
}
