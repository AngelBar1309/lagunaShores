namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class loginFix : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Rols", "mandatory");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Rols", "mandatory", c => c.Boolean(nullable: false));
        }
    }
}
