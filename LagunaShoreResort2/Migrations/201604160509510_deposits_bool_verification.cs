namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deposits_bool_verification : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Deposits", "verification", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Deposits", "verification", c => c.Boolean());
        }
    }
}
