namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salescontract_numberpayments_deleted : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.SalesContracts", "numberPayments");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SalesContracts", "numberPayments", c => c.Int(nullable: false));
        }
    }
}
