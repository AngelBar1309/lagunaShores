namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class change_deposit_depositComment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Deposits", "depositComments", c => c.String());
            DropColumn("dbo.Deposits", "comments");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Deposits", "comments", c => c.String());
            DropColumn("dbo.Deposits", "depositComments");
        }
    }
}
