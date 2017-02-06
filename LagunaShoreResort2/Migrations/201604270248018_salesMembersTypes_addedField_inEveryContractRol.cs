namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salesMembersTypes_addedField_inEveryContractRol : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Rols", "inEveryContract", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Rols", "inEveryContract");
        }
    }
}
