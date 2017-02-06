namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ignoringAllPreviouschanges : DbMigration
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
                        verifiedByAdmin = c.Boolean(nullable: false),
                        salesMemberID = c.Int(nullable: false),
                        clientAssignedID = c.Int(nullable: false),
                        clientAssignorID = c.Int(),
                        condoID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.realStateContractID)
                .ForeignKey("dbo.Condoes", t => t.condoID, cascadeDelete: true)
                .ForeignKey("dbo.SalesMembers", t => t.salesMemberID, cascadeDelete: true)
                .ForeignKey("dbo.Clients", t => t.clientAssignedID, cascadeDelete: true)
                .ForeignKey("dbo.Clients", t => t.clientAssignorID)
                .Index(t => t.salesMemberID)
                .Index(t => t.clientAssignedID)
                .Index(t => t.clientAssignorID)
                .Index(t => t.condoID);
            
            AddColumn("dbo.Deposits", "realStateContractID", c => c.Int());
            CreateIndex("dbo.Deposits", "realStateContractID");
            AddForeignKey("dbo.Deposits", "realStateContractID", "dbo.RealStateContracts", "realStateContractID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RealStateContracts", "clientAssignorID", "dbo.Clients");
            DropForeignKey("dbo.RealStateContracts", "clientAssignedID", "dbo.Clients");
            DropForeignKey("dbo.RealStateContracts", "salesMemberID", "dbo.SalesMembers");
            DropForeignKey("dbo.Deposits", "realStateContractID", "dbo.RealStateContracts");
            DropForeignKey("dbo.RealStateContracts", "condoID", "dbo.Condoes");
            DropIndex("dbo.Deposits", new[] { "realStateContractID" });
            DropIndex("dbo.RealStateContracts", new[] { "condoID" });
            DropIndex("dbo.RealStateContracts", new[] { "clientAssignorID" });
            DropIndex("dbo.RealStateContracts", new[] { "clientAssignedID" });
            DropIndex("dbo.RealStateContracts", new[] { "salesMemberID" });
            DropColumn("dbo.Deposits", "realStateContractID");
            DropTable("dbo.RealStateContracts");
        }
    }
}
