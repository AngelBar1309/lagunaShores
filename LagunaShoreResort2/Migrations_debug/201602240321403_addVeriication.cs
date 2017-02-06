namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addVeriication : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Deposits", "verification", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Deposits", "verification");
        }
    }
}
