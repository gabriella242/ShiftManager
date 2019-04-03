namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DBnameChange : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.PrevWeeks", newName: "History");
            RenameTable(name: "dbo.Remake", newName: "SavedSchedule");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.SavedSchedule", newName: "Remake");
            RenameTable(name: "dbo.History", newName: "PrevWeeks");
        }
    }
}
