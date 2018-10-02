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
    public class CashesController : Controller
    {
        private KimecoEntities db = new KimecoEntities();

        // GET: Admin/Cashes
        [Authorize]
        public ActionResult Index()
        {
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
            return View();
        }

        // POST: Admin/Cashes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "ID,C_Date,Company,ProjectName,Staff,C_Content,Input,Output,Invoice,Ref,CreateDate,CreateBy,Status,Note")] Cash cash)
        {
            if (ModelState.IsValid)
            {
                db.Cashes.Add(cash);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cash);
        }

        // GET: Admin/Cashes/Edit/5
        [Authorize]
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
            return View(cash);
        }

        // POST: Admin/Cashes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "ID,C_Date,Company,ProjectName,Staff,C_Content,Input,Output,Invoice,Ref,CreateDate,CreateBy,Status,Note")] Cash cash)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cash).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cash);
        }

        // GET: Admin/Cashes/Delete/5
        [Authorize]
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
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cash cash = db.Cashes.Find(id);
            db.Cashes.Remove(cash);
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
