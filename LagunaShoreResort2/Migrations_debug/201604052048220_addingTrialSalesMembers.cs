namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingTrialSalesMembers : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ContractSalesMembers", "TrialMemberships_trialMembershipID", "dbo.TrialMemberships");
            DropIndex("dbo.ContractSalesMembers", new[] { "TrialMemberships_trialMembershipID" });
            CreateTable(
                "dbo.TrialSalesMembers",
                c => new
                    {
                        trialSalesMemberID = c.Int(nullable: false, identity: true),
                        trialContractID = c.Int(),
                        salesMemberID = c.Int(nullable: false),
                        rolID = c.Int(nullable: false),
                        trialMemberships_trialMembershipID = c.Int(),
                    })
                .PrimaryKey(t => t.trialSalesMemberID)
                .ForeignKey("dbo.Rols", t => t.rolID, cascadeDelete: true)
                .ForeignKey("dbo.SalesMembers", t => t.salesMemberID, cascadeDelete: true)
                .ForeignKey("dbo.TrialMemberships", t => t.trialMemberships_trialMembershipID)
                .Index(t => t.salesMemberID)
                .Index(t => t.rolID)
                .Index(t => t.trialMemberships_trialMembershipID);
            
            DropColumn("dbo.ContractSalesMembers", "TrialMemberships_trialMembershipID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ContractSalesMembers", "TrialMemberships_trialMembershipID", c => c.Int());
            DropForeignKey("dbo.TrialSalesMembers", "trialMemberships_trialMembershipID", "dbo.TrialMemberships");
            DropForeignKey("dbo.TrialSalesMembers", "salesMemberID", "dbo.SalesMembers");
            DropForeignKey("dbo.TrialSalesMembers", "rolID", "dbo.Rols");
            DropIndex("dbo.TrialSalesMembers", new[] { "trialMemberships_trialMembershipID" });
            DropIndex("dbo.TrialSalesMembers", new[] { "rolID" });
            DropIndex("dbo.TrialSalesMembers", new[] { "salesMemberID" });
            DropTable("dbo.TrialSalesMembers");
            CreateIndex("dbo.ContractSalesMembers", "TrialMemberships_trialMembershipID");
            AddForeignKey("dbo.ContractSalesMembers", "TrialMemberships_trialMembershipID", "dbo.TrialMemberships", "trialMembershipID");
        }
    }
}
