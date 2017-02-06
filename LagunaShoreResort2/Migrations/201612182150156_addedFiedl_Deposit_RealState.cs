namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedFiedl_Deposit_RealState : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RealStateContracts", "deposit", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RealStateContracts", "deposit");
        }
    }
}
