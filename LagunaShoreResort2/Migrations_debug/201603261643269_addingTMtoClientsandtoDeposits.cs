namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingTMtoClientsandtoDeposits : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Deposits", "trialMembershipID", c => c.Int());
            CreateIndex("dbo.Deposits", "trialMembershipID");
            AddForeignKey("dbo.Deposits", "trialMembershipID", "dbo.TrialMemberships", "trialMembershipID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Deposits", "trialMembershipID", "dbo.TrialMemberships");
            DropIndex("dbo.Deposits", new[] { "trialMembershipID" });
            DropColumn("dbo.Deposits", "trialMembershipID");
        }
    }
}
