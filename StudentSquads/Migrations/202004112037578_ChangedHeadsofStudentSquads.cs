namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedHeadsofStudentSquads : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.HeadsOfStudentSquads", "PersonId", "dbo.People");
            DropIndex("dbo.HeadsOfStudentSquads", new[] { "PersonId" });
            AlterColumn("dbo.HeadsOfStudentSquads", "PersonId", c => c.Guid());
            CreateIndex("dbo.HeadsOfStudentSquads", "PersonId");
            AddForeignKey("dbo.HeadsOfStudentSquads", "PersonId", "dbo.People", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HeadsOfStudentSquads", "PersonId", "dbo.People");
            DropIndex("dbo.HeadsOfStudentSquads", new[] { "PersonId" });
            AlterColumn("dbo.HeadsOfStudentSquads", "PersonId", c => c.Guid(nullable: false));
            CreateIndex("dbo.HeadsOfStudentSquads", "PersonId");
            AddForeignKey("dbo.HeadsOfStudentSquads", "PersonId", "dbo.People", "Id", cascadeDelete: true);
        }
    }
}
