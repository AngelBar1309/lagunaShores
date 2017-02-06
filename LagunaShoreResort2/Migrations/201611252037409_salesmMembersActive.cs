namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salesmMembersActive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalesMembers", "emergencyContactName", c => c.String(nullable: false));
            AddColumn("dbo.SalesMembers", "active", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalesMembers", "active");
            DropColumn("dbo.SalesMembers", "emergencyContactName");
        }
    }
}
