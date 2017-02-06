namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NationalityInClients : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "nationality", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "nationality");
        }
    }
}
