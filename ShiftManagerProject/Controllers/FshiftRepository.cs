using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Reflection;
using System.Web.Mvc;
using ShiftManagerProject.DAL;
using ShiftManagerProject.Models;

namespace ShiftManagerProject.Controllers
{
    public class FshiftRepository
    {
        private ShiftManagerContext db = new ShiftManagerContext();
        static Random rnd = new Random();
        static int DaySerial = 0;
        static int[,] Mat;

        public int OfDayHandler(bool whattodo, int d, int s)
        {
            int mo = db.ScheduleParameters.Select(y => y.Morning).FirstOrDefault();
            int af = db.ScheduleParameters.Select(y => y.Afternoon).FirstOrDefault();
            int ni = db.ScheduleParameters.Select(y => y.Night).FirstOrDefault();
            int k = 1;
            int num = 0;

            if (whattodo)
            {
                int dayparameters = Convert.ToInt32(db.ScheduleParameters.Select(t => t.DMorning).Sum()) + Convert.ToInt32(db.ScheduleParameters.Select(w => w.DAfternoon).Sum()) + Convert.ToInt32(db.ScheduleParameters.Select(q => q.DNight).Sum());
                int totalshifts = (db.ScheduleParameters.Select(x => x.Morning + x.Afternoon + x.Night).FirstOrDefault()) + dayparameters + 1;
                int difftotal = (db.ScheduleParameters.Select(x => x.Morning + x.Afternoon + x.Night).FirstOrDefault()) + 1;
                bool flag = true;
                Mat = new int[totalshifts, 7];
                for (int i = 0; i < difftotal; i++)
                {
                    for (int j = 0, x = 1; x < 8 && i == 0; j++, x++)
                    {
                        Mat[i, j] = x;
                    }

                    for (int j = 0, x = i - 1; j < 7 && i != 0; j++, x += (difftotal - 1))
                    {
                        string Dweek = DayOfWeek(j + 1);
                        string NDweek = DayOfWeek(j);
                        if (db.ScheduleParameters.Where(t => t.Day == Dweek).Any() && i == mo + af + ni)
                        {
                            var list = db.ScheduleParameters.Where(t => t.Day == Dweek).ToList();
                            foreach (var para in list)
                            {
                                Mat[i, j] = x;

                                if (para.DMorning != 0)
                                {
                                    for (int g = 0, r = i; g < para.DMorning; g++)
                                    {
                                        Mat[++r, j] = ++x;
                                    }
                                }
                                if (para.DAfternoon != 0)
                                {
                                    for (int g = 0, r = i; g < para.DAfternoon; g++)
                                    {
                                        Mat[++r, j] = ++x;
                                    }
                                }
                                if (para.DNight != 0)
                                {
                                    for (int g = 0, r = i; g < para.DNight; g++)
                                    {
                                        Mat[++r, j] = ++x;
                                    }
                                }
                            }
                            flag = false;
                            continue;
                        }
                        else if(db.ScheduleParameters.Where(t => t.Day == NDweek).Any() && flag)
                        {
                            var dayaddition = db.ScheduleParameters.Where(t => t.Day == NDweek).Select(t=>t.DMorning + t.DAfternoon + t.Night).Sum();
                            x += Convert.ToInt32(dayaddition);
                            Mat[i, j] = x;
                            continue;
                        }
                        Mat[i, j] = x;
                    }
                }
            }
            else
            {
                if (s == 1)
                {
                    for (k = 0; num != Mat[s + k, d - 1]; k++)
                    {
                        //num = Mat[s + k, d - 1] != 99 ? Mat[s + k, d - 1] : Mat[s + k + 1, d - 1] != 99 ? Mat[s + k + 1, d - 1] : 100; 

                        while (num != Mat[s + k, d - 1])
                        {
                            num = Mat[s + k, d - 1] == 99 ? 0 : Mat[s + k, d - 1];
                            if (num == Mat[s + k, d - 1])
                            { break; }
                            else { k++; }
                        }
                        if (num == Mat[s + k, d - 1])
                        { break; }
                    }
                    Mat[s + k, d - 1] = 99;
                }
                else if (s == 2)
                {
                    string Dweek = DayOfWeek(d);
                    var dayaddition = db.ScheduleParameters.Where(t => t.Day == Dweek).Select(t => t.DMorning).Any() ? db.ScheduleParameters.Where(t => t.Day == Dweek).Select(t => t.DMorning).Sum() : 0;

                    for (k = Convert.ToInt32(dayaddition) + mo + 1; num != Mat[k, d - 1]; k++)
                    {
                        while(num != Mat[k, d - 1])
                        {
                            num = Mat[k, d - 1] == 99 ? 0 : Mat[k, d - 1];
                            if (num == Mat[k, d - 1])
                            { break; }
                            else { k++; }
                        }

                        if (num == Mat[k, d - 1])
                        { break; }
                    }
                    Mat[k, d - 1] = 99;
                }

                else if (s == 3)
                {
                    string Dweek = DayOfWeek(d);
                    var dayaddition = db.ScheduleParameters.Where(t => t.Day == Dweek).Select(t => t.DAfternoon + t.DMorning).Any() ? db.ScheduleParameters.Where(t => t.Day == Dweek).Select(t => t.DAfternoon + t.DMorning).Sum() : 0;

                    for (k = Convert.ToInt32(dayaddition) + mo + af + 1; num != Mat[k, d - 1]; k++)
                    {
                        //num = Mat[k, d - 1] != 99 ? Mat[k, d - 1] : Mat[k + 1, d - 1] != 99 ? Mat[k + 1, d - 1] : 100;

                        while (num != Mat[k, d - 1])
                        {
                            num = Mat[k, d - 1] == 99 ? 0 : Mat[k, d - 1];
                            if (num == Mat[k, d - 1])
                            { break; }
                            else { k++; }
                        }

                        if (num == Mat[k, d - 1])
                        { break; }
                    }
                    Mat[k, d - 1] = 99;
                }
            }

            return num;
        }


