namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewTBLs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Remake",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        EmployID = c.Long(nullable: true),
                        Name = c.String(nullable: true),
                        Day = c.String(nullable: false),
                        Morning = c.Boolean(nullable: true),
                        Afternoon = c.Boolean(nullable: true),
                        Night = c.Boolean(nullable: true),
                        OfDayType = c.Int(nullable: false),
                        Dates = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.ScheduleParameters", "MaxMorning", c => c.Int(nullable: false));
            AddColumn("dbo.ScheduleParameters", "MaxAfternoon", c => c.Int(nullable: false));
            AddColumn("dbo.ScheduleParameters", "MaxNight", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScheduleParameters", "MaxNight");
            DropColumn("dbo.ScheduleParameters", "MaxAfternoon");
            DropColumn("dbo.ScheduleParameters", "MaxMorning");
            DropTable("dbo.Remake");
        }
    }
}
