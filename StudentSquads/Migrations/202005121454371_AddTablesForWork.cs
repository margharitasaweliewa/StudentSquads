namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTablesForWork : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Employers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Director = c.String(),
                        MainDocument = c.String(),
                        OfficialName = c.String(),
                        PhoneNumber = c.String(),
                        ContactPerson = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WorkProjects",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Place = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            
            
        }
        
        public override void Down()
        {
           
            DropTable("dbo.WorkProjects");
            DropTable("dbo.Employers");
        }
    }
}
