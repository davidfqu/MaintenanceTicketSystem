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
    public class t_configController : Controller
    {
        private TicketSystemEntities db = new TicketSystemEntities();

      

        // GET: t_config/Edit/5
        public ActionResult Edit(string id)
        {
            try
            {
                if (Session["UserRol"].ToString() != "Admin")
                    return RedirectToAction("Index", "Home");
                //    string username = User.Identity.Name.ToString().Substring(11).ToLower();

                //  var ddlUsuarios = db.t_usuarios.Where(x => x.usuario == username).ToList();

                //if (ddlUsuarios.Any())
                //{
                //  if (ddlUsuarios[0].rol.ToString() == "Admin")
                // {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                t_config t_config = db.t_config.Find(id);
                if (t_config == null)
                {
                    return HttpNotFound();
                }
                return View(t_config);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

            
        }

        // POST: t_config/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "clave,planta,env_correos,server,send_email,send_paswd,port,ind_html,ind_ssl,autoriza_superv,autoriza_seh,dias_revision,aler_gerente,aler_super,aler_compromiso,aler_revision,aler_validaseh,gte_email,gte_email2,gte_email3,path_imgs,ind_testmail,testmail,editar_fechaseg, gte_email4")] t_config t_config)
        {
            if (ModelState.IsValid)
            {
                db.Entry(t_config).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
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
