namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMainPosition : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MainPositions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.HeadsOfStudentSquads", "MainPositionId", c => c.Int());
            CreateIndex("dbo.HeadsOfStudentSquads", "MainPositionId");
            AddForeignKey("dbo.HeadsOfStudentSquads", "MainPositionId", "dbo.MainPositions", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HeadsOfStudentSquads", "MainPositionId", "dbo.MainPositions");
            DropIndex("dbo.HeadsOfStudentSquads", new[] { "MainPositionId" });
            DropColumn("dbo.HeadsOfStudentSquads", "MainPositionId");
            DropTable("dbo.MainPositions");
        }
    }
}
