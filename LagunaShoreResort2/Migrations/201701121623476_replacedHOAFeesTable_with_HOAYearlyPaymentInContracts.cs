namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class replacedHOAFeesTable_with_HOAYearlyPaymentInContracts : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.HOAFees", "condoID", "dbo.Condoes");
            DropIndex("dbo.HOAFees", new[] { "condoID" });
            AddColumn("dbo.RealStateContracts", "HOAYearlyPayment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.SalesContracts", "HOAYearlyPayment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropTable("dbo.HOAFees");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.HOAFees",
                c => new
                    {
                        HOAFeeID = c.Int(nullable: false, identity: true),
                        fee = c.Decimal(nullable: false, precision: 18, scale: 2),
                        fraction = c.String(),
                        condoID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.HOAFeeID);
            
            DropColumn("dbo.SalesContracts", "HOAYearlyPayment");
            DropColumn("dbo.RealStateContracts", "HOAYearlyPayment");
            CreateIndex("dbo.HOAFees", "condoID");
            AddForeignKey("dbo.HOAFees", "condoID", "dbo.Condoes", "condoID", cascadeDelete: true);
        }
    }
}
