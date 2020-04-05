namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConnectUserandPerson : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "PersonId", c => c.Guid(nullable: false));
            CreateIndex("dbo.AspNetUsers", "PersonId");
            AddForeignKey("dbo.AspNetUsers", "PersonId", "dbo.People", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "PersonId", "dbo.People");
            DropIndex("dbo.AspNetUsers", new[] { "PersonId" });
            DropColumn("dbo.AspNetUsers", "PersonId");
        }
    }
}
