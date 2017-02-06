namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ptcDeposits : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Deposits", "paymentType", c => c.String());
            AddColumn("dbo.Deposits", "comments", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Deposits", "comments");
            DropColumn("dbo.Deposits", "paymentType");
        }
    }
}
