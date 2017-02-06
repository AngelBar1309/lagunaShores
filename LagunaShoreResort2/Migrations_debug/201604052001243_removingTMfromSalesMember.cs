namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removingTMfromSalesMember : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.ContractSalesMembers", name: "trialMembership_trialMembershipID", newName: "TrialMemberships_trialMembershipID");
            RenameIndex(table: "dbo.ContractSalesMembers", name: "IX_trialMembership_trialMembershipID", newName: "IX_TrialMemberships_trialMembershipID");
            DropColumn("dbo.ContractSalesMembers", "trialMembershipsID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ContractSalesMembers", "trialMembershipsID", c => c.Int());
            RenameIndex(table: "dbo.ContractSalesMembers", name: "IX_TrialMemberships_trialMembershipID", newName: "IX_trialMembership_trialMembershipID");
            RenameColumn(table: "dbo.ContractSalesMembers", name: "TrialMemberships_trialMembershipID", newName: "trialMembership_trialMembershipID");
        }
    }
}
