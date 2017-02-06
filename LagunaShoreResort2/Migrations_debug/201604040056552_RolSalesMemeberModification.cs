namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RolSalesMemeberModification : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RolSalesMembers", "salesMemeber_salesMemberID", "dbo.SalesMembers");
            DropIndex("dbo.RolSalesMembers", new[] { "salesMemeber_salesMemberID" });
            RenameColumn(table: "dbo.RolSalesMembers", name: "salesMemeber_salesMemberID", newName: "salesMemberID");
            AlterColumn("dbo.RolSalesMembers", "salesMemberID", c => c.Int(nullable: false));
            CreateIndex("dbo.RolSalesMembers", "salesMemberID");
            AddForeignKey("dbo.RolSalesMembers", "salesMemberID", "dbo.SalesMembers", "salesMemberID", cascadeDelete: true);
            DropColumn("dbo.RolSalesMembers", "salesMemeberID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RolSalesMembers", "salesMemeberID", c => c.Int(nullable: false));
            DropForeignKey("dbo.RolSalesMembers", "salesMemberID", "dbo.SalesMembers");
            DropIndex("dbo.RolSalesMembers", new[] { "salesMemberID" });
            AlterColumn("dbo.RolSalesMembers", "salesMemberID", c => c.Int());
            RenameColumn(table: "dbo.RolSalesMembers", name: "salesMemberID", newName: "salesMemeber_salesMemberID");
            CreateIndex("dbo.RolSalesMembers", "salesMemeber_salesMemberID");
            AddForeignKey("dbo.RolSalesMembers", "salesMemeber_salesMemberID", "dbo.SalesMembers", "salesMemberID");
        }
    }
}
