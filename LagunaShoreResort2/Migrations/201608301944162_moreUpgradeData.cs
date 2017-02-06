namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class moreUpgradeData : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalesContracts", "upgrade", c => c.Boolean());
            AddColumn("dbo.SalesContracts", "upgraded", c => c.Boolean());
            AddColumn("dbo.SalesContracts", "previousTMID", c => c.Int());
            AddColumn("dbo.SalesContracts", "previousFCID", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalesContracts", "previousFCID");
            DropColumn("dbo.SalesContracts", "previousTMID");
            DropColumn("dbo.SalesContracts", "upgraded");
            DropColumn("dbo.SalesContracts", "upgrade");
        }
    }
}
