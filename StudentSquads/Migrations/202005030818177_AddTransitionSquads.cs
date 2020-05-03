namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTransitionSquads : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Members", "FromSquadId", c => c.Guid());
            AddColumn("dbo.Members", "ToSquadId", c => c.Guid());
            CreateIndex("dbo.Members", "FromSquadId");
            CreateIndex("dbo.Members", "ToSquadId");
            AddForeignKey("dbo.Members", "FromSquadId", "dbo.Squads", "Id");
            AddForeignKey("dbo.Members", "ToSquadId", "dbo.Squads", "Id");
            DropColumn("dbo.Members", "DateOfTransition");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Members", "DateOfTransition", c => c.DateTime());
            DropForeignKey("dbo.Members", "ToSquadId", "dbo.Squads");
            DropForeignKey("dbo.Members", "FromSquadId", "dbo.Squads");
            DropIndex("dbo.Members", new[] { "ToSquadId" });
            DropIndex("dbo.Members", new[] { "FromSquadId" });
            DropColumn("dbo.Members", "ToSquadId");
            DropColumn("dbo.Members", "FromSquadId");
        }
    }
}
