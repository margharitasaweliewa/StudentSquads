namespace StudentSquads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddedUserIdtoPerson : DbMigration
    {
        public override void Up()
        { Sql(@"
UPDATE [dbo].[People] SET [ApplicationUserId] = N'027a16d1-8330-4fbc-a1eb-9e30c9afa387' WHERE [Id] = N'58553c51-bc92-4f22-a9a7-2f6dac019e88'
UPDATE [dbo].[People] SET [ApplicationUserId] = N'f2c7b26d-9998-4503-a2be-74cb14604784' WHERE [Id] = N'9b3d6c09-7f4a-4918-8211-4707be45fb7a'
UPDATE [dbo].[People] SET [ApplicationUserId] = N'6032ee65-82e2-4f79-9445-9e2ef7dad64a' WHERE [Id] = N'7a0a8bd2-2467-4eba-b7e1-4d22c680fa41'
UPDATE [dbo].[People] SET [ApplicationUserId] = N'5aceb78c-aa5a-42bd-ba07-1bee20b9e8ef' WHERE [Id] = N'9010b15c-e76b-41aa-8913-7e05c6ab37ff'
"); 
        }

        public override void Down()
        {
        }
    }
}
