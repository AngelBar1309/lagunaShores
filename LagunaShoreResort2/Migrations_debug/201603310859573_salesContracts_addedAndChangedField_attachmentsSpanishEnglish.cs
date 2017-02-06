namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salesContracts_addedAndChangedField_attachmentsSpanishEnglish : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalesContracts", "attachmentsSpanish", c => c.String());
            AddColumn("dbo.SalesContracts", "attachmentsEnglish", c => c.String());
            DropColumn("dbo.SalesContracts", "attachments");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SalesContracts", "attachments", c => c.String());
            DropColumn("dbo.SalesContracts", "attachmentsEnglish");
            DropColumn("dbo.SalesContracts", "attachmentsSpanish");
        }
    }
}
