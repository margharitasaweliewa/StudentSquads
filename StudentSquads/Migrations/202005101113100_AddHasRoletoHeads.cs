namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHasRoletoHeads : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HeadsOfStudentSquads", "HasRole", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.HeadsOfStudentSquads", "HasRole");
        }
    }
}
