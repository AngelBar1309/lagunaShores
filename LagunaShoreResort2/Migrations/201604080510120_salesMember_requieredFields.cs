namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salesMember_requieredFields : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SalesMembers", "legalName", c => c.String(nullable: false));
            AlterColumn("dbo.SalesMembers", "firtName", c => c.String(nullable: false));
            AlterColumn("dbo.SalesMembers", "lastName", c => c.String(nullable: false));
            AlterColumn("dbo.SalesMembers", "countryOfResidence", c => c.String(nullable: false));
            AlterColumn("dbo.SalesMembers", "countryOfOrigin", c => c.String(nullable: false));
            AlterColumn("dbo.SalesMembers", "address", c => c.String(nullable: false));
            AlterColumn("dbo.SalesMembers", "telephone", c => c.String(nullable: false));
            AlterColumn("dbo.SalesMembers", "email", c => c.String(nullable: false));
            AlterColumn("dbo.SalesMembers", "taxID", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SalesMembers", "taxID", c => c.String());
            AlterColumn("dbo.SalesMembers", "email", c => c.String());
            AlterColumn("dbo.SalesMembers", "telephone", c => c.String());
            AlterColumn("dbo.SalesMembers", "address", c => c.String());
            AlterColumn("dbo.SalesMembers", "countryOfOrigin", c => c.String());
            AlterColumn("dbo.SalesMembers", "countryOfResidence", c => c.String());
            AlterColumn("dbo.SalesMembers", "lastName", c => c.String());
            AlterColumn("dbo.SalesMembers", "firtName", c => c.String());
            AlterColumn("dbo.SalesMembers", "legalName", c => c.String());
        }
    }
}
