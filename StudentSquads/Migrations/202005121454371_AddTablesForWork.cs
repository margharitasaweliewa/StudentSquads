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
            
            CreateTable(
                "dbo.Works",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Affirmed = c.Boolean(),
                        Rebuke = c.String(),
                        DUT = c.Int(),
                        DateofBegin = c.DateTime(nullable: false),
                        DateofEnd = c.DateTime(nullable: false),
                        Alternative = c.Boolean(nullable: false),
                        AlternativeReason = c.String(),
                        Season = c.String(),
                        WorkProjectId = c.Guid(),
                        MemberId = c.Guid(nullable: false),
                        EmployerId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employers", t => t.EmployerId, cascadeDelete: true)
                .ForeignKey("dbo.Members", t => t.MemberId, cascadeDelete: true)
                .ForeignKey("dbo.WorkProjects", t => t.WorkProjectId)
                .Index(t => t.WorkProjectId)
                .Index(t => t.MemberId)
                .Index(t => t.EmployerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Works", "WorkProjectId", "dbo.WorkProjects");
            DropForeignKey("dbo.Works", "MemberId", "dbo.Members");
            DropForeignKey("dbo.Works", "EmployerId", "dbo.Employers");
            DropIndex("dbo.Works", new[] { "EmployerId" });
            DropIndex("dbo.Works", new[] { "MemberId" });
            DropIndex("dbo.Works", new[] { "WorkProjectId" });
            DropTable("dbo.Works");
            DropTable("dbo.WorkProjects");
            DropTable("dbo.Employers");
        }
    }
}
