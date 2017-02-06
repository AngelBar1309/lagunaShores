namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class idkWhatChanged : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrialMemberships", "upgraded", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrialMemberships", "upgraded");
        }
    }
}
