namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteQuickWeeksSalesContract : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.TrialMemberships", "quickWeeks");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TrialMemberships", "quickWeeks", c => c.Double());
        }
    }
}
