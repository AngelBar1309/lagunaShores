namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class allowingNightsTobeNull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TrialMemberships", "trialNights", c => c.Double());
            AlterColumn("dbo.TrialMemberships", "trialExpDate", c => c.DateTime());
            AlterColumn("dbo.TrialMemberships", "advantageWeeks", c => c.Double());
            AlterColumn("dbo.TrialMemberships", "advantageExpDate", c => c.DateTime());
            AlterColumn("dbo.TrialMemberships", "quickWeeks", c => c.Double());
            AlterColumn("dbo.TrialMemberships", "quickWeeksExpDate", c => c.DateTime());
            AlterColumn("dbo.TrialMemberships", "tmCanceledContract", c => c.Boolean());
            AlterColumn("dbo.TrialMemberships", "tmCanceledContractDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TrialMemberships", "tmCanceledContractDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TrialMemberships", "tmCanceledContract", c => c.Boolean(nullable: false));
            AlterColumn("dbo.TrialMemberships", "quickWeeksExpDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TrialMemberships", "quickWeeks", c => c.Double(nullable: false));
            AlterColumn("dbo.TrialMemberships", "advantageExpDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TrialMemberships", "advantageWeeks", c => c.Double(nullable: false));
            AlterColumn("dbo.TrialMemberships", "trialExpDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TrialMemberships", "trialNights", c => c.Double(nullable: false));
        }
    }
}
