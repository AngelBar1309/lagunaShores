namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class makingNullables : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SalesContracts", "upgrade", c => c.Boolean());
            AlterColumn("dbo.SalesContracts", "upgraded", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SalesContracts", "upgraded", c => c.Boolean(nullable: false));
            AlterColumn("dbo.SalesContracts", "upgrade", c => c.Boolean(nullable: false));
        }
    }
}
