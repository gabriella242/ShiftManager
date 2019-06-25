namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newchanges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
               "dbo.Employees",
               c => new
               {
                   ID = c.Long(nullable: false),
                   FirstName = c.String(nullable: false),
                   LastName = c.String(nullable: false),
                   Email = c.String(nullable: false),
                   Telephone = c.String(nullable: false),
                   Pass = c.String(nullable: false),
                   NoOfShifts = c.Int(nullable: false),
                   Admin = c.Boolean(nullable: false),
               })
               .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.FinalShift",
                c => new
                {
                    ID = c.Long(nullable: false, identity: true),
                    EmployID = c.Long(nullable: false),
                    Name = c.String(),
                    Day = c.String(nullable: false),
                    Morning = c.Boolean(),
                    Afternoon = c.Boolean(),
                    Night = c.Boolean(),
                    OfDayType = c.Int(nullable: false),
                    Dates = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.Preferences",
                c => new
                {
                    ID = c.Long(nullable: false, identity: true),
                    Morning7 = c.Boolean(nullable: false),
                    Afternoon7 = c.Boolean(nullable: false),
                    Night7 = c.Boolean(nullable: false),
                    Morning6 = c.Boolean(),
                    Afternoon6 = c.Boolean(),
                    Night6 = c.Boolean(),
                    Morning5 = c.Boolean(),
                    Afternoon5 = c.Boolean(),
                    Night5 = c.Boolean(),
                    Morning4 = c.Boolean(),
                    Afternoon4 = c.Boolean(),
                    Night4 = c.Boolean(),
                    Morning3 = c.Boolean(),
                    Afternoon3 = c.Boolean(),
                    Night3 = c.Boolean(),
                    Morning2 = c.Boolean(),
                    Afternoon2 = c.Boolean(),
                    Night2 = c.Boolean(),
                    Morning1 = c.Boolean(),
                    Afternoon1 = c.Boolean(),
                    Night1 = c.Boolean(),
                    EmployID = c.Long(),
                    Name = c.String(),
                    Message = c.String(),
                    Discriminator = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.History",
                c => new
                {
                    ID = c.Long(nullable: false, identity: true),
                    EmployID = c.Long(nullable: false),
                    Name = c.String(),
                    Day = c.String(nullable: false),
                    Morning = c.Boolean(nullable: false),
                    Afternoon = c.Boolean(nullable: false),
                    Night = c.Boolean(nullable: false),
                    OfDayType = c.Int(nullable: false),
                    Dates = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.SavedSchedule",
                c => new
                {
                    ID = c.Long(nullable: false, identity: true),
                    EmployID = c.Long(nullable: false),
                    Name = c.String(),
                    Day = c.String(nullable: false),
                    Morning = c.Boolean(),
                    Afternoon = c.Boolean(),
                    Night = c.Boolean(),
                    OfDayType = c.Int(nullable: false),
                    Dates = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.ScheduleParameters",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    Morning = c.Int(nullable: false),
                    Afternoon = c.Int(nullable: false),
                    Night = c.Int(nullable: false),
                    Day = c.String(),
                    DMorning = c.Int(),
                    DAfternoon = c.Int(),
                    DNight = c.Int(),
                    MaxMorning = c.Int(nullable: false),
                    MaxAfternoon = c.Int(nullable: false),
                    MaxNight = c.Int(nullable: false),
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
        }
    }
}
