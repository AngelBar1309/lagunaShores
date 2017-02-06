namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VerifiByAdminyDos : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "verifiedByAdmin", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "verifiedByAdmin");
        }
    }
}
