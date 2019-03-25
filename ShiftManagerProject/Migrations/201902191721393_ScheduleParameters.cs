namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ScheduleParameters : DbMigration
    {
        public override void Up()
        {
            CreateTable(
           "dbo.ScheduleParameters",
           c => new
           {
               ID = c.Int(nullable: false, identity: true),
               Morning = c.Int(nullable: true),
               Afternoon = c.Int(nullable: true),
               Night = c.Int(nullable: true),
               Day = c.String(nullable: true),
               DMorning = c.Int(nullable: true),
               DAfternoon = c.Int(nullable: true),
               DNight = c.Int(nullable: true),
           })
           .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.ShiftsPerWeek",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    NumOfShifts = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ID);

        }
        
        public override void Down()
        {
            DropTable("dbo.ShiftsPerWeek");
            DropTable("dbo.ScheduleParameters");
        }
    }
}
