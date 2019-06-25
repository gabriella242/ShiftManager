
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftManagerProject.Models
{
    public class Employees
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }
        
        [DisplayName("First Name")]
        [Required(ErrorMessage = "First Name Required")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Last Name Required")]
        public string LastName { get; set; }

        [DataType(DataType.EmailAddress)]
        [DisplayName("User")]
        [Required(ErrorMessage = "Email Required")]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [DisplayName("Telephone")]
        [Required(ErrorMessage = "Telephone Required")]
        public string Telephone { get; set; }

        [DisplayName("Password")]
        [Required(ErrorMessage = "Password Required")]
        public string Pass { get; set; }

        [Required(ErrorMessage = "Required")]
        [DisplayName("Number of Shifts")]
        public int NoOfShifts { get; set; }

        [DisplayName("Admin Permissions")]
        [Required(ErrorMessage = "Admin Permissions Required")]
        public Nullable<bool> Admin { get; set; }

    }

    public class Sunday : Monday
    {
        [Required(ErrorMessage = "Required")]
        [DisplayName("Morning")]
        public Nullable<bool> Morning1 { get; set; }
        [Required(ErrorMessage = "Required")]
        [DisplayName("Afternoon")]
        public Nullable<bool> Afternoon1 { get; set; }
        [Required(ErrorMessage = "Required")]
        [DisplayName("Night")]
        public Nullable<bool> Night1 { get; set; }
    }
    public class Monday : Tuesday
    {
        [Required(ErrorMessage = "Required")]
        [DisplayName("Morning")]
        public Nullable<bool> Morning2 { get; set; }
        [Required(ErrorMessage = "Required")]
        [DisplayName("Afternoon")]
        public Nullable<bool> Afternoon2 { get; set; }
        [Required(ErrorMessage = "Required")]
        [DisplayName("Night")]
        public Nullable<bool> Night2 { get; set; }
    }
    public class Tuesday : Wednesday
    {
        [Required(ErrorMessage = "Required")]
        [DisplayName("Morning")]
        public Nullable<bool> Morning3 { get; set; }
        [Required(ErrorMessage = "Required")]
        [DisplayName("Afternoon")]
        public Nullable<bool> Afternoon3 { get; set; }
        [Required(ErrorMessage = "Required")]
        [DisplayName("Night")]
        public Nullable<bool> Night3 { get; set; }
    }
    public class Wednesday : Thursday
    {
        [Required(ErrorMessage = "Required")]
        [DisplayName("Morning")]
        public Nullable<bool> Morning4 { get; set; }
        [Required(ErrorMessage = "Required")]
        [DisplayName("Afternoon")]
        public Nullable<bool> Afternoon4 { get; set; }
        [Required(ErrorMessage = "Required")]
        [DisplayName("Night")]
        public Nullable<bool> Night4 { get; set; }
    }
    public class Thursday : Friday
    {
        [Required(ErrorMessage = "Required")]
        [DisplayName("Morning")]
        public Nullable<bool> Morning5 { get; set; }
        [Required(ErrorMessage = "Required")]
        [DisplayName("Afternoon")]
        public Nullable<bool> Afternoon5 { get; set; }
        [Required(ErrorMessage = "Required")]
        [DisplayName("Night")]
        public Nullable<bool> Night5 { get; set; }
    }
    public class Friday : Preferences
    {
        [Required(ErrorMessage = "Required")]
        [DisplayName("Morning")]
        public Nullable<bool> Morning6 { get; set; }
        [Required(ErrorMessage = "Required")]
        [DisplayName("Afternoon")]
        public Nullable<bool> Afternoon6 { get; set; }
        [Required(ErrorMessage = "Required")]
        [DisplayName("Night")]
        public Nullable<bool> Night6 { get; set; }
    }
    public class Preferences
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Required(ErrorMessage = "Required")]
        [DisplayName("Morning")]
        public Nullable<bool> Morning7 { get; set; }
        [Required(ErrorMessage = "Required")]
        [DisplayName("Afternoon")]
        public Nullable<bool> Afternoon7 { get; set; }
        [Required(ErrorMessage = "Required")]
        [DisplayName("Night")]
        public Nullable<bool> Night7 { get; set; }
    }

    public class ShiftPref : Sunday
    {
        [Required]
        public long EmployID { get; set; }

        public string Name { get; set; }

        public string Message { get; set; }
    }

    public class FinalShift 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long ID { get; set; }

        public long EmployID { get; set; }

        public string Name { get; set; }

        [Required]
        public string Day { get; set; }

        [DisplayName("Morning")]
        public Nullable<bool> Morning { get; set; }

        [DisplayName("Afternoon")]
        public Nullable<bool> Afternoon { get; set; }
 
        [DisplayName("Night")]
        public Nullable<bool> Night { get; set; }

        public int OfDayType { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Required]
        public DateTime Dates { get; set; }

        public IEnumerable<Employees> Employees { get; set; }
    }

    public class History
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long ID { get; set; }

        public long EmployID { get; set; }

        public string Name { get; set; }

        [Required]
        public string Day { get; set; }

        [Required]
        [DisplayName("Morning")]
        public Nullable<bool> Morning { get; set; }
        [Required]
        [DisplayName("Afternoon")]
        public Nullable<bool> Afternoon { get; set; }
        [Required]
        [DisplayName("Night")]
        public Nullable<bool> Night { get; set; }

        public int OfDayType { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Required]
        public DateTime Dates { get; set; }

        public IEnumerable<Employees> Employees { get; set; }
    }

    public class ScheduleParameters
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int ID { get; set; }

        [DisplayName("Morning")]
        public int Morning { get; set; }

        [DisplayName("Afternoon")]
        public int Afternoon { get; set; }
   
        [DisplayName("Night")]
        public int Night { get; set; }

        public string Day { get; set; }

        [DisplayName("Morning")]
        public Nullable<int> DMorning { get; set; }

        [DisplayName("Afternoon")]
        public Nullable<int> DAfternoon { get; set; }

        [DisplayName("Night")]
        public Nullable<int> DNight { get; set; }

        [DisplayName("Max Morning Shifts")]
        public int MaxMorning { get; set; }

        [DisplayName("Max Afternoon Shifts")]
        public int MaxAfternoon { get; set; }

        [DisplayName("Max Night Shifts")]
        public int MaxNight { get; set; }
    }

    public class ShiftsPerWeek
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int ID { get; set; }

        [Required]
        public int NumOfShifts { get; set; }
    }

    public class SavedSchedule
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long ID { get; set; }

        public long EmployID { get; set; }

        public string Name { get; set; }

        [Required]
        public string Day { get; set; }

        [DisplayName("Morning")]
        public Nullable<bool> Morning { get; set; }

        [DisplayName("Afternoon")]
        public Nullable<bool> Afternoon { get; set; }

        [DisplayName("Night")]
        public Nullable<bool> Night { get; set; }

        public int OfDayType { get; set; }

        [Required]
        public DateTime Dates { get; set; }

        public IEnumerable<Employees> Employees { get; set; }
    }

}