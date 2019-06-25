using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShiftManagerProject.DAL;
using ShiftManagerProject.Models;

namespace ShiftManagerProject.Controllers
{
    public class ScheduleParametersController : Controller
    {
        private ShiftManagerContext db = new ShiftManagerContext();
        private HistoryDeletionHandler HsDelete = new HistoryDeletionHandler();

        public ActionResult Index()
        {
            if (db.ScheduleParameters.Where(x => x.Day == null).Any())
            {
                ViewBag.WeekP = 1;
            }
            return View(db.ScheduleParameters.ToList());
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduleParameters scheduleParameters = db.ScheduleParameters.Find(id);
            if (scheduleParameters == null)
            {
                return HttpNotFound();
            }
            return View(scheduleParameters);
        }


        public ActionResult DayCreate(bool? whattodo, bool? require, bool? dayexists)
        {
            if(dayexists == true)
            {
                ModelState.AddModelError("Day", "Day already exists! Please add all additions to existing day");
                return View();
            }
            if(require == true)
            {
                ModelState.AddModelError("Day", "Day is Required");
                return View();
            }
            if(whattodo == true)
            {
                ModelState.AddModelError("Day", "Up to 7 days are allowed to be added!");
            }
            else if(whattodo == false)
            {
                ModelState.AddModelError("DMorning", "Positive Numbers only");
            }
            return View();
        }

        public ActionResult WeekCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ScheduleParameters scheduleParameters)
        {
            int TotalShifts = 0;
            int idnum = 0;
            if (Convert.ToInt32(scheduleParameters.DMorning) < 0 || Convert.ToInt32(scheduleParameters.DAfternoon) < 0 || Convert.ToInt32(scheduleParameters.DNight) < 0)
            {
                return RedirectToAction("DayCreate", new { whattodo = false });
            }
            
            if (scheduleParameters.Day != null && db.ScheduleParameters.Select(o => o.Day != null).Count() > 7)
            {
                return RedirectToAction("DayCreate", new { whattodo = true });
            }
            if(scheduleParameters.Day == null & (Convert.ToInt32(scheduleParameters.DMorning) != 0 || Convert.ToInt32(scheduleParameters.DAfternoon) != 0 || Convert.ToInt32(scheduleParameters.DNight) != 0))
            {
                return RedirectToAction("DayCreate", new { whattodo = true, require = true });
            }
            if(db.ScheduleParameters.Where(d=>d.Day == scheduleParameters.Day).Count() > 0)
            {
                return RedirectToAction("DayCreate", new { dayexists = true });
            }
           
            HsDelete.SpecialFixedFshiftDeletion();
            if (db.ShiftsPerWeek.Any())
            {
                idnum = db.ShiftsPerWeek.FirstOrDefault().ID;
            }

            if (scheduleParameters.Day == null)
            {
                if (idnum == 0)
                {
                    ShiftsPerWeek SPW = new ShiftsPerWeek
                    {
                        NumOfShifts = (scheduleParameters.Morning + scheduleParameters.Afternoon + scheduleParameters.Night) * 7
                    };
                    db.ShiftsPerWeek.Add(SPW);
                    db.SaveChanges();
                }
                else
                {
                    ShiftsPerWeek EditOldShift = db.ShiftsPerWeek.Find(idnum);
                    EditOldShift.NumOfShifts = (scheduleParameters.Morning + scheduleParameters.Afternoon + scheduleParameters.Night) * 7;
                    db.Entry(EditOldShift).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            else
            {
                scheduleParameters.DMorning = scheduleParameters.DMorning == null ? 0 : scheduleParameters.DMorning;
                scheduleParameters.DAfternoon = scheduleParameters.DAfternoon == null ? 0 : scheduleParameters.DAfternoon;
                scheduleParameters.DNight = scheduleParameters.DNight == null ? 0 : scheduleParameters.DNight;

                TotalShifts += Convert.ToInt32(scheduleParameters.DMorning);
                TotalShifts += Convert.ToInt32(scheduleParameters.DAfternoon);
                TotalShifts += Convert.ToInt32(scheduleParameters.DNight);

                TotalShifts += db.ShiftsPerWeek.Select(y => y.NumOfShifts).FirstOrDefault();
                ShiftsPerWeek SperW = db.ShiftsPerWeek.Find(idnum);
                SperW.NumOfShifts = TotalShifts;

                db.Entry(SperW).State = EntityState.Modified;
                db.SaveChanges();
            }

            if (ModelState.IsValid)
            {
                db.ScheduleParameters.Add(scheduleParameters);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(scheduleParameters);
        }

        public ActionResult EditWeek(int? id, bool? whattodo)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduleParameters scheduleParameters = db.ScheduleParameters.Find(id);
            if (scheduleParameters == null)
            {
                return HttpNotFound();
            }
            if (whattodo == true)
            {
                ModelState.AddModelError("Morning", "Positive Numbers only");
                return View(scheduleParameters);
            }

            return View(scheduleParameters);
        }

        public ActionResult EditDay(int? id, bool? whattodo)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduleParameters scheduleParameters = db.ScheduleParameters.Find(id);
            if (scheduleParameters == null)
            {
                return HttpNotFound();
            }
            if (whattodo == true)
            {
                ModelState.AddModelError("DMorning", "Positive Numbers only");
                return View(scheduleParameters);
            }

            return View(scheduleParameters);
        }

        public ActionResult EditMax(int? id, bool? whattodo)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduleParameters scheduleParameters = db.ScheduleParameters.Find(id);
            if (scheduleParameters == null)
            {
                return HttpNotFound();
            }

            if (whattodo == true)
            {
                ModelState.AddModelError("MaxMorning", "Positive Numbers only");
                return View(scheduleParameters);
            }

            return View(scheduleParameters);
        }

