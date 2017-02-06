namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddContractSendToConcordAndOwnerInSalesContract : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalesContracts", "csToOwner", c => c.Boolean(nullable: false));
            AddColumn("dbo.SalesContracts", "csToOwnerDate", c => c.DateTime());
            AddColumn("dbo.SalesContracts", "csToConcord", c => c.Boolean(nullable: false));
            AddColumn("dbo.SalesContracts", "csToConcordDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalesContracts", "csToConcordDate");
            DropColumn("dbo.SalesContracts", "csToConcord");
            DropColumn("dbo.SalesContracts", "csToOwnerDate");
            DropColumn("dbo.SalesContracts", "csToOwner");
        }
    }
}
