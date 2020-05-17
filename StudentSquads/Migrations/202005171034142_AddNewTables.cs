namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EventLevels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NewName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MembershipTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Professions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProfessionSchools",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProfessionId = c.Guid(nullable: false),
                        DateofBegin = c.DateTime(nullable: false),
                        DateofEnd = c.DateTime(nullable: false),
                        Name = c.String(),
                        Profession_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Professions", t => t.Profession_Id)
                .Index(t => t.Profession_Id);
            
            CreateTable(
                "dbo.RaitingEventInfoFiles",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RaitingEventInfoId = c.Guid(nullable: false),
                        Path = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RaitingEventInfoes", t => t.RaitingEventInfoId, cascadeDelete: true)
                .Index(t => t.RaitingEventInfoId);
            
            CreateTable(
                "dbo.RaitingEventInfoes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SquadId = c.Guid(nullable: false),
                        RaitingSectionId = c.Guid(nullable: false),
                        RaitingEventId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RaitingEvents", t => t.RaitingEventId, cascadeDelete: true)
                .ForeignKey("dbo.RaitingSections", t => t.RaitingSectionId, cascadeDelete: true)
                .ForeignKey("dbo.Squads", t => t.SquadId, cascadeDelete: true)
                .Index(t => t.SquadId)
                .Index(t => t.RaitingSectionId)
                .Index(t => t.RaitingEventId);
            
            CreateTable(
                "dbo.RaitingEvents",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        EventLevelId = c.Guid(nullable: false),
                        SquadId = c.Guid(),
                        UniversityHeadquarterId = c.Guid(),
                        RegionalHeadquarterId = c.Guid(),
                        RaitingId = c.Guid(nullable: false),
                        EventLevel_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EventLevels", t => t.EventLevel_Id)
                .ForeignKey("dbo.Raitings", t => t.RaitingId, cascadeDelete: true)
                .ForeignKey("dbo.RegionalHeadquarters", t => t.RegionalHeadquarterId)
                .ForeignKey("dbo.Squads", t => t.SquadId)
                .ForeignKey("dbo.UniversityHeadquarters", t => t.UniversityHeadquarterId)
                .Index(t => t.SquadId)
                .Index(t => t.UniversityHeadquarterId)
                .Index(t => t.RegionalHeadquarterId)
                .Index(t => t.RaitingId)
                .Index(t => t.EventLevel_Id);
            
            CreateTable(
                "dbo.Raitings",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DateofBegin = c.DateTime(nullable: false),
                        DateofEnd = c.DateTime(),
                        DateofCreation = c.DateTime(),
                        Comment = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RaitingSections",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        CountTyoe = c.String(),
                        EventLevelId = c.Guid(nullable: false),
                        MembershipTypeId = c.Guid(nullable: false),
                        EventLevel_Id = c.Int(),
                        MembershipType_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EventLevels", t => t.EventLevel_Id)
                .ForeignKey("dbo.MembershipTypes", t => t.MembershipType_Id)
                .Index(t => t.EventLevel_Id)
                .Index(t => t.MembershipType_Id);
            
            CreateTable(
                "dbo.RaitingPlaces",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RaitingId = c.Guid(nullable: false),
                        RaitingSectionId = c.Guid(nullable: false),
                        SquadId = c.Guid(nullable: false),
                        Place = c.Int(nullable: false),
                        MainPlace = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Raitings", t => t.RaitingId, cascadeDelete: true)
                .ForeignKey("dbo.RaitingSections", t => t.RaitingSectionId, cascadeDelete: true)
                .ForeignKey("dbo.Squads", t => t.SquadId, cascadeDelete: true)
                .Index(t => t.RaitingId)
                .Index(t => t.RaitingSectionId)
                .Index(t => t.SquadId);
            
            CreateTable(
                "dbo.ScholAttendings",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PersonId = c.Guid(nullable: false),
                        ProfessionSchoolId = c.Guid(nullable: false),
                        CertificateNumber = c.String(),
                        Succeed = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.People", t => t.PersonId, cascadeDelete: true)
                .ForeignKey("dbo.ProfessionSchools", t => t.ProfessionSchoolId, cascadeDelete: true)
                .Index(t => t.PersonId)
                .Index(t => t.ProfessionSchoolId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ScholAttendings", "ProfessionSchoolId", "dbo.ProfessionSchools");
            DropForeignKey("dbo.ScholAttendings", "PersonId", "dbo.People");
            DropForeignKey("dbo.RaitingPlaces", "SquadId", "dbo.Squads");
            DropForeignKey("dbo.RaitingPlaces", "RaitingSectionId", "dbo.RaitingSections");
            DropForeignKey("dbo.RaitingPlaces", "RaitingId", "dbo.Raitings");
            DropForeignKey("dbo.RaitingEventInfoFiles", "RaitingEventInfoId", "dbo.RaitingEventInfoes");
            DropForeignKey("dbo.RaitingEventInfoes", "SquadId", "dbo.Squads");
            DropForeignKey("dbo.RaitingEventInfoes", "RaitingSectionId", "dbo.RaitingSections");
            DropForeignKey("dbo.RaitingSections", "MembershipType_Id", "dbo.MembershipTypes");
            DropForeignKey("dbo.RaitingSections", "EventLevel_Id", "dbo.EventLevels");
            DropForeignKey("dbo.RaitingEventInfoes", "RaitingEventId", "dbo.RaitingEvents");
            DropForeignKey("dbo.RaitingEvents", "UniversityHeadquarterId", "dbo.UniversityHeadquarters");
            DropForeignKey("dbo.RaitingEvents", "SquadId", "dbo.Squads");
            DropForeignKey("dbo.RaitingEvents", "RegionalHeadquarterId", "dbo.RegionalHeadquarters");
            DropForeignKey("dbo.RaitingEvents", "RaitingId", "dbo.Raitings");
            DropForeignKey("dbo.RaitingEvents", "EventLevel_Id", "dbo.EventLevels");
            DropForeignKey("dbo.ProfessionSchools", "Profession_Id", "dbo.Professions");
            DropIndex("dbo.ScholAttendings", new[] { "ProfessionSchoolId" });
            DropIndex("dbo.ScholAttendings", new[] { "PersonId" });
            DropIndex("dbo.RaitingPlaces", new[] { "SquadId" });
            DropIndex("dbo.RaitingPlaces", new[] { "RaitingSectionId" });
            DropIndex("dbo.RaitingPlaces", new[] { "RaitingId" });
            DropIndex("dbo.RaitingSections", new[] { "MembershipType_Id" });
            DropIndex("dbo.RaitingSections", new[] { "EventLevel_Id" });
            DropIndex("dbo.RaitingEvents", new[] { "EventLevel_Id" });
            DropIndex("dbo.RaitingEvents", new[] { "RaitingId" });
            DropIndex("dbo.RaitingEvents", new[] { "RegionalHeadquarterId" });
            DropIndex("dbo.RaitingEvents", new[] { "UniversityHeadquarterId" });
            DropIndex("dbo.RaitingEvents", new[] { "SquadId" });
            DropIndex("dbo.RaitingEventInfoes", new[] { "RaitingEventId" });
            DropIndex("dbo.RaitingEventInfoes", new[] { "RaitingSectionId" });
            DropIndex("dbo.RaitingEventInfoes", new[] { "SquadId" });
            DropIndex("dbo.RaitingEventInfoFiles", new[] { "RaitingEventInfoId" });
            DropIndex("dbo.ProfessionSchools", new[] { "Profession_Id" });
            DropTable("dbo.ScholAttendings");
            DropTable("dbo.RaitingPlaces");
            DropTable("dbo.RaitingSections");
            DropTable("dbo.Raitings");
            DropTable("dbo.RaitingEvents");
            DropTable("dbo.RaitingEventInfoes");
            DropTable("dbo.RaitingEventInfoFiles");
            DropTable("dbo.ProfessionSchools");
            DropTable("dbo.Professions");
            DropTable("dbo.MembershipTypes");
            DropTable("dbo.EventLevels");
        }
    }
}
