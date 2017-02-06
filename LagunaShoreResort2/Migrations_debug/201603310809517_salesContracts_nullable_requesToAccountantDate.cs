namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salesContracts_nullable_requesToAccountantDate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SalesContracts", "requestToAccountantDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SalesContracts", "requestToAccountantDate", c => c.DateTime(nullable: false));
        }
    }
}
