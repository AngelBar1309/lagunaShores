namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deposit_HOA_relations : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Deposits", "realStateContractID", "dbo.RealStateContracts");
            DropForeignKey("dbo.Deposits", "salesContractID", "dbo.SalesContracts");
            DropIndex("dbo.Deposits", new[] { "salesContractID" });
            DropIndex("dbo.Deposits", new[] { "realStateContractID" });
            AddColumn("dbo.Deposits", "salesContractID_HOA", c => c.Int());
            AddColumn("dbo.Deposits", "realStateContractID_HOA", c => c.Int());
            AddColumn("dbo.Deposits", "realStateContract_realStateContractID", c => c.Int());
            AddColumn("dbo.Deposits", "salesContract_salesContractID", c => c.Int());
            CreateIndex("dbo.Deposits", "salesContractID_HOA");
            CreateIndex("dbo.Deposits", "realStateContractID_HOA");
            CreateIndex("dbo.Deposits", "realStateContract_realStateContractID");
            CreateIndex("dbo.Deposits", "salesContract_salesContractID");
            AddForeignKey("dbo.Deposits", "salesContractID_HOA", "dbo.SalesContracts", "salesContractID");
            AddForeignKey("dbo.Deposits", "realStateContractID_HOA", "dbo.RealStateContracts", "realStateContractID");
            AddForeignKey("dbo.Deposits", "realStateContract_realStateContractID", "dbo.RealStateContracts", "realStateContractID");
            AddForeignKey("dbo.Deposits", "salesContract_salesContractID", "dbo.SalesContracts", "salesContractID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Deposits", "salesContract_salesContractID", "dbo.SalesContracts");
            DropForeignKey("dbo.Deposits", "realStateContract_realStateContractID", "dbo.RealStateContracts");
            DropForeignKey("dbo.Deposits", "realStateContractID_HOA", "dbo.RealStateContracts");
            DropForeignKey("dbo.Deposits", "salesContractID_HOA", "dbo.SalesContracts");
            DropIndex("dbo.Deposits", new[] { "salesContract_salesContractID" });
            DropIndex("dbo.Deposits", new[] { "realStateContract_realStateContractID" });
            DropIndex("dbo.Deposits", new[] { "realStateContractID_HOA" });
            DropIndex("dbo.Deposits", new[] { "salesContractID_HOA" });
            DropColumn("dbo.Deposits", "salesContract_salesContractID");
            DropColumn("dbo.Deposits", "realStateContract_realStateContractID");
            DropColumn("dbo.Deposits", "realStateContractID_HOA");
            DropColumn("dbo.Deposits", "salesContractID_HOA");
            CreateIndex("dbo.Deposits", "realStateContractID");
            CreateIndex("dbo.Deposits", "salesContractID");
            AddForeignKey("dbo.Deposits", "salesContractID", "dbo.SalesContracts", "salesContractID");
            AddForeignKey("dbo.Deposits", "realStateContractID", "dbo.RealStateContracts", "realStateContractID");
        }
    }
}
