namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RSC_nullable_assignor : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RealStateContracts", "clientAssignorID", "dbo.Clients");
            DropIndex("dbo.RealStateContracts", new[] { "clientAssignorID" });
            AlterColumn("dbo.RealStateContracts", "clientAssignorID", c => c.Int());
            CreateIndex("dbo.RealStateContracts", "clientAssignorID");
            AddForeignKey("dbo.RealStateContracts", "clientAssignorID", "dbo.Clients", "clientID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RealStateContracts", "clientAssignorID", "dbo.Clients");
            DropIndex("dbo.RealStateContracts", new[] { "clientAssignorID" });
            AlterColumn("dbo.RealStateContracts", "clientAssignorID", c => c.Int(nullable: false));
            CreateIndex("dbo.RealStateContracts", "clientAssignorID");
            AddForeignKey("dbo.RealStateContracts", "clientAssignorID", "dbo.Clients", "clientID", cascadeDelete: true);
        }
    }
}
