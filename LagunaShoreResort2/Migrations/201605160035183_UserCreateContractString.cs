namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserCreateContractString : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalesContracts", "userCreateContract", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalesContracts", "userCreateContract");
        }
    }
}
