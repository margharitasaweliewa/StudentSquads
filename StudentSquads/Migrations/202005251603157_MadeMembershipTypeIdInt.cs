namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadeMembershipTypeIdInt : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RaitingSections", "MembershipType_Id", "dbo.MembershipTypes");
            DropIndex("dbo.RaitingSections", new[] { "MembershipType_Id" });
            DropColumn("dbo.RaitingSections", "MembershipTypeId");
            RenameColumn(table: "dbo.RaitingSections", name: "MembershipType_Id", newName: "MembershipTypeId");
            AlterColumn("dbo.RaitingSections", "MembershipTypeId", c => c.Int(nullable: false));
            AlterColumn("dbo.RaitingSections", "MembershipTypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.RaitingSections", "MembershipTypeId");
            AddForeignKey("dbo.RaitingSections", "MembershipTypeId", "dbo.MembershipTypes", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RaitingSections", "MembershipTypeId", "dbo.MembershipTypes");
            DropIndex("dbo.RaitingSections", new[] { "MembershipTypeId" });
            AlterColumn("dbo.RaitingSections", "MembershipTypeId", c => c.Int());
            AlterColumn("dbo.RaitingSections", "MembershipTypeId", c => c.Guid(nullable: false));
            RenameColumn(table: "dbo.RaitingSections", name: "MembershipTypeId", newName: "MembershipType_Id");
            AddColumn("dbo.RaitingSections", "MembershipTypeId", c => c.Guid(nullable: false));
            CreateIndex("dbo.RaitingSections", "MembershipType_Id");
            AddForeignKey("dbo.RaitingSections", "MembershipType_Id", "dbo.MembershipTypes", "Id");
        }
    }
}
