namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixRelationShip_deposits_RS_FA_Contracts : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Deposits", "realStateContractID");
            DropColumn("dbo.Deposits", "salesContractID");
            RenameColumn(table: "dbo.Deposits", name: "realStateContract_realStateContractID", newName: "realStateContractID");
            RenameColumn(table: "dbo.Deposits", name: "salesContract_salesContractID", newName: "salesContractID");
            RenameIndex(table: "dbo.Deposits", name: "IX_salesContract_salesContractID", newName: "IX_salesContractID");
            RenameIndex(table: "dbo.Deposits", name: "IX_realStateContract_realStateContractID", newName: "IX_realStateContractID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Deposits", name: "IX_realStateContractID", newName: "IX_realStateContract_realStateContractID");
            RenameIndex(table: "dbo.Deposits", name: "IX_salesContractID", newName: "IX_salesContract_salesContractID");
            RenameColumn(table: "dbo.Deposits", name: "salesContractID", newName: "salesContract_salesContractID");
            RenameColumn(table: "dbo.Deposits", name: "realStateContractID", newName: "realStateContract_realStateContractID");
            AddColumn("dbo.Deposits", "salesContractID", c => c.Int());
            AddColumn("dbo.Deposits", "realStateContractID", c => c.Int());
        }
    }
}
