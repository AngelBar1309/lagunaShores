namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salesContractAddFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalesContracts", "requestToAccountant", c => c.Boolean(nullable: false));
            AddColumn("dbo.SalesContracts", "requestToAccountantDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.SalesContracts", "attachments", c => c.String());
            AddColumn("dbo.SalesContracts", "currency", c => c.String());
            AddColumn("dbo.SalesContracts", "listPrice", c => c.Double(nullable: false));
            AddColumn("dbo.SalesContracts", "deedWeeks", c => c.Int(nullable: false));
            AddColumn("dbo.SalesContracts", "bonusWeeks", c => c.Double(nullable: false));
            AddColumn("dbo.SalesContracts", "bonusWeeksExp", c => c.DateTime(nullable: false));
            AddColumn("dbo.SalesContracts", "advantageWeeks", c => c.Double(nullable: false));
            AddColumn("dbo.SalesContracts", "advantageWeeksExp", c => c.DateTime(nullable: false));
            AddColumn("dbo.SalesContracts", "commissionPaid", c => c.Boolean(nullable: false));
            AddColumn("dbo.SalesContracts", "commissionPaidDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.SalesContracts", "salesDirector");
            DropColumn("dbo.SalesContracts", "requestDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SalesContracts", "requestDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.SalesContracts", "salesDirector", c => c.String());
            DropColumn("dbo.SalesContracts", "commissionPaidDate");
            DropColumn("dbo.SalesContracts", "commissionPaid");
            DropColumn("dbo.SalesContracts", "advantageWeeksExp");
            DropColumn("dbo.SalesContracts", "advantageWeeks");
            DropColumn("dbo.SalesContracts", "bonusWeeksExp");
            DropColumn("dbo.SalesContracts", "bonusWeeks");
            DropColumn("dbo.SalesContracts", "deedWeeks");
            DropColumn("dbo.SalesContracts", "listPrice");
            DropColumn("dbo.SalesContracts", "currency");
            DropColumn("dbo.SalesContracts", "attachments");
            DropColumn("dbo.SalesContracts", "requestToAccountantDate");
            DropColumn("dbo.SalesContracts", "requestToAccountant");
        }
    }
}
