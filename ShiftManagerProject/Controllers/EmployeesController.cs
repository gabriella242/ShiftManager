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
    public class EmployeesController : Controller
    {
        private EmployeeRepository EmployRes = new EmployeeRepository();
        private ShiftManagerContext db = new ShiftManagerContext();

        public ActionResult Index()
        {
            return View(db.Employees.ToList());
        }

        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employees employees = db.Employees.Find(id);
            if (employees == null)
            {
                return HttpNotFound();
            }
            return View(employees);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employees employees)
        {
            if (!(ModelState.IsValid))
            {
                return View(employees);
            }

            if (employees.FirstName.Any(char.IsDigit))
            {
                ModelState.AddModelError("FirstName", "Letters Only");
                return View(employees);
            }
            else if(employees.LastName.Any(char.IsDigit))
            {
                ModelState.AddModelError("LastName", "Letters Only");
                return View(employees);
            }
            else if(employees.ID < 0)
            {
                ModelState.AddModelError("ID", "Positive Numbers Only");
                return View(employees);
            }

            if(EmployRes.UserCounter(employees))
            {
                ModelState.AddModelError("Email", "Employee already exist");
                return View(employees);
            }
            if(employees.NoOfShifts < 0)
            {
                ModelState.AddModelError("NoOfShifts", "Positive Numbers Only");
                return View(employees);
            }
            if (employees.Telephone.Any(char.IsLetter))
            {
                ModelState.AddModelError("Telephone", "Numbers Only");
                return View(employees);
            }

            if (ModelState.IsValid)
            {
                db.Employees.Add(employees);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(employees);
        }

        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employees employees = db.Employees.Find(id);
            if (employees == null)
            {
                return HttpNotFound();
            }
            return View(employees);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Employees employees)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employees).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(employees);
        }

        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employees employees = db.Employees.Find(id);
            if (employees == null)
            {
                return HttpNotFound();
            }
            return View(employees);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Employees employees = db.Employees.Find(id);
            ShiftPref shiftPref = db.ShiftPref.Where(x=>x.EmployID == id).FirstOrDefault(); //employee(many) -> shiftpref(one) relationship
            db.Preferences.Remove(shiftPref);
            db.SaveChanges();
            db.Employees.Remove(employees);
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
