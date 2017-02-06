namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class realState_depositos_verifiedByAdmin : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RealStateContracts", "verifiedByAdmin", c => c.Boolean(nullable: false));
            AddColumn("dbo.Deposits", "realStateContractID", c => c.Int());
            CreateIndex("dbo.Deposits", "realStateContractID");
            AddForeignKey("dbo.Deposits", "realStateContractID", "dbo.RealStateContracts", "realStateContractID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Deposits", "realStateContractID", "dbo.RealStateContracts");
            DropIndex("dbo.Deposits", new[] { "realStateContractID" });
            DropColumn("dbo.Deposits", "realStateContractID");
            DropColumn("dbo.RealStateContracts", "verifiedByAdmin");
        }
    }
}
