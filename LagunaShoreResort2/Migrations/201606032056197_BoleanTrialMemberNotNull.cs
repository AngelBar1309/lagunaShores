namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BoleanTrialMemberNotNull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TrialMemberships", "tmVerifiedByAdmin", c => c.Boolean(nullable: false));
            AlterColumn("dbo.TrialMemberships", "tmCanceledContract", c => c.Boolean(nullable: false));
            AlterColumn("dbo.TrialMemberships", "tmCommissionPaid", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TrialMemberships", "tmCommissionPaid", c => c.Boolean());
            AlterColumn("dbo.TrialMemberships", "tmCanceledContract", c => c.Boolean());
            AlterColumn("dbo.TrialMemberships", "tmVerifiedByAdmin", c => c.Boolean());
        }
    }
}
