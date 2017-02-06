namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removed_HOAPAymentYear_from_CondoEntity : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Condoes", "HOAYearlyPayment");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Condoes", "HOAYearlyPayment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
