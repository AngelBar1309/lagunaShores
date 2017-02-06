namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deposits_requieredFields : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Deposits", "CurrencyType", c => c.String(nullable: false));
            AlterColumn("dbo.Deposits", "PayMethod", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Deposits", "PayMethod", c => c.String());
            AlterColumn("dbo.Deposits", "CurrencyType", c => c.String());
        }
    }
}
