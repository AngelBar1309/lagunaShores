namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class trialMembershipsAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TrialMemberships",
                c => new
                    {
                        trialMembershipID = c.Int(nullable: false, identity: true),
                        contractNumberTM = c.String(),
                        contractType = c.String(),
                        tmContractDate = c.DateTime(nullable: false),
                        tmSaleAmount = c.Double(nullable: false),
                        tmInterestRate = c.Double(nullable: false),
                        tmVerifiedByAdmin = c.Boolean(nullable: false),
                        tmVerificationDate = c.DateTime(nullable: false),
                        tmRequestToAccountat = c.Boolean(nullable: false),
                        tmRequestToAccountantDate = c.DateTime(nullable: false),
                        trialNights = c.Double(nullable: false),
                        tmCanceledContract = c.Boolean(nullable: false),
                        tmCanceledContractDate = c.DateTime(nullable: false),
                        tmPaymentsStartDate = c.DateTime(nullable: false),
                        tmNumberofDownPayments = c.Int(nullable: false),
                        tmNumberPayments = c.Int(nullable: false),
                        tmQualification = c.String(),
                        tmCurrency = c.String(),
                        tmListPrice = c.Double(nullable: false),
                        tmCommissionPaid = c.Boolean(nullable: false),
                        tmCommissionPaidDate = c.DateTime(nullable: false),
                        tmPaymentsQuantity = c.Int(nullable: false),
                        tmRoomType = c.String(),
                        tmComments = c.String(),
                        clientID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.trialMembershipID)
                .ForeignKey("dbo.Clients", t => t.clientID, cascadeDelete: true)
                .Index(t => t.clientID);
            
            AddColumn("dbo.ContractSalesMembers", "TrialMemberships_trialMembershipID", c => c.Int());
            AddColumn("dbo.Deposits", "TrialMemberships_trialMembershipID", c => c.Int());
            CreateIndex("dbo.ContractSalesMembers", "TrialMemberships_trialMembershipID");
            CreateIndex("dbo.Deposits", "TrialMemberships_trialMembershipID");
            AddForeignKey("dbo.ContractSalesMembers", "TrialMemberships_trialMembershipID", "dbo.TrialMemberships", "trialMembershipID");
            AddForeignKey("dbo.Deposits", "TrialMemberships_trialMembershipID", "dbo.TrialMemberships", "trialMembershipID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Deposits", "TrialMemberships_trialMembershipID", "dbo.TrialMemberships");
            DropForeignKey("dbo.ContractSalesMembers", "TrialMemberships_trialMembershipID", "dbo.TrialMemberships");
            DropForeignKey("dbo.TrialMemberships", "clientID", "dbo.Clients");
            DropIndex("dbo.TrialMemberships", new[] { "clientID" });
            DropIndex("dbo.Deposits", new[] { "TrialMemberships_trialMembershipID" });
            DropIndex("dbo.ContractSalesMembers", new[] { "TrialMemberships_trialMembershipID" });
            DropColumn("dbo.Deposits", "TrialMemberships_trialMembershipID");
            DropColumn("dbo.ContractSalesMembers", "TrialMemberships_trialMembershipID");
            DropTable("dbo.TrialMemberships");
        }
    }
}
