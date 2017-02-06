namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingtrialMemberships : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Clients", "verifiedByAdmin");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Clients", "verifiedByAdmin", c => c.Boolean(nullable: false));
        }
    }
}
