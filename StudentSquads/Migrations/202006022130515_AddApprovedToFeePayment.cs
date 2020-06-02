namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddApprovedToFeePayment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FeePayments", "Approved", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FeePayments", "Approved");
        }
    }
}
