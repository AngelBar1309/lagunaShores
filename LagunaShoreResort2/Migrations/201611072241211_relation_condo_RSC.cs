namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class relation_condo_RSC : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RealStateContracts", "condoID", c => c.Int(nullable: false));
            CreateIndex("dbo.RealStateContracts", "condoID");
            AddForeignKey("dbo.RealStateContracts", "condoID", "dbo.Condoes", "condoID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RealStateContracts", "condoID", "dbo.Condoes");
            DropIndex("dbo.RealStateContracts", new[] { "condoID" });
            DropColumn("dbo.RealStateContracts", "condoID");
        }
    }
}
