namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nationalityAndTypeOfID_requiredField_Client : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Clients", "typeOfID", c => c.String(nullable: false));
            AlterColumn("dbo.Clients", "nationality", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Clients", "nationality", c => c.String());
            AlterColumn("dbo.Clients", "typeOfID", c => c.String());
        }
    }
}
