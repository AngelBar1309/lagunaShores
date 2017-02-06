namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingFKtoDepositsForTM : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Deposits", name: "TrialMemberships_trialMembershipID", newName: "trialMembershipID");
            RenameIndex(table: "dbo.Deposits", name: "IX_TrialMemberships_trialMembershipID", newName: "IX_trialMembershipID");
            AddColumn("dbo.SalesContracts", "numberPayments", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalesContracts", "numberPayments");
            RenameIndex(table: "dbo.Deposits", name: "IX_trialMembershipID", newName: "IX_TrialMemberships_trialMembershipID");
            RenameColumn(table: "dbo.Deposits", name: "trialMembershipID", newName: "TrialMemberships_trialMembershipID");
        }
    }
}
