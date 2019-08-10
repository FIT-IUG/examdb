using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using exam_db.Models;

namespace exam_db.Controllers
{
    public class ConstantsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Constants
        public ActionResult Index()
        {
            return View(db.Constants.ToList());
        }

        // GET: Constants/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Constant constant = db.Constants.Find(id);
            if (constant == null)
            {
                return HttpNotFound();
            }
            return View(constant);
        }

        // GET: Constants/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Constants/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Key,Value")] Constant constant)
        {
            if (ModelState.IsValid)
            {
                db.Constants.Add(constant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(constant);
        }

        // GET: Constants/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Constant constant = db.Constants.Find(id);
            if (constant == null)
            {
                return HttpNotFound();
            }
            return View(constant);
        }

        // POST: Constants/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Key,Value")] Constant constant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(constant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(constant);
        }

        // GET: Constants/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Constant constant = db.Constants.Find(id);
            if (constant == null)
            {
                return HttpNotFound();
            }
            return View(constant);
        }

        // POST: Constants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Constant constant = db.Constants.Find(id);
            db.Constants.Remove(constant);
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
