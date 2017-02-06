namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserCreateContractInTrialMemberShips : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrialMemberships", "userCreateContract", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrialMemberships", "userCreateContract");
        }
    }
}
