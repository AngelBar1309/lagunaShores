namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserCreateContractInSalesContract : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalesContracts", "userCreateContract", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalesContracts", "userCreateContract");
        }
    }
}