        [HttpGet, ActionName("Edit")]
        public ActionResult EditChoice(int? id, bool? WhichEdit)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduleParameters scheduleParameters = db.ScheduleParameters.Find(id);
            if (scheduleParameters == null)
            {
                return HttpNotFound();
            }

            if (WhichEdit == true)
            {
                return RedirectToAction("EditMax", new { ID = id });
            }

            if (scheduleParameters.Day == null)
            {
                return RedirectToAction("EditWeek", new { ID = id });
            }
            else
            {
                return RedirectToAction("EditDay", new { ID = id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ScheduleParameters scheduleParameters, bool? WhichEdit)
        {
            HsDelete.SpecialFixedFshiftDeletion();

            if (WhichEdit == true)
            {
                scheduleParameters.Morning = db.ScheduleParameters.Select(x => x.Morning).FirstOrDefault();
                scheduleParameters.Afternoon = db.ScheduleParameters.Select(x => x.Afternoon).FirstOrDefault();
                scheduleParameters.Night = db.ScheduleParameters.Select(x => x.Night).FirstOrDefault();

                if (scheduleParameters.MaxMorning < 0 || scheduleParameters.MaxAfternoon < 0 || scheduleParameters.MaxNight < 0)
                {
                    return RedirectToAction("EditMax", new { scheduleParameters.ID, whattodo = true });
                }
            }

            if (scheduleParameters.Day == null && WhichEdit == null)
            {
                if (scheduleParameters.Morning < 0 || scheduleParameters.Afternoon < 0 || scheduleParameters.Night < 0)
                {
                    return RedirectToAction("EditWeek", new { scheduleParameters.ID, whattodo = true });
                }

                scheduleParameters.MaxMorning = db.ScheduleParameters.Select(x => x.MaxMorning).FirstOrDefault();
                scheduleParameters.MaxAfternoon = db.ScheduleParameters.Select(x => x.MaxAfternoon).FirstOrDefault();
                scheduleParameters.MaxNight = db.ScheduleParameters.Select(x => x.MaxNight).FirstOrDefault();

                if (scheduleParameters.Morning != db.ScheduleParameters.Select(x => x.Morning).FirstOrDefault())
                {
                    ShiftsPerWeek SperW = db.ShiftsPerWeek.FirstOrDefault();
                    SperW.NumOfShifts += (scheduleParameters.Morning - db.ScheduleParameters.Select(x => x.Morning).FirstOrDefault()) * 7;

                    db.Entry(SperW).State = EntityState.Modified;
                    db.SaveChanges();
                }

                if (scheduleParameters.Afternoon != db.ScheduleParameters.Select(x => x.Afternoon).FirstOrDefault())
                {
                    ShiftsPerWeek SperW = db.ShiftsPerWeek.FirstOrDefault();
                    SperW.NumOfShifts += (scheduleParameters.Afternoon - db.ScheduleParameters.Select(x => x.Afternoon).FirstOrDefault()) * 7;

                    db.Entry(SperW).State = EntityState.Modified;
                    db.SaveChanges();
                }

                if (scheduleParameters.Night != db.ScheduleParameters.Select(x => x.Night).FirstOrDefault())
                {
                    ShiftsPerWeek SperW = db.ShiftsPerWeek.FirstOrDefault();
                    SperW.NumOfShifts += (scheduleParameters.Night - db.ScheduleParameters.Select(x => x.Night).FirstOrDefault()) * 7;

                    db.Entry(SperW).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            else if (scheduleParameters.Day != null && WhichEdit == null)
            {
                if (scheduleParameters.DMorning < 0 || scheduleParameters.DAfternoon < 0 || scheduleParameters.DNight < 0)
                {
                    return RedirectToAction("EditDay", new { scheduleParameters.ID, whattodo = true });
                }

                if (scheduleParameters.DMorning != 0)
                {
                    ShiftsPerWeek SperW = db.ShiftsPerWeek.FirstOrDefault();
                    SperW.NumOfShifts += Convert.ToInt32(scheduleParameters.DMorning);
                    db.Entry(SperW).State = EntityState.Modified;
                    db.SaveChanges();
                }

                if (scheduleParameters.DAfternoon != 0)
                {
                    ShiftsPerWeek SperW = db.ShiftsPerWeek.FirstOrDefault();
                    SperW.NumOfShifts += Convert.ToInt32(scheduleParameters.DAfternoon);

                    db.Entry(SperW).State = EntityState.Modified;
                    db.SaveChanges();
                }

                if (scheduleParameters.DNight != 0)
                {
                    ShiftsPerWeek SperW = db.ShiftsPerWeek.FirstOrDefault();
                    SperW.NumOfShifts += Convert.ToInt32(scheduleParameters.DNight);

                    db.Entry(SperW).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            if (ModelState.IsValid)
            {
                db.Entry(scheduleParameters).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(scheduleParameters);
        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduleParameters scheduleParameters = db.ScheduleParameters.Find(id);
            if (scheduleParameters == null)
            {
                return HttpNotFound();
            }
            return View(scheduleParameters);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ScheduleParameters scheduleParameters = db.ScheduleParameters.Find(id);
            ShiftsPerWeek SperW = db.ShiftsPerWeek.FirstOrDefault();
            HsDelete.SpecialFixedFshiftDeletion();
            if (scheduleParameters.Day == null)
            {
                SperW.NumOfShifts -= scheduleParameters.Morning * 7;
                SperW.NumOfShifts -= scheduleParameters.Afternoon * 7;
                SperW.NumOfShifts -= scheduleParameters.Night * 7;

                db.Entry(SperW).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                SperW.NumOfShifts -= Convert.ToInt32(scheduleParameters.DMorning);
                SperW.NumOfShifts -= Convert.ToInt32(scheduleParameters.DAfternoon);
                SperW.NumOfShifts -= Convert.ToInt32(scheduleParameters.DNight);

                db.Entry(SperW).State = EntityState.Modified;
                db.SaveChanges();
            }

            db.ScheduleParameters.Remove(scheduleParameters);
            db.SaveChanges();
            return RedirectToAction("Index");
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
