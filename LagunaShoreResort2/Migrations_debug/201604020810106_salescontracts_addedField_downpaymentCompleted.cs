namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salescontracts_addedField_downpaymentCompleted : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalesContracts", "downPaymentCompleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalesContracts", "downPaymentCompleted");
        }
    }
}
