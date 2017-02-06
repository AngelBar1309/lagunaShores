namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class realState_addedFieldForReports : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RealStateContracts", "requestToAccountant", c => c.Boolean(nullable: false));
            AddColumn("dbo.RealStateContracts", "canceledContract", c => c.Boolean(nullable: false));
            AddColumn("dbo.RealStateContracts", "commissionPaid", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RealStateContracts", "commissionPaid");
            DropColumn("dbo.RealStateContracts", "canceledContract");
            DropColumn("dbo.RealStateContracts", "requestToAccountant");
        }
    }
}
