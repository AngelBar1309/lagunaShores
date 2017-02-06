namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingLegalName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalesMembers", "legalName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalesMembers", "legalName");
        }
    }
}
