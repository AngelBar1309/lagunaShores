namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deposits_bool_refund : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Deposits", "Refund", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Deposits", "Refund", c => c.Boolean());
        }
    }
}
