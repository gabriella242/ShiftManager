namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initcreate : DbMigration
    {
        public override void Up()
        {
           
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ShiftsPerWeek");
            DropTable("dbo.ScheduleParameters");
            DropTable("dbo.SavedSchedule");
            DropTable("dbo.History");
            DropTable("dbo.Preferences");
            DropTable("dbo.FinalShift");
            DropTable("dbo.Employees");
        }
    }
}
