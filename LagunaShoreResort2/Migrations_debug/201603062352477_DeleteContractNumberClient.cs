namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteContractNumberClient : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Clients", "contractNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Clients", "contractNumber", c => c.String());
        }
    }
}
