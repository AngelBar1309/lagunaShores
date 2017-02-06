namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeletePaymentQuantityInTrialmemberships : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.TrialMemberships", "tmPaymentsQuantity");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TrialMemberships", "tmPaymentsQuantity", c => c.Int(nullable: false));
        }
    }
}
