namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class upgradesInventory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalesContracts", "tradeInValue", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.SalesContracts", "deposit", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.SalesContracts", "previousBalance", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Condoes", "usedFA", c => c.Int(nullable: false));
            AddColumn("dbo.Condoes", "evenFA", c => c.Int(nullable: false));
            AddColumn("dbo.Condoes", "oddFA", c => c.Int(nullable: false));
            AddColumn("dbo.Condoes", "listpriceMin", c => c.Double(nullable: false));
            AddColumn("dbo.Condoes", "listpriceMax", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Condoes", "listpriceMax");
            DropColumn("dbo.Condoes", "listpriceMin");
            DropColumn("dbo.Condoes", "oddFA");
            DropColumn("dbo.Condoes", "evenFA");
            DropColumn("dbo.Condoes", "usedFA");
            DropColumn("dbo.SalesContracts", "previousBalance");
            DropColumn("dbo.SalesContracts", "deposit");
            DropColumn("dbo.SalesContracts", "tradeInValue");
        }
    }
}
