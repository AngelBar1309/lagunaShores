namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salesContracts_nullable_verificationAndComissionDate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SalesContracts", "verificationDate", c => c.DateTime());
            AlterColumn("dbo.SalesContracts", "commissionPaidDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SalesContracts", "commissionPaidDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.SalesContracts", "verificationDate", c => c.DateTime(nullable: false));
        }
    }
}
