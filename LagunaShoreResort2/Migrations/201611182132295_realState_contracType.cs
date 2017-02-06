namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class realState_contracType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RealStateContracts", "contractType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RealStateContracts", "contractType");
        }
    }
}
