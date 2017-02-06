namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedFields_RealStateContract_Interest_ExchangeRage_PaymentQty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RealStateContracts", "interestRate", c => c.Double(nullable: false));
            AddColumn("dbo.RealStateContracts", "currency", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RealStateContracts", "currency");
            DropColumn("dbo.RealStateContracts", "interestRate");
        }
    }
}
