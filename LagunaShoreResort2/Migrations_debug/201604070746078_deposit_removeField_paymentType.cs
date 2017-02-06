namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deposit_removeField_paymentType : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Deposits", "paymentType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Deposits", "paymentType", c => c.String());
        }
    }
}
