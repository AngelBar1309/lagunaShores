namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addNewUnitPrice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalesContracts", "newUnitPrice", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalesContracts", "newUnitPrice");
        }
    }
}
