namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cliente_realstate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RealStateContracts",
                c => new
                    {
                        realStateContractID = c.Int(nullable: false, identity: true),
                        type = c.String(),
                        MLS = c.String(),
                        ownershipHeld = c.String(),
                        saleAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        saleDate = c.DateTime(nullable: false),
                        closingDate = c.DateTime(nullable: false),
                        transfererName = c.String(),
                        salesMemberID = c.Int(nullable: false),
                        clientAssignedID = c.Int(nullable: false),
                        clientAssignorID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.realStateContractID)
                .ForeignKey("dbo.SalesMembers", t => t.salesMemberID, cascadeDelete: false)
                .ForeignKey("dbo.Clients", t => t.clientAssignedID, cascadeDelete: false)
                .ForeignKey("dbo.Clients", t => t.clientAssignorID, cascadeDelete: false)
                .Index(t => t.salesMemberID)
                .Index(t => t.clientAssignedID)
                .Index(t => t.clientAssignorID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RealStateContracts", "clientAssignorID", "dbo.Clients");
            DropForeignKey("dbo.RealStateContracts", "clientAssignedID", "dbo.Clients");
            DropForeignKey("dbo.RealStateContracts", "salesMemberID", "dbo.SalesMembers");
            DropIndex("dbo.RealStateContracts", new[] { "clientAssignorID" });
            DropIndex("dbo.RealStateContracts", new[] { "clientAssignedID" });
            DropIndex("dbo.RealStateContracts", new[] { "salesMemberID" });
            DropTable("dbo.RealStateContracts");
        }
    }
}
