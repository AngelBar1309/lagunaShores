namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSocialMediaSalesMember : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalesMembers", "emergencyContact", c => c.String(nullable: false));
            AddColumn("dbo.SalesMembers", "socialMedia", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalesMembers", "socialMedia");
            DropColumn("dbo.SalesMembers", "emergencyContact");
        }
    }
}
