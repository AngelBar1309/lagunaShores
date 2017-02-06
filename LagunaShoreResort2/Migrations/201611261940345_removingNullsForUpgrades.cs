namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removingNullsForUpgrades : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SalesContracts", "upgrade", c => c.Boolean(nullable: false));
            AlterColumn("dbo.SalesContracts", "upgraded", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SalesContracts", "upgraded", c => c.Boolean());
            AlterColumn("dbo.SalesContracts", "upgrade", c => c.Boolean());
        }
    }
}
