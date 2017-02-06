namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_props_for_amortization_realStateContracts2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RealStateContracts", "NumberofDownPayments", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RealStateContracts", "NumberofDownPayments");
        }
    }
}
