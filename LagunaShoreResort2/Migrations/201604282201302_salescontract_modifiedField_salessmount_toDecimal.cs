namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salescontract_modifiedField_salessmount_toDecimal : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SalesContracts", "saleAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SalesContracts", "saleAmount", c => c.Double(nullable: false));
        }
    }
}
