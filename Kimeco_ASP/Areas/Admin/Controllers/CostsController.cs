using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kimeco_ASP.Models;

namespace Kimeco_ASP.Areas.Admin.Controllers
{
    public class CostsController : Controller
    {
        private KimecoEntities db = new KimecoEntities();

        [HttpGet]
        public FileResult TemplateUpdate()
        {
            string fileDir = Path.Combine(Server.MapPath("~/App_Data/templates"), "SiteCost_Template.xlsx");
            return File(fileDir, System.Net.Mime.MediaTypeNames.Application.Octet, "SiteCost_Template.xlsx");
        }
        // GET: Admin/Costs
        public ActionResult Index()
        {
            return View(db.Costs.ToList());
        }

        // GET: Admin/Costs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cost cost = db.Costs.Find(id);
            if (cost == null)
            {
                return HttpNotFound();
            }
            return View(cost);
        }

        // GET: Admin/Costs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Costs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Item,Unit,UnitPrice,SubTotal,Date,ProjectID,Conpany,Tax,CreateDate,CreateBy,Status,Note,Quantity,VAT,Total")] Cost cost)
        {
            if (ModelState.IsValid)
            {
                db.Costs.Add(cost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cost);
        }

        // GET: Admin/Costs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cost cost = db.Costs.Find(id);
            if (cost == null)
            {
                return HttpNotFound();
            }
            return View(cost);
        }

        // POST: Admin/Costs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Item,Unit,UnitPrice,SubTotal,Date,ProjectID,Conpany,Tax,CreateDate,CreateBy,Status,Note,Quantity,VAT,Total")] Cost cost)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cost);
        }

        // GET: Admin/Costs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cost cost = db.Costs.Find(id);
            if (cost == null)
            {
                return HttpNotFound();
            }
            return View(cost);
        }

        // POST: Admin/Costs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cost cost = db.Costs.Find(id);
            db.Costs.Remove(cost);
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
