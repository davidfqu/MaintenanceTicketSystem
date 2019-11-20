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
    public class t_tickets_imgController : Controller
    {
        private TicketSystemEntities db = new TicketSystemEntities();

        // GET: t_tickets_img
        public ActionResult Index()
        {
            var t_tickets_img = db.t_tickets_img.Include(t => t.t_tickets);
            return View(t_tickets_img.ToList());
        }

        // GET: t_tickets_img/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_tickets_img t_tickets_img = db.t_tickets_img.Find(id);
            if (t_tickets_img == null)
            {
                return HttpNotFound();
            }
            return View(t_tickets_img);
        }

        // GET: t_tickets_img/Create
        public ActionResult Create()
        {
            ViewBag.folio = new SelectList(db.t_tickets, "folio", "planta");
            return View();
        }

        // POST: t_tickets_img/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "folio,consec,imagen,nota")] t_tickets_img t_tickets_img)
        {
            if (ModelState.IsValid)
            {
                db.t_tickets_img.Add(t_tickets_img);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.folio = new SelectList(db.t_tickets, "folio", "planta", t_tickets_img.folio);
            return View(t_tickets_img);
        }

        // GET: t_tickets_img/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_tickets_img t_tickets_img = db.t_tickets_img.Find(id);
            if (t_tickets_img == null)
            {
                return HttpNotFound();
            }
            ViewBag.folio = new SelectList(db.t_tickets, "folio", "planta", t_tickets_img.folio);
            return View(t_tickets_img);
        }

        // POST: t_tickets_img/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "folio,consec,imagen,nota")] t_tickets_img t_tickets_img)
        {
            if (ModelState.IsValid)
            {
                db.Entry(t_tickets_img).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.folio = new SelectList(db.t_tickets, "folio", "planta", t_tickets_img.folio);
            return View(t_tickets_img);
        }

        // GET: t_tickets_img/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_tickets_img t_tickets_img = db.t_tickets_img.Find(id);
            if (t_tickets_img == null)
            {
                return HttpNotFound();
            }
            return View(t_tickets_img);
        }

        // POST: t_tickets_img/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            t_tickets_img t_tickets_img = db.t_tickets_img.Find(id);
            db.t_tickets_img.Remove(t_tickets_img);
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
