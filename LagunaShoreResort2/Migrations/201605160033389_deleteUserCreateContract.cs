namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deleteUserCreateContract : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.SalesContracts", "userCreateContract");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SalesContracts", "userCreateContract", c => c.Int(nullable: false));
        }
    }
}
