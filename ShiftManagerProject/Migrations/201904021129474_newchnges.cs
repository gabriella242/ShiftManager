namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newchnges : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ScheduleParameters", "DMorning", c => c.Int());
            AlterColumn("dbo.ScheduleParameters", "DAfternoon", c => c.Int());
            AlterColumn("dbo.ScheduleParameters", "DNight", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ScheduleParameters", "DNight", c => c.Int(nullable: false));
            AlterColumn("dbo.ScheduleParameters", "DAfternoon", c => c.Int(nullable: false));
            AlterColumn("dbo.ScheduleParameters", "DMorning", c => c.Int(nullable: false));
        }
    }
}