        //public FinalShift CheckPref(int x, string y)
        //{
        //    List<Employees> Workers = new List<Employees>();
        //    var LastDay = db.PrevWeeks.OrderByDescending(p => p.ID).Take(2);
        //    var PrevShift = db.FinalShift.OrderByDescending(p => p.ID).Take(2);
        //    var Employees = db.Employees.ToList();
        //    var flag = 0;
        //    string shiftofday = null;

        //    foreach (var employ in Employees)
        //    {
        //        var a = 0;
        //        flag = 0;
        //        ShiftPref pref = new ShiftPref();
        //        pref = db.ShiftPref.SingleOrDefault(ShiftPref => ShiftPref.EmployID == employ.ID);

        //        if (pref == null)
        //        {
        //            continue;
        //        }

        //        if (x == 1 && y == "M")
        //        {
        //            foreach (var LastShift in LastDay)
        //            {
        //                if (employ.ID == LastShift.EmployID)
        //                {
        //                    flag++;
        //                }
        //            }
        //        }
        //        else if (x == 1 && y == "A")
        //        {
        //            LastDay = db.PrevWeeks.OrderByDescending(p => p.ID).Take(1);
        //            foreach (var lastday in LastDay)
        //            {
        //                if (employ.ID == lastday.EmployID)
        //                {
        //                    flag++;
        //                }
        //            }

        //            foreach (var PrevShifts in PrevShift)
        //            {
        //                if (employ.ID == PrevShifts.EmployID)
        //                {
        //                    flag++;
        //                }
        //            }
        //        }
        //        else if (y == "N" && x == 1)
        //        {
        //            foreach (var LastShift in LastDay)
        //            {
        //                if (employ.ID == LastShift.EmployID && LastShift.Night == true)
        //                {
        //                    flag++;
        //                }
        //            }
        //            PrevShift = db.FinalShift.OrderByDescending(p => p.ID).Take(3);
        //            foreach (var PrevShifts in PrevShift)
        //            {
        //                if (employ.ID == PrevShifts.EmployID)
        //                {
        //                    flag++;
        //                }
        //            }
        //        }
        //        else if (y == "M" && x != 1)
        //        {
        //            PrevShift = db.FinalShift.OrderByDescending(p => p.ID).Take(2);
        //            foreach (var PrevShifts in PrevShift)
        //            {
        //                if (employ.ID == PrevShifts.EmployID)
        //                {
        //                    flag++;
        //                }
        //            }
        //        }
        //        else if ((y == "N" || y == "A") && x != 1)
        //        {
        //            PrevShift = db.FinalShift.OrderByDescending(p => p.ID).Take(3);
        //            foreach (var PrevShifts in PrevShift)
        //            {
        //                if (employ.ID == PrevShifts.EmployID)
        //                {
        //                    flag++;
        //                }
        //            }

        //            if (y == "A" && x > 2)
        //            {
        //                PrevShift = db.FinalShift.Where(k => k.Afternoon == true).OrderByDescending(p => p.ID).Take(x - 1);
        //                foreach (var AfternoonShift in PrevShift)
        //                {
        //                    if (employ.ID == AfternoonShift.EmployID)
        //                    {
        //                        a++;
        //                    }
        //                    if (a == 2)
        //                    {
        //                        flag++;
        //                    }
        //                }
        //            }
        //            if (y == "N" && x > 2)
        //            {
        //                PrevShift = db.FinalShift.Where(k => k.Night == true).OrderByDescending(p => p.ID).Take(x - 1);
        //                foreach (var NightShift in PrevShift)
        //                {
        //                    if (employ.ID == NightShift.EmployID)
        //                    {
        //                        a++;
        //                    }
        //                    if (a == 2)
        //                    {
        //                        flag++;
        //                    }
        //                }
        //            }
        //        }

        //        if (flag == 0)
        //        {
        //            foreach (var shift in pref.GetType().GetProperties())
        //            {
        //                if (shift.Name.EndsWith(Convert.ToString(x)) && shift.Name.StartsWith(y))
        //                {
        //                    shiftofday = shift.Name;
        //                    shiftofday = shiftofday.Remove(shiftofday.Length - 1);

