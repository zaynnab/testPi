namespace Pidev.data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uu : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.depenses", "nbVue", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.depenses", "nbVue");
        }
    }
}
