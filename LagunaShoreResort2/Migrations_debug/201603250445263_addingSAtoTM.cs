namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingSAtoTM : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrialMemberships", "trialExpDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.TrialMemberships", "advantageWeeks", c => c.Double(nullable: false));
            AddColumn("dbo.TrialMemberships", "advantageExpDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.TrialMemberships", "quickWeeks", c => c.Double(nullable: false));
            AddColumn("dbo.TrialMemberships", "quickWeeksExpDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrialMemberships", "quickWeeksExpDate");
            DropColumn("dbo.TrialMemberships", "quickWeeks");
            DropColumn("dbo.TrialMemberships", "advantageExpDate");
            DropColumn("dbo.TrialMemberships", "advantageWeeks");
            DropColumn("dbo.TrialMemberships", "trialExpDate");
        }
    }
}
