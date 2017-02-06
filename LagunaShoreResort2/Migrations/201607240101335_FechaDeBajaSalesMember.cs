namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FechaDeBajaSalesMember : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalesMembers", "droppedOutDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalesMembers", "droppedOutDate");
        }
    }
}
