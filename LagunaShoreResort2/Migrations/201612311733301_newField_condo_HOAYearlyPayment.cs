namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newField_condo_HOAYearlyPayment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Condoes", "HOAYearlyPayment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Condoes", "HOAYearlyPayment");
        }
    }
}
