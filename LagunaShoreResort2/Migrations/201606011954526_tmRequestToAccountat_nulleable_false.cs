namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tmRequestToAccountat_nulleable_false : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TrialMemberships", "tmRequestToAccountat", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TrialMemberships", "tmRequestToAccountat", c => c.Boolean());
        }
    }
}
