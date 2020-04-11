namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedUser : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "PersonId", "dbo.People");
            DropIndex("dbo.AspNetUsers", new[] { "PersonId" });
            AddColumn("dbo.People", "ApplicationUserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.People", "ApplicationUserId");
            AddForeignKey("dbo.People", "ApplicationUserId", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.AspNetUsers", "PersonId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "PersonId", c => c.Guid());
            DropForeignKey("dbo.People", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.People", new[] { "ApplicationUserId" });
            DropColumn("dbo.People", "ApplicationUserId");
            CreateIndex("dbo.AspNetUsers", "PersonId");
            AddForeignKey("dbo.AspNetUsers", "PersonId", "dbo.People", "Id");
        }
    }
}
