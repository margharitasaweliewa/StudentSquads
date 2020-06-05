namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadePersonRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.People", "INN", c => c.String(nullable: false));
            AlterColumn("dbo.People", "Snils", c => c.String(nullable: false));
            AlterColumn("dbo.People", "PhoneNumber", c => c.String(nullable: false));
            AlterColumn("dbo.People", "Email", c => c.String(nullable: false));
            AlterColumn("dbo.People", "PassportSerie", c => c.String(nullable: false));
            AlterColumn("dbo.People", "PassportNumber", c => c.String(nullable: false));
            AlterColumn("dbo.People", "PassportGiven", c => c.String(nullable: false));
            AlterColumn("dbo.People", "DepartmentCode", c => c.String(nullable: false));
            AlterColumn("dbo.People", "CityofBirth", c => c.String(nullable: false));
            AlterColumn("dbo.People", "RegistrationPlace", c => c.String(nullable: false));
            AlterColumn("dbo.People", "FIOinGenetiv", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.People", "FIOinGenetiv", c => c.String());
            AlterColumn("dbo.People", "RegistrationPlace", c => c.String());
            AlterColumn("dbo.People", "CityofBirth", c => c.String());
            AlterColumn("dbo.People", "DepartmentCode", c => c.String());
            AlterColumn("dbo.People", "PassportGiven", c => c.String());
            AlterColumn("dbo.People", "PassportNumber", c => c.String());
            AlterColumn("dbo.People", "PassportSerie", c => c.String());
            AlterColumn("dbo.People", "Email", c => c.String());
            AlterColumn("dbo.People", "PhoneNumber", c => c.String());
            AlterColumn("dbo.People", "Snils", c => c.String());
            AlterColumn("dbo.People", "INN", c => c.String());
        }
    }
}
