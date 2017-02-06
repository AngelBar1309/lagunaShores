namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salesContracts_nullable_canceledDate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SalesContracts", "canceledDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SalesContracts", "canceledDate", c => c.DateTime(nullable: false));
        }
    }
}
