namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsageStartDateInSalesContract : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalesContracts", "usageStartDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalesContracts", "usageStartDate");
        }
    }
}
