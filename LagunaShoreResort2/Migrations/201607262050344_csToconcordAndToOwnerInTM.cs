namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class csToconcordAndToOwnerInTM : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrialMemberships", "csToOwner", c => c.Boolean(nullable: false));
            AddColumn("dbo.TrialMemberships", "csToOwnerDate", c => c.DateTime());
            AddColumn("dbo.TrialMemberships", "csToConcord", c => c.Boolean(nullable: false));
            AddColumn("dbo.TrialMemberships", "csToConcordDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrialMemberships", "csToConcordDate");
            DropColumn("dbo.TrialMemberships", "csToConcord");
            DropColumn("dbo.TrialMemberships", "csToOwnerDate");
            DropColumn("dbo.TrialMemberships", "csToOwner");
        }
    }
}
