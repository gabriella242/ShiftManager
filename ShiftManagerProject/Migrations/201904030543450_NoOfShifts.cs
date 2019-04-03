namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NoOfShifts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "NoOfShifts", c => c.Int(nullable: false));
            DropColumn("dbo.Saturday", "NoOfShifts");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Saturday", "NoOfShifts", c => c.Int());
            DropColumn("dbo.Employees", "NoOfShifts");
        }
    }
}
