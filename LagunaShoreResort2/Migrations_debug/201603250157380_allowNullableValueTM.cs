namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class allowNullableValueTM : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TrialMemberships", "tmVerifiedByAdmin", c => c.Boolean());
            AlterColumn("dbo.TrialMemberships", "tmVerificationDate", c => c.DateTime());
            AlterColumn("dbo.TrialMemberships", "tmRequestToAccountat", c => c.Boolean());
            AlterColumn("dbo.TrialMemberships", "tmRequestToAccountantDate", c => c.DateTime());
            AlterColumn("dbo.TrialMemberships", "tmCommissionPaid", c => c.Boolean());
            AlterColumn("dbo.TrialMemberships", "tmCommissionPaidDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TrialMemberships", "tmCommissionPaidDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TrialMemberships", "tmCommissionPaid", c => c.Boolean(nullable: false));
            AlterColumn("dbo.TrialMemberships", "tmRequestToAccountantDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TrialMemberships", "tmRequestToAccountat", c => c.Boolean(nullable: false));
            AlterColumn("dbo.TrialMemberships", "tmVerificationDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TrialMemberships", "tmVerifiedByAdmin", c => c.Boolean(nullable: false));
        }
    }
}
