namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingTMtoSalesMembers : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ContractSalesMembers", "salesContractID", "dbo.SalesContracts");
            DropIndex("dbo.ContractSalesMembers", new[] { "salesContractID" });
            RenameColumn(table: "dbo.ContractSalesMembers", name: "TrialMemberships_trialMembershipID", newName: "trialMembership_trialMembershipID");
            RenameIndex(table: "dbo.ContractSalesMembers", name: "IX_TrialMemberships_trialMembershipID", newName: "IX_trialMembership_trialMembershipID");
            AddColumn("dbo.ContractSalesMembers", "trialMembershipsID", c => c.Int());
            AlterColumn("dbo.ContractSalesMembers", "salesContractID", c => c.Int());
            CreateIndex("dbo.ContractSalesMembers", "salesContractID");
            AddForeignKey("dbo.ContractSalesMembers", "salesContractID", "dbo.SalesContracts", "salesContractID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ContractSalesMembers", "salesContractID", "dbo.SalesContracts");
            DropIndex("dbo.ContractSalesMembers", new[] { "salesContractID" });
            AlterColumn("dbo.ContractSalesMembers", "salesContractID", c => c.Int(nullable: false));
            DropColumn("dbo.ContractSalesMembers", "trialMembershipsID");
            RenameIndex(table: "dbo.ContractSalesMembers", name: "IX_trialMembership_trialMembershipID", newName: "IX_TrialMemberships_trialMembershipID");
            RenameColumn(table: "dbo.ContractSalesMembers", name: "trialMembership_trialMembershipID", newName: "TrialMemberships_trialMembershipID");
            CreateIndex("dbo.ContractSalesMembers", "salesContractID");
            AddForeignKey("dbo.ContractSalesMembers", "salesContractID", "dbo.SalesContracts", "salesContractID", cascadeDelete: true);
        }
    }
}
