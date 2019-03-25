using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ShiftManagerProject.DAL;
using ShiftManagerProject.Models;

namespace ShiftManagerProject.Controllers
{
    public class PrevWeeksController : Controller
    {
        private ShiftManagerContext db = new ShiftManagerContext();
        private HistoryDeletionHandler HsDelete = new HistoryDeletionHandler();
        private static List<PrevWeeks> ListForDownload = new List<PrevWeeks>();

        public ActionResult Index()
        {
            var Pcount = db.PrevWeeks.ToList();
            if (Pcount.Count()> 476)
            {
                HsDelete.PrevWeeksDeletion();
            }

            var nextshifts = db.PrevWeeks.OrderBy(r => DbFunctions.TruncateTime(r.Dates)).ThenBy(c => c.OfDayType).ToList();
            ViewBag.Employees = db.Employees.ToList();
            ListForDownload = nextshifts;
            return View(nextshifts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FormCollection form)
        {
            string Ename = form["Employees"].ToString();
            bool FromDateBool = DateTime.TryParse(form["From"].ToString(), out DateTime FromDate);
            bool ToDateBool = DateTime.TryParse(form["To"].ToString(), out DateTime ToDate);
            var ReportShifts = db.PrevWeeks.OrderBy(r => DbFunctions.TruncateTime(r.Dates)).ThenBy(c => c.OfDayType).ToList();

            if (FromDateBool && ToDateBool)
            {
                ReportShifts = ReportShifts.Where(f => f.Dates.Date >= FromDate && f.Dates.Date <= ToDate).OrderBy(r => r.Dates.Date).ThenBy(c => c.OfDayType).ToList();
            }

            if (Ename != "")
            {
                ReportShifts = ReportShifts.Where(x => x.Name == Ename).OrderBy(r => r.Dates.Date).ThenBy(c => c.OfDayType).ToList();
            }
         
            ListForDownload = ReportShifts;
            ViewBag.Employees = db.Employees.ToList();
            return View(ReportShifts);
        }

        public ActionResult LastWeek()
        {          
            HsDelete.PrevWeeksDeletion();
            HsDelete.FshiftDeletion();

            var context = ((IObjectContextAdapter)db).ObjectContext;
            var refreshableObjects = db.ChangeTracker.Entries().Select(c => c.Entity).ToList();
            context.Refresh(RefreshMode.StoreWins, refreshableObjects);

            int i = 0;
        
            var nextshifts = db.PrevWeeks.ToList();

            if (!(nextshifts.Where(y=>y.Dates.Date == DateTime.Now.Date).Any()))
            {
                ViewBag.ShiftUpdate = ++i;
            }
            else
            {
                DateTime NextSunday = DateTime.Now.AddDays(1);
                while (NextSunday.DayOfWeek != DayOfWeek.Sunday)
                { NextSunday = NextSunday.AddDays(1); }
                DateTime lastSunday = DateTime.Now;
                while (lastSunday.DayOfWeek != DayOfWeek.Sunday)
                { lastSunday = lastSunday.AddDays(-1); }             
                nextshifts = nextshifts.Where(f => f.Dates.Date < NextSunday.Date && f.Dates.Date >= lastSunday.Date).OrderBy(x => x.OfDayType).ToList();
            }
            ViewBag.ShiftUpdate = i;

            return View(nextshifts);
        }

        public ActionResult FShiftsForEmployees()
        {
            int i = 0;
            var nextshifts = db.PrevWeeks.ToList();
            DateTime NextSunday = DateTime.Now.AddDays(1);
            while (NextSunday.DayOfWeek != DayOfWeek.Sunday)
            { NextSunday = NextSunday.AddDays(1); }
            var StartDateForSecondWeek = nextshifts.Where(x => x.Dates.Date == NextSunday.Date).Select(x => x.Dates).FirstOrDefault();

            if (DateTime.Now > StartDateForSecondWeek)
            {
                ViewBag.ShiftUpdate = ++i;
            }
            else
            {
                nextshifts = nextshifts.Where(x => x.Dates.Date >= NextSunday.Date).OrderBy(x => x.OfDayType).ToList();
            }
            ViewBag.ShiftUpdate = i;

            return View(nextshifts);
        }

        public ActionResult DeleteConfirmed()
        {
            HsDelete.PrevWeeksDeletion();
            HsDelete.FshiftDeletion();
            return RedirectToAction("Index");
        }

        public FileContentResult DownloadP()
        {
            var fileDownloadName = String.Format(DateTime.Now.ToString("dd/MM/yyyy") + " Shifts.xlsx");
            const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            ExcelPackage package = GenerateExcelFile(ListForDownload.OrderBy(r => r.Dates).ThenBy(c => c.OfDayType).ToList());

            var fsr = new FileContentResult(package.GetAsByteArray(), contentType)
            {
                FileDownloadName = fileDownloadName
            };

            return fsr;
        }

        private static ExcelPackage GenerateExcelFile(IEnumerable<PrevWeeks> datasource)
        {

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(DateTime.Now.ToString("dd/MM/yyyy"));


            for (int i = 1, j = 1; i < 2; i++)
            {
                foreach (var item in datasource.ElementAt(i).GetType().GetProperties())
                {
                    if (item.Name == "ID" || item.Name == "EmployID" || item.Name == "Employees" || item.Name == "OfDayType")
                    { continue; }
                    ws.Cells[i, j++].Value = item.Name;
                }
            }

            for (int i = 0, j = 1; i < datasource.Count(); i++, j = 1)
            {
                ws.Cells[i + 2, j++].Value = datasource.ElementAt(i).Name;
                ws.Cells[i + 2, j++].Value = datasource.ElementAt(i).Day;
                ws.Cells[i + 2, j++].Value = datasource.ElementAt(i).Morning;
                ws.Cells[i + 2, j++].Value = datasource.ElementAt(i).Afternoon;
                ws.Cells[i + 2, j++].Value = datasource.ElementAt(i).Night;
                ws.Cells[i + 2, j++].Value = datasource.ElementAt(i).Dates.ToString("dd/MM/yyyy");
            }

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
