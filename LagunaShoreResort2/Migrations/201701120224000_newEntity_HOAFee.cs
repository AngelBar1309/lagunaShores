namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newEntity_HOAFee : DbMigration
    {
        public override void Up()
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
                .PrimaryKey(t => t.HOAFeeID)
                .ForeignKey("dbo.Condoes", t => t.condoID, cascadeDelete: true)
                .Index(t => t.condoID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HOAFees", "condoID", "dbo.Condoes");
            DropIndex("dbo.HOAFees", new[] { "condoID" });
            DropTable("dbo.HOAFees");
        }
    }
}
