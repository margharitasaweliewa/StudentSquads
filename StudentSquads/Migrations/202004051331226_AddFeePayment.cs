namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFeePayment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FeePayments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DateofPayment = c.DateTime(nullable: false),
                        SumofPayment = c.Int(nullable: false),
                        PersonId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.People", t => t.PersonId, cascadeDelete: true)
                .Index(t => t.PersonId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FeePayments", "PersonId", "dbo.People");
            DropIndex("dbo.FeePayments", new[] { "PersonId" });
            DropTable("dbo.FeePayments");
        }
    }
}
