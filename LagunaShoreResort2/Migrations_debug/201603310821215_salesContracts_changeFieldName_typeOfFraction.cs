namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salesContracts_changeFieldName_typeOfFraction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalesContracts", "typeOfFraction", c => c.String());
            DropColumn("dbo.SalesContracts", "typeOfFraccion");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SalesContracts", "typeOfFraccion", c => c.String());
            DropColumn("dbo.SalesContracts", "typeOfFraction");
        }
    }
}
