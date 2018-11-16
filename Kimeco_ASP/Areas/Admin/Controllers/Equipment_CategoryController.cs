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
    public class Equipment_CategoryController : Controller
    {
        private KimecoEntities db = new KimecoEntities();

        // GET: Admin/Equipment_Category
        public ActionResult Index()
        {
            return View(db.Equipment_Category.ToList());
        }

        // GET: Admin/Equipment_Category/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment_Category equipment_Category = db.Equipment_Category.Find(id);
            if (equipment_Category == null)
            {
                return HttpNotFound();
            }
            return View(equipment_Category);
        }

        // GET: Admin/Equipment_Category/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Equipment_Category/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,CreateDate,CreateBy,Status,Note")] Equipment_Category equipment_Category)
        {
            if (ModelState.IsValid)
            {
                db.Equipment_Category.Add(equipment_Category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(equipment_Category);
        }

        // GET: Admin/Equipment_Category/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment_Category equipment_Category = db.Equipment_Category.Find(id);
            if (equipment_Category == null)
            {
                return HttpNotFound();
            }
            return View(equipment_Category);
        }

        // POST: Admin/Equipment_Category/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,CreateDate,CreateBy,Status,Note")] Equipment_Category equipment_Category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(equipment_Category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(equipment_Category);
        }

        // GET: Admin/Equipment_Category/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment_Category equipment_Category = db.Equipment_Category.Find(id);
            if (equipment_Category == null)
            {
                return HttpNotFound();
            }
            return View(equipment_Category);
        }

        // POST: Admin/Equipment_Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Equipment_Category equipment_Category = db.Equipment_Category.Find(id);
            db.Equipment_Category.Remove(equipment_Category);
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
