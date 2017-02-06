namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addConcordInTM : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrialMemberships", "Concord", c => c.String());
            DropColumn("dbo.TrialMemberships", "csToOwner");
            DropColumn("dbo.TrialMemberships", "csToOwnerDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TrialMemberships", "csToOwnerDate", c => c.DateTime());
            AddColumn("dbo.TrialMemberships", "csToOwner", c => c.Boolean(nullable: false));
            DropColumn("dbo.TrialMemberships", "Concord");
        }
    }
}
