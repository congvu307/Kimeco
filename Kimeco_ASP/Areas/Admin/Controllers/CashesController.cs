using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kimeco_ASP.Models;

namespace Kimeco_ASP.Areas.Admin.Controllers
{
    public class CashesController : Controller
    {
        private KimecoEntities db = new KimecoEntities();
        // GET: Admin/Cashes
        [Authorize(Roles ="Admin")]
        [HttpGet]
        public ActionResult Spending()
        {
            var listCashSpending = db.Cashes.Where(x => x.Status == false).OrderBy(x => x.C_Date).ToList();
            return View(listCashSpending);
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public ActionResult Approve(int? cash_spending_id)
        {
            if (cash_spending_id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cash cash = db.Cashes.Find(cash_spending_id);
            if (cash == null)
            {
                return HttpNotFound();
            }
            var isExistDate = db.CashReports.Any(x => x.Date == cash.C_Date && x.ProjectName == cash.ProjectName);
            if (isExistDate)
            {
                var this_Date = db.CashReports.FirstOrDefault(x => x.Date == cash.C_Date && x.ProjectName == cash.ProjectName);
                if (this_Date != null)
                {
                    this_Date.SubTotalInput += cash.Input;
                }
            }
            else
            {
                var thisMonth_Report = db.CashReports.Where(x => x.Date.Year == cash.C_Date.Year && x.Date.Month == cash.C_Date.Month).OrderBy(x=>x.Date).ToList();
                var sumInput = db.Cashes.Where(x => x.C_Date.Month == cash.C_Date.Month && x.C_Date.Year == cash.C_Date.Year && x.C_Date.Day < cash.C_Date.Day).Sum(y => y.Input);
                var sumOutput = db.Cashes.Where(x => x.C_Date.Month == cash.C_Date.Month && x.C_Date.Year == cash.C_Date.Year && x.C_Date.Day < cash.C_Date.Day).Sum(y => y.Output);
                var decimal_sumInput = sumInput == null? 0 :sumInput;
                var decimal_sumOutput = sumOutput == null ? 0 : sumOutput;
                CashReport cashreport = new CashReport();
                cashreport.Date = cash.C_Date;
                cashreport.ProjectName = cash.ProjectName;
                cashreport.SubTotalOutput = decimal_sumOutput + cash.Output;
                cashreport.SubTotalInput = decimal_sumInput + cash.Input;
                cashreport.CreateDate = DateTime.Now;
                cashreport.Status = true;
                cashreport.CreateBy = Request.Cookies["username"] == null ? "" : Request.Cookies["username"].Value;
                if (cash.C_Date.Day != 1)
                {
                    CashReport firstDate = thisMonth_Report.FirstOrDefault();
                    if (firstDate != null)
                    {
                        cashreport.LastMonthRemain = firstDate.LastMonthRemain;
                    }
                }
                else
                {
                    if (cash.C_Date.Month != 1)
                    {
                        var thismouth = db.CashReports.Where(x => x.Date.Year == cash.C_Date.Year && x.Date.Month == (cash.C_Date.Month - 1)).OrderByDescending(x=>x.Date).FirstOrDefault();
                        if (thismouth != null)
                        {
                            cashreport.LastMonthRemain = thismouth.LastMonthRemain;
                        }
                    }
                    else
                    {
                        var thismouth = db.CashReports.Where(x => x.Date.Year == (cash.C_Date.Year - 1) && x.Date.Month == 12).FirstOrDefault();
                        if (thismouth != null)
                        {
                            cashreport.LastMonthRemain = thismouth.LastMonthRemain;
                        }
                    }
                }
                db.CashReports.Add(cashreport);
            }
            cash.Status = true;
            db.SaveChanges();
            return RedirectToAction("Spending");
        }

        [Authorize]
        public ActionResult Index()
        {
            ViewBag.NumberOfSpending = db.Cashes.Where(x => x.Status == false).Count();
            ViewData["listProject"] = db.Projects.Where(x=>x.Status == true && x.Note != "Disable").ToList();
            return View(db.Cashes.ToList());
        }

        

        // GET: Admin/Cashes/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cash cash = db.Cashes.Find(id);
            if (cash == null)
            {
                return HttpNotFound();
            }
            return View(cash);
        }

        // GET: Admin/Cashes/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewData["listproject"] = db.Projects.ToList();
            return View();
        }

        // POST: Admin/Cashes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "ID,C_Date,Company,ProjectName,Staff,C_Content,Input,Output,Invoice,Ref,CreateDate,CreateBy,Status,Note")] Cash cash,string stringDay)
        {
            if (ModelState.IsValid)
            {
                cash.C_Date = DateTime.ParseExact(stringDay, "d/M/yyyy", CultureInfo.InvariantCulture);
                cash.CreateDate = DateTime.Now;
                cash.Status = false;
                if (Request.Cookies["username"] != null)
                {
                    cash.CreateBy = Request.Cookies["username"].Value;
                }
                db.Cashes.Add(cash);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index","Cashes");
        }

        // GET: Admin/Cashes/Edit/5
        [Authorize(Roles ="Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cash cash = db.Cashes.Find(id);
            if (cash == null)
            {
                return HttpNotFound();
            }
            ViewBag.Date = cash.C_Date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            ViewData["listProject"] = db.Projects.Where(x => x.Status == true && x.Note != "Disable").ToList();
            return View(cash);
        }

        // POST: Admin/Cashes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "ID,C_Date,Company,ProjectName,Staff,C_Content,Input,Output,Invoice,Ref,CreateDate,CreateBy,Status,Note")] Cash cash,string stringDay)
        {
            if (ModelState.IsValid)
            {
                cash.C_Date = DateTime.ParseExact(stringDay, "d/M/yyyy", CultureInfo.InvariantCulture);
                cash.CreateDate = DateTime.Now;
                db.Entry(cash).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cash);
        }

        // GET: Admin/Cashes/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cash cash = db.Cashes.Find(id);
            if (cash == null)
            {
                return HttpNotFound();
            }
            return View(cash);
        }

        // POST: Admin/Cashes/Delete/5
        [Authorize(Roles ="Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cash cash = db.Cashes.Find(id);
            cash.Note = "Disable";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public FileResult Download_Template()
        {
            var filename = "InputCash_Template.xlsx";
            string fileDir = System.IO.Path.Combine(Server.MapPath("~/App_Data/templates"), filename);
            return File(fileDir, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
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
