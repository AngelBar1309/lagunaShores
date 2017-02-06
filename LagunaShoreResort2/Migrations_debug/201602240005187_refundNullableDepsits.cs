namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class refundNullableDepsits : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Deposits", "Refund", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Deposits", "Refund", c => c.Boolean(nullable: false));
        }
    }
}
