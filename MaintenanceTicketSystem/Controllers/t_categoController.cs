using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MaintenanceTicketSystem.Models;

namespace MaintenanceTicketSystem.Controllers
{
    public class t_categoController : Controller
    {
        private TicketSystemEntities db = new TicketSystemEntities();

        // GET: t_catego
        public ActionResult Index()
        {
            return View(db.t_catego.ToList());
        }

        // GET: t_catego/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_catego t_catego = db.t_catego.Find(id);
            if (t_catego == null)
            {
                return HttpNotFound();
            }
            return View(t_catego);
        }

        // GET: t_catego/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: t_catego/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "categoria,descripcion,nota")] t_catego t_catego)
        {
            if (ModelState.IsValid)
            {
                db.t_catego.Add(t_catego);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(t_catego);
        }

        // GET: t_catego/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_catego t_catego = db.t_catego.Find(id);
            if (t_catego == null)
            {
                return HttpNotFound();
            }
            return View(t_catego);
        }

        // POST: t_catego/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "categoria,descripcion,nota")] t_catego t_catego)
        {
            if (ModelState.IsValid)
            {
                db.Entry(t_catego).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(t_catego);
        }

        // GET: t_catego/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_catego t_catego = db.t_catego.Find(id);
            if (t_catego == null)
            {
                return HttpNotFound();
            }
            return View(t_catego);
        }

        // POST: t_catego/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            t_catego t_catego = db.t_catego.Find(id);
            db.t_catego.Remove(t_catego);
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
