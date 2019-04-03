namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DBnameChanges : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Saturday", newName: "Preferences");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Preferences", newName: "Saturday");
        }
    }
}
