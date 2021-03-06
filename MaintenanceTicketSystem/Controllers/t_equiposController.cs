﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MaintenanceTicketSystem.Models;
using PagedList;

namespace MaintenanceTicketSystem.Controllers
{
    public class t_equiposController : Controller
    {
        private TicketSystemEntities db = new TicketSystemEntities();

        // GET: t_equipos
        public ActionResult Index(string searchString, string currentFilter, int? page)
        {
            try
            {
                if (Session["UserRol"].ToString() != "Admin")
                    return RedirectToAction("Index", "Home");

                string depto = Session["Depto"].ToString();
               
                ViewBag.SearchString = new SelectList(db.t_catego.Where(x => x.depto == depto), "categoria", "descripcion");
           


                var t_equipos = db.t_equipos.Include(t => t.t_catego);

                t_equipos = t_equipos.Where(x => x.depto == depto).OrderBy(t => t.categoria).OrderBy(x => x.descripcion);

                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }
                ViewBag.CurrentFilter = searchString;
                ViewBag.CurrentSearch = null;

                if (!String.IsNullOrEmpty(searchString))
                {
                    t_equipos = t_equipos.Where(x => x.categoria == searchString).OrderBy(x => x.descripcion);

                }

                int pageSize = 20;
                int pageNumber = (page ?? 1);
                return View(t_equipos.ToPagedList(pageNumber, pageSize));
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
            

           
        }

        // GET: t_equipos/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_equipos t_equipos = db.t_equipos.Find(id);
            if (t_equipos == null)
            {
                return HttpNotFound();
            }
            return View(t_equipos);
        }

        // GET: t_equipos/Create
        public ActionResult Create()
        {
            try
            {
                if (Session["UserRol"].ToString() != "Admin")
                    return RedirectToAction("Index", "Home");

                string depto = Session["Depto"].ToString();
                ViewBag.categoria = new SelectList(db.t_catego.Where(x => x.depto == depto), "categoria", "descripcion");
                return View();
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: t_equipos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "categoria,equipo,descripcion")] t_equipos t_equipos)
        {
            try
            {
                string depto = Session["Depto"].ToString();
                if (ModelState.IsValid)
                {
                    t_equipos.depto = depto;
                    t_equipos.equipo = t_equipos.equipo.ToUpper();
                    db.t_equipos.Add(t_equipos);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.categoria = new SelectList(db.t_catego, "categoria", "descripcion", t_equipos.categoria);
                return View(t_equipos);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
           
        }

        // GET: t_equipos/Edit/5
        public ActionResult Edit(string id, string depto, string categoria)
        {
            try
            {
                string depto2 = Session["Depto"].ToString();

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                t_equipos t_equipos = db.t_equipos.Find(categoria, id, depto);
                if (t_equipos == null)
                {
                    return HttpNotFound();
                }
                ViewBag.categoria = new SelectList(db.t_catego.Where(x => x.depto == depto2), "categoria", "descripcion", t_equipos.categoria);
                return View(t_equipos);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: t_equipos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "categoria,equipo,descripcion")] t_equipos t_equipos)
        {
            try
            {
                string depto = Session["Depto"].ToString();
                t_equipos.depto = depto;
                if (ModelState.IsValid)
                {
                    db.Entry(t_equipos).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.categoria = new SelectList(db.t_catego, "categoria", "descripcion", t_equipos.categoria);
                return View(t_equipos);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: t_equipos/Delete/5
        public ActionResult Delete(string id, string depto, string categoria)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_equipos t_equipos = db.t_equipos.Find(categoria, id, depto);
            if (t_equipos == null)
            {
                return HttpNotFound();
            }
            return View(t_equipos);
        }

        // POST: t_equipos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id, string depto, string categoria)
        {
            t_equipos t_equipos = db.t_equipos.Find(categoria, id, depto);
            db.t_equipos.Remove(t_equipos);
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
        public JsonResult existe(string equipo, string categoria)
        {
            equipo = equipo.ToUpper();
            var validateName = db.t_equipos.FirstOrDefault
                                (x => x.equipo == equipo && x.categoria == categoria);
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
