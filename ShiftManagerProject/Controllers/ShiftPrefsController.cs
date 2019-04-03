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
    public class ShiftPrefsController : Controller
    {
        private ShiftManagerContext db = new ShiftManagerContext();

        public ActionResult Index(long? ID)
        {
            if(ID==null)
            {
                if ((string)Session["UserSess"] != null)
                {
                    var name = (string)Session["UserSess"];
                    Employees shift = db.Employees.FirstOrDefault(x => x.FirstName == name);
                    if(shift!=null)
                    {
                        ID = shift.ID;
                    }
                }
            }

            if(ID!=null)
            {
                var employee = db.ShiftPref.Where(u => u.EmployID == ID);

                if (!(db.ShiftPref.Any(u => u.EmployID == ID)))
                {
                    return RedirectToAction("Create", new { id = ID });
                }
                return View(employee);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Create(long? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!(db.ShiftPref.Any(x => x.EmployID == id)))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ShiftPref shiftPref)
        {
            var employ = db.Employees.Find(shiftPref.EmployID);
            shiftPref.Name = employ.FirstName;

            if (ModelState.IsValid)
            {
                db.Preferences.Add(shiftPref);
                db.SaveChanges();
                return RedirectToAction("Index", new { Id = shiftPref.EmployID});
            }

            return View(shiftPref);
        }

        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShiftPref shiftPref = db.ShiftPref.Find(id);
            if (shiftPref == null)
            {
                return HttpNotFound();
            }
            return View(shiftPref);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ShiftPref shiftPref)
        {
            ShiftPref shift = db.ShiftPref.Find(shiftPref.ID);
            shiftPref.EmployID = shift.EmployID;
            shiftPref.Name = shift.Name;

            var result = db.ShiftPref.SingleOrDefault(b => b.ID == shiftPref.ID);

            if (result != null && ModelState.IsValid)
            {
                db.Entry(result).CurrentValues.SetValues(shiftPref);
                db.SaveChanges();

                if ((bool)System.Web.HttpContext.Current.Session["admin"]== true)
                {
                    return RedirectToAction("ListOfShifts", "FinalShifts");
                }
                else
                {
                    return RedirectToAction("Index", "ShiftPrefs");
                }
            }
            return View(shiftPref);
        }

        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShiftPref shiftPref = db.ShiftPref.Find(id);
            if (shiftPref == null)
            {
                return HttpNotFound();
            }
            return View(shiftPref);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ShiftPref shiftPref = db.ShiftPref.Find(id);
            db.Preferences.Remove(shiftPref);
            db.SaveChanges();
            return RedirectToAction("Index", new { ID = shiftPref.EmployID });
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
