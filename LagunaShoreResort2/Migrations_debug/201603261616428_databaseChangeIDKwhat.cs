namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class databaseChangeIDKwhat : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Deposits", "salesContractID", "dbo.SalesContracts");
            DropIndex("dbo.Deposits", new[] { "salesContractID" });
            AlterColumn("dbo.Deposits", "salesContractID", c => c.Int());
            CreateIndex("dbo.Deposits", "salesContractID");
            AddForeignKey("dbo.Deposits", "salesContractID", "dbo.SalesContracts", "salesContractID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Deposits", "salesContractID", "dbo.SalesContracts");
            DropIndex("dbo.Deposits", new[] { "salesContractID" });
            AlterColumn("dbo.Deposits", "salesContractID", c => c.Int(nullable: false));
            CreateIndex("dbo.Deposits", "salesContractID");
            AddForeignKey("dbo.Deposits", "salesContractID", "dbo.SalesContracts", "salesContractID", cascadeDelete: true);
        }
    }
}
