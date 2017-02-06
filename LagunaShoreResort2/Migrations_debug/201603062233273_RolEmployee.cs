namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RolEmployee : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ContractSalesMembers", "salesMemberTypeID", "dbo.SalesMemberTypes");
            DropIndex("dbo.ContractSalesMembers", new[] { "salesMemberTypeID" });
            CreateTable(
                "dbo.Rols",
                c => new
                    {
                        rolID = c.Int(nullable: false, identity: true),
                        type = c.String(),
                        comssion = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.rolID);
            
            CreateTable(
                "dbo.RolSalesMembers",
                c => new
                    {
                        rolSalesMemberID = c.Int(nullable: false, identity: true),
                        rolID = c.Int(nullable: false),
                        salesMemeberID = c.Int(nullable: false),
                        salesMemeber_salesMemberID = c.Int(),
                    })
                .PrimaryKey(t => t.rolSalesMemberID)
                .ForeignKey("dbo.Rols", t => t.rolID, cascadeDelete: true)
                .ForeignKey("dbo.SalesMembers", t => t.salesMemeber_salesMemberID)
                .Index(t => t.rolID)
                .Index(t => t.salesMemeber_salesMemberID);
            
            AddColumn("dbo.ContractSalesMembers", "rolID", c => c.Int(nullable: false));
            CreateIndex("dbo.ContractSalesMembers", "rolID");
            AddForeignKey("dbo.ContractSalesMembers", "rolID", "dbo.Rols", "rolID", cascadeDelete: true);
            DropColumn("dbo.ContractSalesMembers", "salesMemberTypeID");
            DropTable("dbo.SalesMemberTypes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SalesMemberTypes",
                c => new
                    {
                        salesMemberTypeID = c.Int(nullable: false, identity: true),
                        type = c.String(),
                        comssion = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.salesMemberTypeID);
            
            AddColumn("dbo.ContractSalesMembers", "salesMemberTypeID", c => c.Int(nullable: false));
            DropForeignKey("dbo.RolSalesMembers", "salesMemeber_salesMemberID", "dbo.SalesMembers");
            DropForeignKey("dbo.RolSalesMembers", "rolID", "dbo.Rols");
            DropForeignKey("dbo.ContractSalesMembers", "rolID", "dbo.Rols");
            DropIndex("dbo.RolSalesMembers", new[] { "salesMemeber_salesMemberID" });
            DropIndex("dbo.RolSalesMembers", new[] { "rolID" });
            DropIndex("dbo.ContractSalesMembers", new[] { "rolID" });
            DropColumn("dbo.ContractSalesMembers", "rolID");
            DropTable("dbo.RolSalesMembers");
            DropTable("dbo.Rols");
            CreateIndex("dbo.ContractSalesMembers", "salesMemberTypeID");
            AddForeignKey("dbo.ContractSalesMembers", "salesMemberTypeID", "dbo.SalesMemberTypes", "salesMemberTypeID", cascadeDelete: true);
        }
    }
}
