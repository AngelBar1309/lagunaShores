namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteUsingStartAndAddYearEvenInSalesContract : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalesContracts", "yearEven", c => c.Boolean(nullable: false));
            DropColumn("dbo.SalesContracts", "usageStartDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SalesContracts", "usageStartDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.SalesContracts", "yearEven");
        }
    }
}
