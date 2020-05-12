namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadeHasRoleObligatory : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.HeadsOfStudentSquads", "HasRole", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.HeadsOfStudentSquads", "HasRole", c => c.Boolean());
        }
    }
}
