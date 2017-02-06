namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class realState_depositRealState : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.RealStateContracts", "contractType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RealStateContracts", "contractType", c => c.String());
        }
    }
}
