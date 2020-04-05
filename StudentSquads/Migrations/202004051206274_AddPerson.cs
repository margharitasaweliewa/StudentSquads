namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPerson : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.People",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        LastName = c.String(),
                        FirstName = c.String(),
                        PatronymicName = c.String(),
                        Sex = c.Boolean(),
                        DateofBirth = c.DateTime(),
                        INN = c.String(),
                        Snils = c.String(),
                        PhoneNumber = c.String(),
                        Email = c.String(),
                        PasportSerie = c.String(),
                        PassportNumber = c.String(),
                        DateofIssue = c.DateTime(),
                        DepartmentCode = c.String(),
                        CityofBirth = c.String(),
                        RegistrationPlace = c.String(),
                        DateOfEnter = c.DateTime(),
                        DateOfExit = c.DateTime(),
                        MembershipNumber = c.String(),
                        ApplicationFrom = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Members", "PersonId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Members", "PersonId");
            AddForeignKey("dbo.Members", "PersonId", "dbo.People", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Members", "PersonId", "dbo.People");
            DropIndex("dbo.Members", new[] { "PersonId" });
            DropColumn("dbo.Members", "PersonId");
            DropTable("dbo.People");
        }
    }
}
