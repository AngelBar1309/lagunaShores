namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddClosingCostInSalesContract : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalesContracts", "closingCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalesContracts", "closingCost");
        }
    }
}
