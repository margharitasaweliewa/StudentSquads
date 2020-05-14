namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAudit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WorkChangeTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
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

            AddColumn("dbo.Works", "CreateTima", c => c.DateTime(nullable: false));
            AddColumn("dbo.Works", "OriginalWorkId", c => c.Guid());
            AddColumn("dbo.Works", "Approved", c => c.Boolean());
            AddColumn("dbo.Works", "ExitReason", c => c.String());
            AddColumn("dbo.Works", "WorkChangeTypeId", c => c.Int());
            CreateIndex("dbo.Works", "OriginalWorkId");
            CreateIndex("dbo.Works", "WorkChangeTypeId");
            AddForeignKey("dbo.Works", "OriginalWorkId", "dbo.Works", "Id");
            AddForeignKey("dbo.Works", "WorkChangeTypeId", "dbo.WorkChangeTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Works", "WorkChangeTypeId", "dbo.WorkChangeTypes");
            DropForeignKey("dbo.Works", "OriginalWorkId", "dbo.Works");
            DropIndex("dbo.Works", new[] { "WorkChangeTypeId" });
            DropIndex("dbo.Works", new[] { "OriginalWorkId" });
            DropColumn("dbo.Works", "WorkChangeTypeId");
            DropColumn("dbo.Works", "ExitReason");
            DropColumn("dbo.Works", "Approved");
            DropColumn("dbo.Works", "OriginalWorkId");
            DropColumn("dbo.Works", "CreateTima");
            DropTable("dbo.WorkChangeTypes");
            DropForeignKey("dbo.Works", "WorkProjectId", "dbo.WorkProjects");
            DropForeignKey("dbo.Works", "MemberId", "dbo.Members");
            DropForeignKey("dbo.Works", "EmployerId", "dbo.Employers");
            DropIndex("dbo.Works", new[] { "EmployerId" });
            DropIndex("dbo.Works", new[] { "MemberId" });
            DropIndex("dbo.Works", new[] { "WorkProjectId" });
            DropTable("dbo.Works");
        }
    }
}
