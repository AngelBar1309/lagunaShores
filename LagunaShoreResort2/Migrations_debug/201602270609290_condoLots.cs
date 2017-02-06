namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class condoLots : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Condoes", "lot", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Condoes", "lot");
        }
    }
}
