namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedStatus : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Members", "StatusId", c => c.Int());
            CreateIndex("dbo.Members", "StatusId");
            AddForeignKey("dbo.Members", "StatusId", "dbo.Status", "Id");
            DropColumn("dbo.Members", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Members", "Status", c => c.String());
            DropForeignKey("dbo.Members", "StatusId", "dbo.Status");
            DropIndex("dbo.Members", new[] { "StatusId" });
            DropColumn("dbo.Members", "StatusId");
            DropTable("dbo.Status");
        }
    }
}
