namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salesMember_deleted_legalName : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.SalesMembers", "legalName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SalesMembers", "legalName", c => c.String(nullable: false));
        }
    }
}