        //                    var val = (Boolean)shift.GetValue(pref);
        //                    if (val)
        //                    {
        //                        Workers.Add(employ);
        //                        break;
        //                    }
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    FinalShift Fshift = new FinalShift();

        //    if (!Workers.Any())
        //    {
        //        foreach (var employ in Employees)
        //        {
        //            ShiftPref prefshift = new ShiftPref();
        //            prefshift = db.ShiftPref.SingleOrDefault(ShiftPref => ShiftPref.EmployID == employ.ID);
        //            if (prefshift == null)
        //            {
        //                continue;
        //            }

        //            foreach (var shift in prefshift.GetType().GetProperties())
        //            {
        //                if (shift.Name.EndsWith(Convert.ToString(x)) && shift.Name.StartsWith(y))
        //                {
        //                    shiftofday = shift.Name;
        //                    shiftofday = shiftofday.Remove(shiftofday.Length - 1);

        //                    var val = (Boolean)shift.GetValue(prefshift);
        //                    if (val)
        //                    {
        //                        Workers.Add(employ);
        //                        break;
        //                    }
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    else if (Workers.Any())
        //    {
        //        int r = rnd.Next(Workers.Count);
        //        Fshift = new FinalShift
        //        {
        //            EmployID = Workers[r].ID,
        //            Name = Workers[r].FirstName
        //        };
        //    }


        //    var Ftype = typeof(FinalShift);

        //    foreach (var shift in Fshift.GetType().GetProperties())
        //    {
        //        if (shift.Name == "Day")
        //        {
        //            Ftype.GetProperty(shift.Name).SetValue(Fshift, DayOfWeek(x));
        //        }
        //        if (shiftofday == shift.Name)
        //        {
        //            Ftype.GetProperty(shift.Name).SetValue(Fshift, true);
        //        }
        //    }
        //    return (Fshift);
        //}

