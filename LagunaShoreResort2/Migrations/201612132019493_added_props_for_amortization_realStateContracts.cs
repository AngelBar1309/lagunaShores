namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_props_for_amortization_realStateContracts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RealStateContracts", "closingCost", c => c.Double(nullable: false));
            AddColumn("dbo.RealStateContracts", "paymentsQuantity", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RealStateContracts", "paymentsQuantity");
            DropColumn("dbo.RealStateContracts", "closingCost");
        }
    }
}
