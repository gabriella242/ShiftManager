using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using ShiftManagerProject.Models;
using System.ComponentModel.DataAnnotations;

namespace ShiftManagerProject.DAL
{
    public class ShiftManagerContext : DbContext
    {

        public ShiftManagerContext() : base("name=ShiftManagerContext")
        {
        }

        public DbSet<Employees> Employees { get; set; }
        public DbSet<ShiftPref> ShiftPref { get; set; }
        public DbSet<FinalShift> FinalShift { get; set; } 
        public DbSet<History> History { get; set; }
        public DbSet<SavedSchedule> SavedSchedule { get; set; }

        public DbSet<Sunday> Sunday { get; set; }
        public DbSet<Monday> Monday { get; set; }
        public DbSet<Tuesday> Tuesday { get; set; }
        public DbSet<Wednesday> Wednesday { get; set; }
        public DbSet<Thursday> Thursday { get; set; }
        public DbSet<Friday> Friday { get; set; }
        public DbSet<Preferences> Preferences { get; set; }
        public DbSet<ShiftsPerWeek> ShiftsPerWeek { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public System.Data.Entity.DbSet<ShiftManagerProject.Models.ScheduleParameters> ScheduleParameters { get; set; }
    }
}
