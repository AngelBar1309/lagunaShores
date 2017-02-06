namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedFields_RealStateContract_RequieredFields : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RealStateContracts", "type", c => c.String(nullable: false));
            AlterColumn("dbo.RealStateContracts", "MLS", c => c.String(nullable: false));
            AlterColumn("dbo.RealStateContracts", "ownershipHeld", c => c.String(nullable: false));
            AlterColumn("dbo.RealStateContracts", "currency", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RealStateContracts", "currency", c => c.String());
            AlterColumn("dbo.RealStateContracts", "ownershipHeld", c => c.String());
            AlterColumn("dbo.RealStateContracts", "MLS", c => c.String());
            AlterColumn("dbo.RealStateContracts", "type", c => c.String());
        }
    }
}