        public string DayOfWeek(int d)
        {
            List<string> dayofweek = new List<string>(new string[] { "", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" });
            return (dayofweek[d]);
        }

        //public bool AfternoonChecker(long ID)
        //{
        //    var PrevShifts = db.FinalShift.Where(k => k.Afternoon == true).OrderByDescending(k => k.ID).Take(7);
        //    var p = 0;

        //    foreach (var AfternoonShift in PrevShifts)
        //    {
        //        if (ID == AfternoonShift.EmployID)
        //        {
        //            p++;
        //        }
        //    }
        //    if (p == 2)
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        public List<long> ListOfEmployees()
        {
            List<long> EmployIDs = new List<long>();
            foreach (var emp in db.Employees)
            {
                EmployIDs.Add(emp.ID);
            }
            return EmployIDs;
        }

        //public void EightEightChecker()
        //{
        //    var Cshifts = (db.FinalShift.OrderByDescending(k => k.ID).Take(28).OrderBy(k => k.ID)).ToList();
        //    int flagC = 0;

        //    foreach (var employ in ListOfEmployees())
        //    {
        //        flagC = 0;
        //        for (int p = 0, q = 0; p < Cshifts.Count(); p++)
        //        {
        //            if (Cshifts[p].Day != "Sunday")
        //            {
        //                if (Cshifts[p].Morning == true)
        //                {
        //                    q = q == 2 ? 0 : q;
        //                    q++;

        //                    if (q != 2)
        //                    {
        //                        if (Cshifts[p - 2].EmployID == Cshifts[p].EmployID && Cshifts[p].EmployID == employ)
        //                        {
        //                            flagC++;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (Cshifts[p - 3].EmployID == Cshifts[p].EmployID && Cshifts[p].EmployID == employ)
        //                        {
        //                            flagC++;
        //                        }
        //                    }
        //                }
        //                else if (Cshifts[p].Afternoon == true)
        //                {
        //                    if (Cshifts[p - 3].EmployID == Cshifts[p].EmployID && Cshifts[p].EmployID == employ)
        //                    {
        //                        flagC++;
        //                    }
        //                }

        //            }
        //            if (flagC == 2)
        //            {
        //                flagC = 0;
        //                FinalShift ID = db.FinalShift.Find(Cshifts[p].ID);
        //                Cshifts[p].EmployID = 0;
        //                Cshifts[p].Name = null;
        //                var result = db.FinalShift.SingleOrDefault(b => b.ID == ID.ID);
        //                db.Entry(result).CurrentValues.SetValues(Cshifts[p]);
        //                db.SaveChanges();
        //            }
        //        }
        //    }
        //}

        //public void NightChecker()
        //{
        //    int flagC = 0;
        //    var fshifts = (db.FinalShift.OrderByDescending(k => k.ID).Take(28).OrderBy(k => k.ID)).ToList();

        //    foreach (var employ in ListOfEmployees())
        //    {
        //        List<FinalShift> NightShifts = new List<FinalShift>();
        //        flagC = 0;
        //        foreach (var shift in fshifts)
        //        {
        //            if (shift.Night == true && shift.EmployID == employ)
        //            {
        //                flagC++;
        //                NightShifts.Add(shift);
        //            }
        //            if (flagC > 2)
        //            {
        //                FinalShift ID = db.FinalShift.Find(shift.ID);
        //                shift.EmployID = 0;
        //                shift.Name = null;
        //                var result = db.FinalShift.SingleOrDefault(b => b.ID == ID.ID);
        //                db.Entry(result).CurrentValues.SetValues(shift);
        //                db.SaveChanges();
        //            }
        //        }
        //    }
        //}

        //public void MorningReset(FinalShift BeforeChecking, int i)
        //{
        //    BeforeChecking.Day = DayOfWeek(i);
        //    BeforeChecking.Morning = true;
        //    BeforeChecking.EmployID = 0;
        //    BeforeChecking.Name = null;
        //    SavingToDB(BeforeChecking);
        //}

        //public void SavingToDB(FinalShift BeforeChecking)
        //{
        //    BeforeChecking.OfDayType = DaySerial++;
        //    DaySerial = DaySerial > 27 ? 0 : DaySerial;
        //    db.FinalShift.Add(BeforeChecking);
        //    db.SaveChanges();
        //}

        //public FinalShift ResetFeilds(FinalShift BeforeChecking)
        //{
        //    BeforeChecking.EmployID = 0;
        //    BeforeChecking.Name = null;
        //    return BeforeChecking;
        //}

        public void PrevShiftsRotation()
        {
            History pweek = new History();
            var fshift = db.FinalShift.ToList();
            foreach (var Fshift in fshift)
            {
                pweek.Day = Fshift.Day;
                pweek.EmployID = Fshift.EmployID;
                pweek.Name = Fshift.Name;
                pweek.Dates = Fshift.Dates;
                pweek.OfDayType = Fshift.OfDayType;

                if (Fshift.Morning == null)
                {
                    pweek.Morning = false;
                }
                else
                {
                    pweek.Morning = Fshift.Morning;
                }
                if (Fshift.Afternoon == null)
                {
                    pweek.Afternoon = false;
                }
                else
                {
                    pweek.Afternoon = Fshift.Afternoon;
                }
                if (Fshift.Night == null)
                {
                    pweek.Night = false;
                }
                else
                {
                    pweek.Night = Fshift.Night;
                }
                db.History.Add(pweek);
                db.SaveChanges();
            }
        }

        public bool FutureShifts(int x, string y, Employees employee)
        {
            string dayofweek = DayOfWeek(x);
            string Fdayofweek = x != 7 ? DayOfWeek(x + 1) : DayOfWeek(x);
            int flag = 0;
            if (y == "M")
            {
                var futureShifts = db.FinalShift.Where(k => k.Day == dayofweek);
                foreach (var shift in futureShifts)
                {
                    if (shift.EmployID == employee.ID)
                    {
                        flag++;
                    }
                }
                if (flag != 0)
                {
                    return true;
                }
            }

            if (y == "A")
            {
                if (x != 7)
                {
                    var futureShifts = db.FinalShift.Where(k => (k.Day == dayofweek && k.Night == true) || (k.Day == Fdayofweek && k.Morning == true));

                    foreach (var shift in futureShifts)
                    {
                        if (shift.EmployID == employee.ID)
                        {
                            flag++;
                        }
                    }
                }
                else
                {
                    var futureShifts = db.FinalShift.Where(k => k.Day == dayofweek && k.Night == true);

                    foreach (var shift in futureShifts)
                    {
                        if (shift.EmployID == employee.ID)
                        {
                            flag++;
                        }
                    }
                }

                if (flag != 0)
                {
                    return true;
                }
            }

            if (y == "N")
            {
                if (x != 7)
                {
                    var futureShifts = db.FinalShift.Where(k => k.Day == Fdayofweek && (k.Morning == true || k.Afternoon == true));

                    foreach (var shift in futureShifts)
                    {
                        if (shift.EmployID == employee.ID)
                        {
                            flag++;
                        }
                    }
                }

                if (flag != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool PreviousShifts(int x, string y, Employees employee)
        {
            string dayofweek = DayOfWeek(x);
            string lastdayofweek = DayOfWeek(x - 1);
            var totalshifts = db.ShiftsPerWeek.Select(o => o.NumOfShifts).FirstOrDefault();
            int flag = 0;
            int mo = db.ScheduleParameters.Select(k => k.Morning).FirstOrDefault();
            int af = db.ScheduleParameters.Select(k => k.Afternoon).FirstOrDefault();
            int ni = db.ScheduleParameters.Select(k => k.Night).FirstOrDefault();

            if (y == "M")
            {
                if (x == 1)
                {
                    var LastDay = db.History.OrderByDescending(r => DbFunctions.TruncateTime(r.Dates)).Take(totalshifts).OrderByDescending(c => c.OfDayType).Take(af + ni);
                    foreach (var LastShift in LastDay)
                    {
                        if (LastShift.EmployID == employee.ID)
                        {
                            flag++;
                        }
                    }
                }
                else
                {
                    var PrevShifts = db.FinalShift.Where(k => (k.Day == lastdayofweek && (k.Afternoon == true || k.Night == true)) || (k.Day == dayofweek && k.Morning == true));
                    foreach (var shift in PrevShifts)
                    {
                        if (shift.EmployID == employee.ID)
                        {
                            flag++;
                        }
                    }
                }

                if (flag != 0)
                {
                    return true;
                }
            }

            if (y == "A")
            {
                if (x == 1)
                {
                    var LastDay = db.History.OrderByDescending(r => DbFunctions.TruncateTime(r.Dates)).Take(totalshifts).OrderByDescending(c => c.OfDayType).Take(ni);
                    foreach (var lastday in LastDay)
                    {
                        if (lastday.EmployID == employee.ID)
                        {
                            flag++;
                        }
                    }

                    var PrevShifts = db.FinalShift.Where(k => k.Day == dayofweek);

                    foreach (var shift in PrevShifts)
                    {
                        if (shift.EmployID == employee.ID)
                        {
                            flag++;
                        }
                    }
                }
                else
                {
                    var PrevShifts = db.FinalShift.Where(k => (k.Day == lastdayofweek && k.Night == true) || (k.Day == dayofweek));

                    foreach (var shift in PrevShifts)
                    {
                        if (shift.EmployID == employee.ID)
                        {
                            flag++;
                        }
                    }
                }

                if (flag != 0)
                {
                    return true;
                }
            }

            if (y == "N")
            {
                if (x == 1)
                {
                    var LastDay = db.History.OrderByDescending(r => DbFunctions.TruncateTime(r.Dates)).Take(totalshifts).OrderByDescending(c => c.OfDayType).Take(ni);
                    var PrevShifts = db.FinalShift.Where(k => k.Day == dayofweek);

                    foreach (var shift in PrevShifts)
                    {
                        if (shift.EmployID == employee.ID)
                        {
                            flag++;
                        }
                    }

                    foreach (var shift in LastDay)
                    {
                        if (shift.EmployID == employee.ID)
                        {
                            flag++;
                        }
                    }
                }
                else
                {
                    var PrevShifts = db.FinalShift.Where(k => k.Day == dayofweek);

                    foreach (var shift in PrevShifts)
                    {
                        if (shift.EmployID == employee.ID)
                        {
                            flag++;
                        }
                    }
                }

                if (flag != 0)
                {
                    return true;
                }
            }
            return false;
        }

        //public bool Exist(int x, string y)
        //{
        //    int flag = 0;
        //    string dayofweek = DayOfWeek(x);
        //    List<FinalShift> futureShifts = db.FinalShift.Where(k => k.Day == dayofweek).ToList();
        //    if (futureShifts.Any())
        //    {
        //        switch (y)
        //        {
        //            case "M":
        //                foreach (var shift in futureShifts)
        //                {
        //                    if (shift.Morning == true)
        //                    {
        //                        AssignDayType(shift);
        //                        flag++;
        //                    }
        //                }
        //                if (flag == 2)
        //                {
        //                    return true;
        //                }
        //                break;
        //            case "A":
        //                foreach (var shift in futureShifts)
        //                {
        //                    if (shift.Afternoon == true)
        //                    {
        //                        AssignDayType(shift);
        //                        return true;
        //                    }
        //                }
        //                break;
        //            case "N":
        //                foreach (var shift in futureShifts)
        //                {
        //                    if (shift.Night == true)
        //                    {
        //                        AssignDayType(shift);
        //                        return true;
        //                    }
        //                }
        //                break;
        //        }
        //    }
        //    return false;
        //}

        //public bool OneMornExist(int x, string y)
        //{
        //    string dayofweek = DayOfWeek(x);
        //    var futureShifts = db.FinalShift.Where(k => k.Day == dayofweek);
        //    if (futureShifts.Any())
        //    {
        //        switch (y)
        //        {
        //            case "M":
        //                foreach (var shift in futureShifts)
        //                {
        //                    if (shift.Morning == true)
        //                    {
        //                        return true;
        //                    }
        //                }
        //                break;
        //        }
        //    }
        //    return false;
        //}
        public bool MaxMornings(long ID, int x)
        {
            int a = 0;
            var PrevShift = db.FinalShift.Where(k => k.Morning == true).ToList();
            foreach (var MorningShift in PrevShift)
            {
                if (ID == MorningShift.EmployID)
                {
                    a++;
                }
                if (a == db.ScheduleParameters.Select(y => y.MaxMorning).FirstOrDefault())
                {
                    return true;
                }
            }
            return false;
        }

        public bool MaxAfternoons(long ID, int x)
        {
            int a = 0;
            var PrevShift = db.FinalShift.Where(k => k.Afternoon == true).ToList();
            foreach (var AfternoonShift in PrevShift)
            {
                if (ID == AfternoonShift.EmployID)
                {
                    a++;
                }
                if (a == db.ScheduleParameters.Select(y => y.MaxAfternoon).FirstOrDefault())
                {
                    return true;
                }
            }
            return false;
        }

        public bool MaxNights(long ID, int x)
        {
            int a = 0;
            var PrevShift = db.FinalShift.Where(k => k.Night == true).ToList();
            foreach (var NightShift in PrevShift)
            {
                if (ID == NightShift.EmployID)
                {
                    a++;
                }
                if (a == db.ScheduleParameters.Select(y => y.MaxNight).FirstOrDefault())
                {
                    return true;
                }
            }
            return false;
        }

        public bool CanAddShift(long ID)
        {
            var EmployShifts = db.FinalShift.Where(k => k.EmployID == ID);
            //ShiftPref pref = db.ShiftPref.SingleOrDefault(ShiftPref => ShiftPref.EmployID == ID);

            Employees EmployS = db.Employees.Find(ID);

            if (EmployShifts.Count() < EmployS.NoOfShifts)
            {
                return true;
            }
            return false;
        }

        public FinalShift NewCheckerP(int x, string y)
        {

            List<Employees> Workers = new List<Employees>();
            var Employees = db.Employees.ToList();
            int flag = 0;
            FinalShift Fshift = new FinalShift();
            int r;

            foreach (var employ in Employees)
            {
                string shiftofday = null;
                ShiftPref pref = db.ShiftPref.SingleOrDefault(ShiftPref => ShiftPref.EmployID == employ.ID);

                if (pref == null)
                {
                    continue;
                }

                if (!PreviousShifts(x, y, employ))
                {
                    if (!FutureShifts(x, y, employ))
                    {
                        foreach (var shift in pref.GetType().GetProperties())
                        {
                            if (shift.Name.EndsWith(Convert.ToString(x)) && shift.Name.StartsWith(y))
                            {
                                shiftofday = shift.Name;
                                shiftofday = shiftofday.Remove(shiftofday.Length - 1);

                                var val = (Boolean)shift.GetValue(pref);
                                if (val && CanAddShift(employ.ID))
                                {
                                    Workers.Add(employ);
                                    break;
                                }
                                break;
                            }
                        }
                    }
                }
            }

            Redo: if (Workers.Any())
            {
                r = rnd.Next(Workers.Count);
                Fshift = new FinalShift
                {
                    EmployID = Workers[r].ID,
                    Name = Workers[r].FirstName,
                    Day = DayOfWeek(x)
                };
            }
            else
            {
                return null;
            }

            switch (y)
            {
                case "M":
                    if (MaxMornings(Fshift.EmployID, x))
                    {
                        if (flag == 3)
                        {
                            return null;
                        }
                        Workers.Remove(Workers[r]);
                        flag++;
                        goto Redo;
                    }
                    Fshift.Morning = true;
                    break;
                case "A":
                    if (MaxAfternoons(Fshift.EmployID, x))
                    {
                        if (flag == 3)
                        {
                            return null;
                        }
                        Workers.Remove(Workers[r]);
                        flag++;
                        goto Redo;
                    }
                    Fshift.Afternoon = true;
                    break;
                case "N":
                    if (MaxNights(Fshift.EmployID, x))
                    {
                        if (flag == 3)
                        {
                            return null;
                        }
                        Workers.Remove(Workers[r]);
                        flag++;
                        goto Redo;
                    }
                    Fshift.Night = true;
                    break;
            }

            return Fshift;

        }

        public void AssignDayType(FinalShift Fshift)
        {
            var totalshifts = db.ShiftsPerWeek.Select(o => o.NumOfShifts).FirstOrDefault();
            var result = db.FinalShift.SingleOrDefault(b => b.ID == Fshift.ID);
            Fshift.OfDayType = DaySerial++;
            DaySerial = DaySerial > (totalshifts - 1) ? 0 : DaySerial;

            if (result != null)
            {
                db.Entry(result).CurrentValues.SetValues(Fshift);
                db.SaveChanges();
            }
            else
            {
                new Exception("There is no such record to modify");
            }
        }

        public void LeastShiftPref()
        {
            ShiftPref prefshifts = new ShiftPref();
            List<Employees> employees = db.Employees.ToList();
            var totalshifts = db.ShiftsPerWeek.Select(o => o.NumOfShifts).FirstOrDefault();     
            var val = false;
            FinalShift NewFinalS = new FinalShift();
            List<string> FirstLetter = new List<string>(new string[] { "", "M", "A", "N" });
            int[] counter = new int[4], TrueCounter = new int[4];
            int i, j = 0, y = 0, max = 0, TrueMax = 0;
            var dayparameters = db.ScheduleParameters.Where(o => o.Day != null).Any() ? db.ScheduleParameters.Where(o => o.Day != null).Select(o => o.Day).ToList().Last() : null;
            if(dayparameters != null)
            { for (i = 1; DayOfWeek(i) != dayparameters; i++) ; }
            else { i = 1; }     
            var daynumber = i;

            OfDayHandler(true, 0, 0);
            if (db.FinalShift.Count() > 0 && db.FinalShift.Count() < totalshifts)
            {
                var FixedShifts = db.FinalShift.ToList();
                foreach (var shift in FixedShifts)
                {
                    for (i = 1; DayOfWeek(i) != shift.Day; i++) ;
                    y = shift.Morning == true ? 1 : shift.Afternoon == true ? 2 : 3;

                    OrderOfDayTypeHandler(i, y);
                }
            }

            for (i = 1; i < 8; i++)
            {
                foreach (var employ in employees)
                {
                    prefshifts = db.ShiftPref.SingleOrDefault(ShiftPref => ShiftPref.EmployID == employ.ID);
                    if (prefshifts == null)
                    {
                        continue;
                    }

                    foreach (var shifts in prefshifts.GetType().GetProperties())
                    {
                        if (shifts.Name.EndsWith(Convert.ToString(i)))
                        {
                            if (NewExistForNewCheckerP(i, shifts.Name.Substring(0, 1)))
                            { continue; }

                            val = (Boolean)shifts.GetValue(prefshifts);

                            switch (shifts.Name.Substring(0, 1))
                            {
                                case "M":
                                    if (!val)
                                    {
                                        counter[1] += 1;
                                    }
                                    else
                                    {
                                        TrueCounter[1] += 1;
                                    }
                                    break;
                                case "A":
                                    if (!val)
                                    {
                                        counter[2] += 1;
                                    }
                                    else
                                    {
                                        TrueCounter[2] += 1;
                                    }
                                    break;
                                case "N":
                                    if (!val)
                                    {
                                        counter[3] += 1;
                                    }
                                    else
                                    {
                                        TrueCounter[3] += 1;
                                    }
                                    break;
                            }
                        }
                    }
                }

                DayOfWeek day = new System.DayOfWeek();
                string dday = DayOfWeek(i);

                for (int d = 0; d < 7; d++)
                {
                    if (day.ToString() != dday)
                    {
                        day = (DayOfWeek)((d + 1) % 7);
                    }
                    else { break; }
                }

                max = counter.ToList().IndexOf(counter.Max());
                TrueMax = TrueCounter.ToList().IndexOf(TrueCounter.Max());

                if (counter.Max() > 0)
                {
                    NewFinalS = NewCheckerP(i, FirstLetter[max]);
                    if (NewFinalS == null)
                    {
                        NewFinalS = new FinalShift
                        {
                            EmployID = 0,
                            Name = null,
                            Day = DayOfWeek(i)
                        };
                        switch (FirstLetter[max])
                        {
                            case "M":
                                NewFinalS.Morning = true;
                                break;
                            case "A":
                                NewFinalS.Afternoon = true;
                                break;
                            case "N":
                                NewFinalS.Night = true;
                                break;
                        }
                    }
                    NewFinalS.OfDayType = OrderOfDayTypeHandler(i, max);
                    NewFinalS.Dates = NextWeeksDates(day);
                    db.FinalShift.Add(NewFinalS);
                    db.SaveChanges();
                }
                else if (TrueCounter.Max() > 0 && counter.Max() == 0)
                {
                    NewFinalS = NewCheckerP(i, FirstLetter[TrueMax]);
                    if (NewFinalS == null)
                    {
                        NewFinalS = new FinalShift
                        {
                            EmployID = 0,
                            Name = null,
                            Day = DayOfWeek(i)
                        };
                        switch (FirstLetter[TrueMax])
                        {
                            case "M":
                                NewFinalS.Morning = true;
                                break;
                            case "A":
                                NewFinalS.Afternoon = true;
                                break;
                            case "N":
                                NewFinalS.Night = true;
                                break;
                        }
                    }
                    NewFinalS.OfDayType = OrderOfDayTypeHandler(i, TrueMax);
                    NewFinalS.Dates = NextWeeksDates(day);
                    db.FinalShift.Add(NewFinalS);
                    db.SaveChanges();
                }

                counter[0] = counter[1] = counter[2] = counter[3] = 0;
                TrueCounter[0] = TrueCounter[1] = TrueCounter[2] = TrueCounter[3] = 0;

                j++;

                i = j + 1 < (totalshifts + daynumber) ? i : 9;
                i = i + 1 == 8 ? 0 : i;

            }
        }

        public int OrderOfDayTypeHandler(int d, int s)
        {
            return OfDayHandler(false, d, s);
        }

        public bool NewExistForNewCheckerP(int x, string y)
        {
            int flag = 0;
            string dayofweek = DayOfWeek(x);
            List<FinalShift> futureShifts = db.FinalShift.Where(k => k.Day == dayofweek).ToList();
            int mo = db.ScheduleParameters.Select(k => k.Morning).FirstOrDefault();
            int af = db.ScheduleParameters.Select(k => k.Afternoon).FirstOrDefault();
            int ni = db.ScheduleParameters.Select(k => k.Night).FirstOrDefault();

            var dayparameters = db.ScheduleParameters.Where(o => o.Day == dayofweek).FirstOrDefault();
            if (dayparameters != null)
            {
                if (dayparameters.DMorning != 0)
                {
                    mo += Convert.ToInt32(dayparameters.DMorning);
                }
                if (dayparameters.DAfternoon != 0)
                {
                    af += Convert.ToInt32(dayparameters.DAfternoon);
                }
                if (dayparameters.DNight != 0)
                {
                    ni += Convert.ToInt32(dayparameters.DNight);
                }
            }

            if (futureShifts.Any())
            {
                switch (y)
                {
                    case "M":
                        foreach (var shift in futureShifts)
                        {
                            if (shift.Morning == true)
                            {
                                flag++;
                            }
                        }
                        if (flag == mo)
                        {
                            return true;
                        }
                        break;
                    case "A":
                        foreach (var shift in futureShifts)
                        {
                            if (shift.Afternoon == true)
                            {
                                flag++;
                            }
                            if (flag == af)
                            {
                                return true;
                            }
                        }
                        break;
                    case "N":
                        foreach (var shift in futureShifts)
                        {
                            if (shift.Night == true)
                            {
                                flag++;
                            }
                            if (flag == ni)
                            {
                                return true;
                            }
                        }
                        break;
                }
            }
            return false;
        }


        public List<string> AvailableEmployees(int x, string y)
        {
            ShiftPref pref = new ShiftPref();
            bool val;
            List<string> prefshifts = new List<string>();

            foreach (var emp in db.Employees.ToList())
            {
                pref = db.ShiftPref.SingleOrDefault(ShiftPref => ShiftPref.EmployID == emp.ID);
                Employees EmployS = db.Employees.Find(emp.ID);

                if (pref == null)
                {
                    continue;
                }

                foreach (var shifts in pref.GetType().GetProperties())
                {
                    if (shifts.Name.EndsWith(Convert.ToString(x)))
                    {
                        if (shifts.Name.Substring(0, 1) == y)
                        {
                            val = (Boolean)shifts.GetValue(pref);

                            if (val && (db.FinalShift.Where(p => p.EmployID == emp.ID).Count()) < EmployS.NoOfShifts)
                            {
                                prefshifts.Add(emp.FirstName);
                            }
                            else
                            { break; }
                        }
                    }
                }
            }

            return prefshifts;
        }

        public List<string> AvailableEightEightEmployees(int x, string y, List<string> AvailableEmployees)
        {
            List<string> EightShiftWorkers = new List<string>();
            var FinalShiftList = db.FinalShift.ToList();
            var EmployeeList = db.Employees.ToList();
            var totalshifts = db.ShiftsPerWeek.Select(o => o.NumOfShifts).FirstOrDefault();

            foreach (var emp in AvailableEmployees)
            {
                var empID = EmployeeList.Where(e => e.FirstName == emp).Select(w => w.ID).Single();

                if (x == 1 && y == "M" ? db.History.OrderByDescending(r => DbFunctions.TruncateTime(r.Dates)).Take(totalshifts).OrderByDescending(c => c.OfDayType).Where(t => t.EmployID == empID && t.Day == "Saturday" && t.Night == true).Count() == 0 :
                    y == "M" ? FinalShiftList.Where(d => d.Day == DayOfWeek(x - 1) && d.Night == true && d.EmployID == empID).Count() == 0 :
                    y == "N" && x != 7 ? FinalShiftList.Where(q => q.Day == DayOfWeek(x + 1) && q.Morning == true && q.EmployID == empID).Count() == 0 : true)
                {
                    if (FinalShiftList.Where(d => d.Day == DayOfWeek(x) && d.EmployID == empID).Count() == 0)
                    {
                        EightShiftWorkers.Add(emp);
                    }
                }

            }

            return EightShiftWorkers;
        }

        public DateTime NextWeeksDates(DayOfWeek dayOfWeek)
        {
            DateTime DateOfNextWeekDay = DateTime.Now.AddDays(1);

            DateTime nextSunday = DateTime.Now.AddDays(1);
            while (nextSunday.DayOfWeek != System.DayOfWeek.Sunday)
            { nextSunday = nextSunday.AddDays(1); }

            if (dayOfWeek.ToString() == nextSunday.DayOfWeek.ToString())
            {
                DateOfNextWeekDay = nextSunday;
            }
            else
            {
                while (DateOfNextWeekDay.DayOfWeek.ToString() != dayOfWeek.ToString() || DateOfNextWeekDay < nextSunday)
                { DateOfNextWeekDay = DateOfNextWeekDay.AddDays(1); }
            }

            return DateOfNextWeekDay;
        }

        public void SaveToSavedScheduleTBL()
        {
            SavedSchedule re = new SavedSchedule();
            var fshift = db.FinalShift.ToList();
            foreach (var shift in fshift)
            {
                re.Day = shift.Day;
                re.Dates = shift.Dates;
                re.OfDayType = shift.OfDayType;

                re.Morning = shift.Morning;
                re.Afternoon = shift.Afternoon;
                re.Night = shift.Night;
                re.EmployID = shift.EmployID;
                re.Name = shift.Name;

                db.SavedSchedule.Add(re);
                db.SaveChanges();
            }
        }

        public void SaveRemakeToFinalTBL()
        {
            var remakeList = db.SavedSchedule.ToList();
            FinalShift re = new FinalShift();

            foreach (var shift in remakeList)
            {
                re.Day = shift.Day;
                re.Dates = shift.Dates;
                re.OfDayType = shift.OfDayType;

                re.Morning = shift.Morning;
                re.Afternoon = shift.Afternoon;
                re.Night = shift.Night;
                re.EmployID = shift.EmployID;
                re.Name = shift.Name;

                db.FinalShift.Add(re);
                db.SaveChanges();
            }
        }
    }
}