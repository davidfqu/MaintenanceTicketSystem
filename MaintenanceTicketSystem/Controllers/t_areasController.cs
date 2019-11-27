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
    public class t_areasController : Controller
    {
        private TicketSystemEntities db = new TicketSystemEntities();

        // GET: t_areas
        public ActionResult Index()
        {
            if (Session["UserRol"].ToString() != "Admin")
                return View();
            return View(db.t_areas.ToList());
        }

        // GET: t_areas/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_areas t_areas = db.t_areas.Find(id);
            if (t_areas == null)
            {
                return HttpNotFound();
            }
            return View(t_areas);
        }

        // GET: t_areas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: t_areas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "area,descripcion")] t_areas t_areas)
        {
            if (ModelState.IsValid)
            {
                t_areas.area = t_areas.area.ToUpper();
                db.t_areas.Add(t_areas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(t_areas);
        }

        // GET: t_areas/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_areas t_areas = db.t_areas.Find(id);
            if (t_areas == null)
            {
                return HttpNotFound();
            }
            return View(t_areas);
        }

        // POST: t_areas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "area,descripcion")] t_areas t_areas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(t_areas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(t_areas);
        }

        // GET: t_areas/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_areas t_areas = db.t_areas.Find(id);
            if (t_areas == null)
            {
                return HttpNotFound();
            }
            return View(t_areas);
        }

        // POST: t_areas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            t_areas t_areas = db.t_areas.Find(id);
            db.t_areas.Remove(t_areas);
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

        public JsonResult existe(string area)
        {
            area = area.ToUpper();
            var validateName = db.t_areas.FirstOrDefault
                                (x => x.area == area);
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
