namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class camposnuevosalescontract : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalesContracts", "NumberofDownPayments", c => c.Int(nullable: false));
            AddColumn("dbo.SalesContracts", "numberPayments", c => c.Int(nullable: false));
            AddColumn("dbo.SalesContracts", "qualification", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalesContracts", "qualification");
            DropColumn("dbo.SalesContracts", "numberPayments");
            DropColumn("dbo.SalesContracts", "NumberofDownPayments");
        }
    }
}
