namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCampsForDwonloadContracts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "typeOfID", c => c.String());
            AddColumn("dbo.SalesContracts", "priceInWordSpanish", c => c.String());
            AddColumn("dbo.SalesContracts", "priceInWordInglish", c => c.String());
            AddColumn("dbo.SalesContracts", "Concord", c => c.String());
            AddColumn("dbo.Condoes", "building", c => c.String());
            AddColumn("dbo.Condoes", "sqft", c => c.String());
            AddColumn("dbo.Condoes", "sqmt", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Condoes", "sqmt");
            DropColumn("dbo.Condoes", "sqft");
            DropColumn("dbo.Condoes", "building");
            DropColumn("dbo.SalesContracts", "Concord");
            DropColumn("dbo.SalesContracts", "priceInWordInglish");
            DropColumn("dbo.SalesContracts", "priceInWordSpanish");
            DropColumn("dbo.Clients", "typeOfID");
        }
    }
}
