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
    public class t_fallasController : Controller
    {
        private TicketSystemEntities db = new TicketSystemEntities();

        // GET: t_fallas
        public ActionResult Index()
        {
            if (Session["UserRol"].ToString() != "Admin")
                return View();

            return View(db.t_fallas.ToList());
        }

        // GET: t_fallas/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_fallas t_fallas = db.t_fallas.Find(id);
            if (t_fallas == null)
            {
                return HttpNotFound();
            }
            return View(t_fallas);
        }

        // GET: t_fallas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: t_fallas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "falla,descripcion")] t_fallas t_fallas)
        {
            if (ModelState.IsValid)
            {
                db.t_fallas.Add(t_fallas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(t_fallas);
        }

        // GET: t_fallas/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_fallas t_fallas = db.t_fallas.Find(id);
            if (t_fallas == null)
            {
                return HttpNotFound();
            }
            return View(t_fallas);
        }

        // POST: t_fallas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "falla,descripcion")] t_fallas t_fallas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(t_fallas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(t_fallas);
        }

        // GET: t_fallas/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_fallas t_fallas = db.t_fallas.Find(id);
            if (t_fallas == null)
            {
                return HttpNotFound();
            }
            return View(t_fallas);
        }

        // POST: t_fallas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            t_fallas t_fallas = db.t_fallas.Find(id);
            db.t_fallas.Remove(t_fallas);
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

        public JsonResult existe(string falla)
        {
            var validateName = db.t_fallas.FirstOrDefault
                                (x => x.falla == falla);
            if (validateName != null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
