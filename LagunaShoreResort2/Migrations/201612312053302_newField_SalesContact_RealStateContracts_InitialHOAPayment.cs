namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newField_SalesContact_RealStateContracts_InitialHOAPayment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RealStateContracts", "InitialHOAMonth", c => c.DateTime(nullable: false));
            AddColumn("dbo.SalesContracts", "InitialHOAMonth", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalesContracts", "InitialHOAMonth");
            DropColumn("dbo.RealStateContracts", "InitialHOAMonth");
        }
    }
}
