namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadeApprovesByCommandStaffNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Members", "ApprovedByCommandStaff", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Members", "ApprovedByCommandStaff", c => c.Boolean(nullable: false));
        }
    }
}
