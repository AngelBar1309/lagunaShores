namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class realState_noTransfererName : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.RealStateContracts", "transfererName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RealStateContracts", "transfererName", c => c.String());
        }
    }
}
