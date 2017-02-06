namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deposit_refNumber_requiered : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Deposits", "RefNumber", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Deposits", "RefNumber", c => c.String());
        }
    }
}
