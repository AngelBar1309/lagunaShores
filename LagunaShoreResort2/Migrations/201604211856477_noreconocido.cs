namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class noreconocido : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Rols", "mandatory", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Rols", "mandatory");
        }
    }
}
