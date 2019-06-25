using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using ShiftManagerProject.DAL;
using ShiftManagerProject.Models;
using System.Net.Mail;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ShiftManagerProject.Controllers
{
    public class FinalShiftsController : Controller
    {
        private ShiftManagerContext db = new ShiftManagerContext();
        private FshiftRepository FSrespo = new FshiftRepository();
        private HistoryDeletionHandler HsDelete = new HistoryDeletionHandler();

        public ActionResult ListOfShifts()
        {
            int closed = 0;
            if (!db.FinalShift.Any())
            {
                closed = 1;
            }
            ViewBag.close = closed;
            return View(db.ShiftPref.ToList());
        }

        public ActionResult Fixed()
        {
            HsDelete.FshiftDeletion();
            FinalShift finalShift = new FinalShift
            {
                Employees = db.Employees.Where(x => x.Admin == false).ToList()
            };
            var totalshifts = db.ShiftsPerWeek.Select(o => o.NumOfShifts).FirstOrDefault();
            DateTime NextSunday = DateTime.Now.AddDays(1);
            while (NextSunday.DayOfWeek != DayOfWeek.Sunday)
            { NextSunday = NextSunday.AddDays(1); }

            var listofFixed = new List<FinalShift>();
            if ((db.FinalShift.Where(y => y.Dates > NextSunday.Date).Count() <= totalshifts / 2) && db.FinalShift.Count() > 0)
            {
                foreach (var shift in db.FinalShift.Where(y => y.Dates > NextSunday.Date).ToList())
                {
                    listofFixed.Add(shift);
                }
            }
            ViewBag.FixedList = listofFixed;
            return View(finalShift);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Fixed(FinalShift FixedShift)
        {
            if((FixedShift.Morning == true &&  FixedShift.Afternoon == true) ||
                (FixedShift.Morning == true && FixedShift.Night == true) || 
                (FixedShift.Afternoon == true && FixedShift.Night == true) || 
                (FixedShift.Morning == true && FixedShift.Afternoon == true && FixedShift.Night == true))
            {
                ModelState.AddModelError("Night", "Choose one shift");
                FixedShift.Employees = db.Employees.Where(x => x.Admin == false).ToList(); ;
                return View(FixedShift);
            }

            var employ = db.Employees.Where(x => x.Admin == false).ToList();
            var totalshifts = db.ShiftsPerWeek.Select(o => o.NumOfShifts).FirstOrDefault();
            DateTime NextSunday = DateTime.Now.AddDays(1);
            while (NextSunday.DayOfWeek != DayOfWeek.Sunday)
            { NextSunday = NextSunday.AddDays(1); }
            var listofFixed = new List<FinalShift>();
            if ((db.FinalShift.Where(s => s.Dates > NextSunday.Date).Count() <= totalshifts / 2) && db.FinalShift.Count() > 0)
            {
                foreach (var shift in db.FinalShift.Where(e => e.Dates > NextSunday.Date).ToList())
                {
                    listofFixed.Add(shift);
                }
            }

            ViewBag.FixedList = listofFixed;
            var xyz = listofFixed.Where(w=>w.Day == FixedShift.Day && w.Name == FixedShift.Name).Count();


            if (xyz > 0)
            {
                ModelState.AddModelError("Name", "This employee has already been placed on this day");
                FixedShift.Employees = employ;
                return View(FixedShift);
            }

            if (FixedShift.Morning == true)
            {
                var x = db.ScheduleParameters.Select(c => c.Morning).FirstOrDefault();
                var w = db.ScheduleParameters.Where(d => d.Day == FixedShift.Day).Select(d => d.DMorning).FirstOrDefault();
                if(w == null) { w = 0; }
                var p = db.FinalShift.Where(f => f.Day == FixedShift.Day && f.Morning == true).Count();
                if (p >= (x + w))
                {
                    ModelState.AddModelError("Night", "Maximum employees per shift. Please update the settings to allow more employees");
                    FixedShift.Employees = employ;
                    return View(FixedShift);
                }

            }
            else if(FixedShift.Afternoon == true)
            {
                var x = db.ScheduleParameters.Select(c => c.Afternoon).FirstOrDefault();
                var w = db.ScheduleParameters.Where(d => d.Day == FixedShift.Day).Select(d => d.DAfternoon).FirstOrDefault();
                if (w == null) { w = 0; }
                var p = db.FinalShift.Where(f => f.Day == FixedShift.Day && f.Afternoon == true).Count();
                if (p >= (x + w))
                {
                    ModelState.AddModelError("Night", "Maximum employees per shift, please update the settings to allow more employees");
                    FixedShift.Employees = employ;
                    return View(FixedShift);
                }

            }
            else
            {
                var x = db.ScheduleParameters.Select(c => c.Night).FirstOrDefault();
                var w = db.ScheduleParameters.Where(d => d.Day == FixedShift.Day).Select(d => d.DNight).FirstOrDefault();
                if (w == null) { w = 0; }
                var p = db.FinalShift.Where(f => f.Day == FixedShift.Day && f.Night == true).Count();
                if (p >= (x + w))
                {
                    ModelState.AddModelError("Night", "Maximum employees per shift, please update the settings to allow more employees");
                    FixedShift.Employees = employ;
                    return View(FixedShift);
                }

            }

            bool flag = false;
            int i, y;
            Employees Emp = db.Employees.FirstOrDefault(x => x.FirstName == FixedShift.Name);
            for (i = 1; FSrespo.DayOfWeek(i) != FixedShift.Day; i++) ;
            y = FixedShift.Morning == true ? 0 : FixedShift.Afternoon == true ? 2 : 3;

            DayOfWeek day = new System.DayOfWeek();
            string dday = FSrespo.DayOfWeek(i);

            for (int d = 0; d < 7; d++)
            {
                if (day.ToString() != dday)
                {
                    day = (DayOfWeek)((d + 1) % 7);
                }
                else { break; }
            }

            FixedShift.Dates = FSrespo.NextWeeksDates(day);

            if (db.FinalShift.Count() <= totalshifts) { FSrespo.OfDayHandler(true, 0, 0); }

            try
            {
                if (Emp != null && ModelState.IsValid)
                {
                    FixedShift.EmployID = Emp.ID;
                    flag = true;
                    FixedShift.OfDayType = FSrespo.OrderOfDayTypeHandler(i, y);
                    db.FinalShift.Add(FixedShift);
                    db.SaveChanges();
                    return RedirectToAction("Fixed");
                }
            }
            catch
            {
                if (flag)
                {
                    ModelState.AddModelError("Name", "Error Saving this shift");
                    FixedShift.Employees = db.Employees.ToList();
                    return View(FixedShift);
                }
                ModelState.AddModelError("Name", "Employee's ID was not found or matched");
                FixedShift.Employees = db.Employees.ToList();
                return View(FixedShift);
            }

            return View(FixedShift);
        }

        public ActionResult Index()
        {
            int ad = 0;
            var totalshifts = db.ShiftsPerWeek.Select(o => o.NumOfShifts).FirstOrDefault();
            var pweeks = db.History.OrderByDescending(x => x.ID).Take(totalshifts).OrderBy(w => w.OfDayType).Select(k => k.EmployID).ToList();
            var fweeks = db.FinalShift.OrderBy(q => q.OfDayType).Select(k => k.EmployID).ToList();
            if (pweeks.SequenceEqual(fweeks))
            {
                ad = 1;
            }

            ViewBag.admin = ad;

            var nextshifts = db.FinalShift.OrderBy(x => x.OfDayType).ToList();
            return View(nextshifts);
        }

        public ActionResult NewClose()
        {
            HsDelete.HistoryDeletion();
            HsDelete.SpecialFixedFshiftDeletion();

            FSrespo.NewLChecker();
            //FSrespo.LeastShiftPref();
            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FinalShift finalShift)
        {
            if (ModelState.IsValid)
            {
                db.FinalShift.Add(finalShift);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        public ActionResult Send()
        {
            FSrespo.PrevShiftsRotation();
            HsDelete.HistoryDeletion();
            string ShiftsBody = null;
            foreach (var shift in db.FinalShift.Where(d => d.EmployID == 327339339).ToList())
            {
                ShiftsBody += shift.Dates.ToString("dd/MM/yyyy") + " -> " + shift.Day + " " + (shift.Morning == true ? "Morning" : shift.Afternoon == true ? "Afternoon" : "Night") + ", ";
            }

            const string accountSid = "ACf5f149a9c569d36575a2320d60ed4281";
            const string authToken = "794bf494659491d04411de9f0ce64d72";
            TwilioClient.Init(accountSid, authToken);
            var mediaUrl = 
            new Uri( "https://climacons.herokuapp.com/clear.png" );
            var to = new PhoneNumber("  PHONER NUMBER ");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                | SecurityProtocolType.Tls11
                                                | SecurityProtocolType.Tls12
                                                | SecurityProtocolType.Ssl3;
            var message = MessageResource.Create(
              to: to,
              from: new PhoneNumber("+18646101609"),
              body: "Your shifts for next week are: " + ShiftsBody);
            Console.WriteLine(message.Sid);
            //var totalshifts = db.ShiftsPerWeek.Select(o => o.NumOfShifts).FirstOrDefault();

            //DateTime nextSunday = DateTime.Now.AddDays(1);
            //while (nextSunday.DayOfWeek != DayOfWeek.Sunday)
            //{ nextSunday = nextSunday.AddDays(1); }
            //var NextWeek = Convert.ToDateTime(nextSunday).ToString("dd/MM/yyyy");
            //var shifts = db.PrevWeeks.ToList().OrderByDescending(k => k.ID).Take(totalshifts).OrderBy(x => x.OfDayType);
            //var empList = new List<Employees>();

            //var smtp = new SmtpClient
            //{
            //    Host = "smtp.gmail.com",
            //    Port = 587,
            //    EnableSsl = true,
            //    DeliveryMethod = SmtpDeliveryMethod.Network,
            //    UseDefaultCredentials = false,
            //    Credentials = new NetworkCredential(" YOUR EMAIL", " YOUR PASSWORD")
            //};

            //foreach(var emp in db.Employees.ToList())
            //{
            //    if(shifts.Where(x=>x.EmployID == emp.ID).Any())
            //    {
            //        empList.Add(emp);
            //    }
            //}

            //foreach (var employee in empList)
            //{
            //    MailMessage mailMessage = new MailMessage
            //    {
            //        From = new MailAddress(" YOUR EMAIL ")
            //    };
            //    mailMessage.To.Add(new MailAddress(employee.Email));
            //    mailMessage.Subject = "Your work schedule has been updated!";
            //    string body = "<table>" + "<h3>" + "Your work schedule has been updated for week " + NextWeek + "</h3>";
            //    foreach (var shift in shifts.Where(x => x.EmployID == employee.ID).OrderBy(d => d.OfDayType))
            //    {
            //        body += "<tr>";
            //        body += "<th>" + shift.Day + " " + "</th>";
            //        body += "<td>" + (shift.Morning == true ? " Morning" : shift.Afternoon == true ? " Afternoon" : " Night") + "</td>";
            //        body += "</tr>";
            //    }
            //    body += "</table>";
            //    mailMessage.Body = body;
            //    mailMessage.IsBodyHtml = true;
            //    smtp.Send(mailMessage);
            //}

            return RedirectToAction("Index");
        }

        public ActionResult Edit(long? id, bool? confirmed)
        {
            int x;
            List<Employees> EmployeeList = new List<Employees>();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FinalShift finalShift = db.FinalShift.Find(id);
            if (finalShift == null)
            {
                return HttpNotFound();
            }

            if (confirmed == true)
            {
                finalShift.Employees = db.Employees.ToList();
                return View(finalShift);
            }

            for (x = 0; FSrespo.DayOfWeek(x) != finalShift.Day; x++) ;
            string y = finalShift.Morning == true ? "M" : finalShift.Afternoon == true ? "A" : "N";

            if (confirmed == false)
            {
                foreach (var eight in FSrespo.AvailableEightEightEmployees(x, y, FSrespo.AvailableEmployees(x, y)).ToList())
                {
                    foreach (var worker in db.Employees.ToList())
                    {
                        if (eight == worker.FirstName)
                        {
                            EmployeeList.Add(worker);
                        }
                    }
                }

                finalShift.Employees = EmployeeList.ToList();
                return View(finalShift);
            }

            foreach (var avail in FSrespo.AvailableEmployees(x, y))
            {
                foreach (var worker in db.Employees.ToList())
                {
                    if (avail == worker.FirstName)
                    {
                        if (!FSrespo.PreviousShifts(x, y, worker))
                        {
                            if (!(FSrespo.FutureShifts(x, y, worker)))
                            {
                                EmployeeList.Add(worker);
                            }
                        }
                    }
                }
            }

            finalShift.Employees = EmployeeList.ToList();
            return View(finalShift);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FinalShift finalShift)
        {
            foreach (var worker in db.Employees)
            {
                if (finalShift.Name == worker.FirstName)
                {
                    finalShift.EmployID = worker.ID;
                    break;
                }
            }

            if (ModelState.IsValid)
            {
                db.Entry(finalShift).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            finalShift.Employees = db.Employees.ToList();
            return View(finalShift);
        }

        public ActionResult SaveToRemake()
        {
            HsDelete.SavedScheduleDeletion();
            FSrespo.SaveToSavedScheduleTBL();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult SavedSchedule()
        {
            int ad = 0;
            var totalshifts = db.ShiftsPerWeek.Select(o => o.NumOfShifts).FirstOrDefault();
            var sweeks = db.SavedSchedule.Take(totalshifts).OrderBy(w => w.OfDayType).Select(k => k.EmployID).ToList();
            var fweeks = db.FinalShift.OrderBy(q => q.OfDayType).Select(k => k.EmployID).ToList();
            if (sweeks.SequenceEqual(fweeks))
            {
                ad = 1;
            }

            ViewBag.admin = ad;

            ViewBag.msg = db.SavedSchedule.Select(g => g.OfDayType).Count() == totalshifts ? 1 : 0;
            var nextshifts = db.SavedSchedule.OrderBy(x => x.OfDayType).ToList();
            return View(nextshifts);
        }

        public ActionResult SaveTheSchedule(bool id)
        {
            if (id)
            {
                HsDelete.SpecialFixedFshiftDeletion();
                FSrespo.SaveRemakeToFinalTBL();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FinalShift finalShift = db.FinalShift.Find(id);
            if (finalShift == null)
            {
                return HttpNotFound();
            }
            return View(finalShift);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            FinalShift finalShift = db.FinalShift.Find(id);
            db.FinalShift.Remove(finalShift);
            db.SaveChanges();
            return RedirectToAction("Fixed");
        }


        public FileContentResult Download()
        {
            var fileDownloadName = String.Format(DateTime.Now.ToString("dd/MM/yyyy") + " Shifts.xlsx");
            const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            // Pass ef data to method
            ExcelPackage package = GenerateExcelFile(db.FinalShift.OrderBy(x => x.OfDayType).ToList());

            var fsr = new FileContentResult(package.GetAsByteArray(), contentType)
            {
                FileDownloadName = fileDownloadName
            };

            return fsr;
        }

        private static ExcelPackage GenerateExcelFile(IEnumerable<FinalShift> datasource)
        {

            ExcelPackage pck = new ExcelPackage();

            //Create the worksheet 
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(DateTime.Now.ToString("dd/MM/yyyy"));

            // Sets Headers
            for (int i = 1, j = 1; i < 2; i++)
            {
                foreach (var item in datasource.ElementAt(i).GetType().GetProperties())
                {
                    if(item.Name == "ID" || item.Name == "EmployID" || item.Name == "Employees" || item.Name == "OfDayType")
                    { continue; }
                    ws.Cells[i, j++].Value = item.Name;
                }
            }

            for (int i = 0, j = 1; i < datasource.Count(); i++, j= 1)
            {
                ws.Cells[i + 2, j++].Value = datasource.ElementAt(i).Name;
                ws.Cells[i + 2, j++].Value = datasource.ElementAt(i).Day;
                ws.Cells[i + 2, j++].Value = datasource.ElementAt(i).Morning;
                ws.Cells[i + 2, j++].Value = datasource.ElementAt(i).Afternoon;
                ws.Cells[i + 2, j++].Value = datasource.ElementAt(i).Night;
                ws.Cells[i + 2, j++].Value = datasource.ElementAt(i).Dates.ToString("dd/MM/yyyy");
            }

            // Format Header of Table
            using (ExcelRange rng = ws.Cells["A1:M1"])
            {
                rng.Style.Font.Bold = true;
            }

            using (ExcelRange rng = ws.Cells)
            {
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.AutoFitColumns();
            }

            return pck;
        }

        public ActionResult Message(string employees, string new_message, Boolean? sms, Boolean? email)
        {
            if(sms == true)
            {
                const string accountSid = "ACf5f149a9c569d36575a2320d60ed4281";
                const string authToken = "794bf494659491d04411de9f0ce64d72";
                TwilioClient.Init(accountSid, authToken);
                var mediaUrl =
                new Uri("https://climacons.herokuapp.com/clear.png");
                var to = new PhoneNumber(" YOUR TELEPHONE NUMBER ");
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                    | SecurityProtocolType.Tls11
                                                    | SecurityProtocolType.Tls12
                                                    | SecurityProtocolType.Ssl3;
                var message = MessageResource.Create(
                  to: to,
                  from: new PhoneNumber("+18646101609"),
                  body: "Important message from the manager:  " + new_message);
                Console.WriteLine(message.Sid);
            }
            if(email == true)
            {
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(" YOUR EMAIL", "YOUR PASSWORD ")
                };


                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress("YOUR EMAIL ")
                    };
                    mailMessage.To.Add(new MailAddress(db.Employees.Where(d => d.FirstName == employees).Select(d => d.Email).FirstOrDefault()));
                    mailMessage.Subject = "Important message from the manager!";
                    mailMessage.Body = new_message;
                    smtp.Send(mailMessage);
            }

            return RedirectToAction("ListOfShifts");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
