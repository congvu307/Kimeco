using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kimeco_ASP.Models;

namespace Kimeco_ASP.Areas.Admin.Controllers
{
    public class CashReportsController : Controller
    {
        private KimecoEntities db = new KimecoEntities();

        // GET: Admin/CashReports
        public ActionResult Index()
        {
            return View(db.CashReports.ToList());
        }

        // GET: Admin/CashReports/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CashReport cashReport = db.CashReports.Find(id);
            if (cashReport == null)
            {
                return HttpNotFound();
            }
            return View(cashReport);
        }

        // GET: Admin/CashReports/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/CashReports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Date,ProjectName,LastMonthRemain,SubTotalInput,SubTotalOutput,BankRemain,CashInHand,ProjectTotal,CreateDate,CreateBy,Note,Status")] CashReport cashReport)
        {
            if (ModelState.IsValid)
            {
                db.CashReports.Add(cashReport);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cashReport);
        }

        // GET: Admin/CashReports/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CashReport cashReport = db.CashReports.Find(id);
            if (cashReport == null)
            {
                return HttpNotFound();
            }
            return View(cashReport);
        }

        // POST: Admin/CashReports/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Date,ProjectName,LastMonthRemain,SubTotalInput,SubTotalOutput,BankRemain,CashInHand,ProjectTotal,CreateDate,CreateBy,Note,Status")] CashReport cashReport)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cashReport).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cashReport);
        }

        // GET: Admin/CashReports/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CashReport cashReport = db.CashReports.Find(id);
            if (cashReport == null)
            {
                return HttpNotFound();
            }
            return View(cashReport);
        }

        // POST: Admin/CashReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CashReport cashReport = db.CashReports.Find(id);
            db.CashReports.Remove(cashReport);
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
