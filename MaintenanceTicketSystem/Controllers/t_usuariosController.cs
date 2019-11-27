using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MaintenanceTicketSystem.Models;
using WebMatrix.Data;
using System.Globalization;
using PagedList;

namespace MaintenanceTicketSystem.Controllers
{
    public class t_usuariosController : Controller
    {
        private TicketSystemEntities db = new TicketSystemEntities();

        // GET: t_usuarios
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, string searchOption, int? page)
        {
            if (Session["UserRol"].ToString() != "Admin")
                return View();

            var t_usuarios = db.t_usuarios.Include(t => t.t_catego);

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
                ViewBag.CurrentSearch = searchOption;
                if (searchOption == "No. de empleado")
                {

                    t_usuarios = t_usuarios.Where(x => x.no_emp.ToString() == searchString);
                }


                if (searchOption == "Nombre")
                {

                    t_usuarios = t_usuarios.Where(x => x.nombre.Contains(searchString));
                }


                if (searchOption == "Rol")
                {

                    t_usuarios = t_usuarios.Where(x => x.rol.Contains(searchString));
                }

            }

            ViewBag.CurrentSort = sortOrder;

            ViewBag.NoEmpleadoSortParm = sortOrder == "No. de empleado" ? "No. de empleado_desc" : "No. de empleado";
            ViewBag.NombreSortParm = sortOrder == "Nombre" ? "Nombre_desc" : "Nombre";
            ViewBag.RolSortParm = sortOrder == "Rol" ? "Rol_desc" : "Rol";



            if (sortOrder != null)
            {
                switch (sortOrder)
                {
                    case "No. de empleado":
                        t_usuarios = t_usuarios.OrderBy(x => x.no_emp);
                        break;
                    case "Nombre":
                        t_usuarios = t_usuarios.OrderBy(x => x.nombre);
                        break;
                    case "Rol":
                        t_usuarios = t_usuarios.OrderBy(x => x.rol);
                        break;
                    case "No. de empleado_desc":
                        t_usuarios = t_usuarios.OrderByDescending(x => x.no_emp);
                        break;
                    case "Nombre_desc":
                        t_usuarios = t_usuarios.OrderByDescending(x => x.nombre);
                        break;
                    case "Rol_desc":
                        t_usuarios = t_usuarios.OrderByDescending(x => x.rol);
                        break;

                    default:
                        break;
                }
            }
            else
            {
                t_usuarios = t_usuarios.OrderBy(x => x.nombre);

            }


            int pageSize = 50;
            int pageNumber = (page ?? 1);
                        
            return View(t_usuarios.ToPagedList(pageNumber, pageSize));
        }

        // GET: t_usuarios/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_usuarios t_usuarios = db.t_usuarios.Find(id);
            if (t_usuarios == null)
            {
                return HttpNotFound();
            }
            return View(t_usuarios);
        }

        // GET: t_usuarios/Create
        public ActionResult Create(int? idEmpleado)
        {
            List<SelectListItem> rol = new List<SelectListItem>();
            rol.Add(new SelectListItem() { Text = "Usuario", Value = "Usuario" });
            rol.Add(new SelectListItem() { Text = "Supervisor", Value = "Supervisor" });
            rol.Add(new SelectListItem() { Text = "Admin", Value = "Admin" });

            ViewBag.rol = new SelectList(rol, "Value", "Text");
            ViewBag.categoria = new SelectList(db.t_catego, "categoria", "descripcion");

            return View();
        }

        public ActionResult datosTress(int id)
        {

            TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

            var db = WebMatrix.Data.Database.Open("TressConn");



            var selectedQueryString = @"SELECT co.cb_nombres + ' ' +co.CB_APE_PAT + ' ' +co.CB_APE_MAT as NOMBRE
                                        ,co.CB_TURNO,po.PU_DESCRIP AS PUESTO,
                                        co.CB_NIVEL2,n3.TB_ELEMENT AS AREA,co2.CB_E_MAIL
                                        from [Tress_MedlineMXL].[dbo].COLABORA co 
                                        inner join [Tress_MedlineMXL].[dbo].PUESTO po on co.CB_PUESTO = po.PU_CODIGO 
                                        inner join [Tress_MedlineMXL].[dbo].NIVEL2 n2 on co.CB_NIVEL2 = n2.TB_CODIGO 
                                        inner join [Tress_MedlineMXL].[dbo].NIVEL3 n3 on co.CB_NIVEL3 = n3.TB_CODIGO 
                                        inner join [Tress_MedlineMXL].[dbo].NIVEL4 n4 on co.CB_NIVEL4 = n4.TB_CODIGO 
                                        inner join [Tress_MedlineMXL].[dbo].TURNO tu on co.CB_TURNO = tu.TU_CODIGO 
										inner join [Tress_MedlineMXL].[dbo].COLABORA co2 on co.CB_NIVEL4 = co2.CB_CODIGO

                                        WHERE co.CB_CODIGO = " + id.ToString() + @" and co.CB_ACTIVO = 'S' 
                                        ORDER BY PU_DESCRIP,CB_TURNO";

            var datos = db.Query(selectedQueryString);


            string[] empleado = new string[6];

            if (datos.Any())
            {
                for (int i = 0; i < 6; i++)
                {
                    empleado[i] = datos.ElementAt(0)[i];
                }
                empleado[0] = cultInfo.ToTitleCase(empleado[0].ToString().ToLower());
            }

            

            return Json(empleado, JsonRequestBehavior.AllowGet); 
        }


        // POST: t_usuarios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "usuario,no_emp,planta,nombre,email,turno,puesto,supervisor,depto,rol,categoria")] t_usuarios t_usuarios)
        {
            if (ModelState.IsValid)
            {
                if( t_usuarios.rol.ToString() != "Supervisor")
                t_usuarios.categoria = null;

                t_usuarios.email = t_usuarios.email.ToLower();
                t_usuarios.supervisor = t_usuarios.supervisor.ToLower();
                t_usuarios.usuario = t_usuarios.usuario.ToLower();
                t_usuarios.f_id = DateTime.Now;
                t_usuarios.u_id = @User.Identity.Name.ToString().Substring(11).ToLower();
                db.t_usuarios.Add(t_usuarios);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(t_usuarios);
        }

        // GET: t_usuarios/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_usuarios t_usuarios = db.t_usuarios.Find(id);
            if (t_usuarios == null)
            {
                return HttpNotFound();
            }

            List<SelectListItem> rol = new List<SelectListItem>();
            rol.Add(new SelectListItem() { Text = t_usuarios.rol.ToString(), Value = t_usuarios.rol.ToString() });

            if(t_usuarios.rol.ToString() == "Usuario")
            {
                rol.Add(new SelectListItem() { Text = "Supervisor", Value = "Supervisor" });
                rol.Add(new SelectListItem() { Text = "Admin", Value = "Admin" });
            }

            if (t_usuarios.rol.ToString() == "Supervisor")
            {
                rol.Add(new SelectListItem() { Text = "Usuario", Value = "Usuario" });
                rol.Add(new SelectListItem() { Text = "Admin", Value = "Admin" });
            }

            if (t_usuarios.rol.ToString() == "Admin")
            {
                rol.Add(new SelectListItem() { Text = "Usuario", Value = "Usuario" });
                rol.Add(new SelectListItem() { Text = "Supervisor", Value = "Supervisor" });
            }



            ViewBag.categoria = new SelectList(db.t_catego, "categoria", "descripcion");

            ViewBag.rol = new SelectList(rol, "Value", "Text");
            return View(t_usuarios);
        }

        // POST: t_usuarios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "usuario,no_emp,planta,nombre,email,turno,puesto,supervisor,depto,rol,u_id,f_id, categoria")] t_usuarios t_usuarios)
        {
            if (ModelState.IsValid)
            {
                t_usuarios.email = t_usuarios.email.ToLower();
                if(t_usuarios.supervisor != null)
                t_usuarios.supervisor = t_usuarios.supervisor.ToLower();

                t_usuarios.usuario = t_usuarios.usuario.ToLower();
                t_usuarios.f_id = DateTime.Now;
                t_usuarios.u_id = @User.Identity.Name.ToString().Substring(11).ToLower();
                db.Entry(t_usuarios).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(t_usuarios);
        }

        public ActionResult EditarManager(string id, string rol)
        {
            t_usuarios t_usuarios = db.t_usuarios.Find(id);

            t_usuarios.rol = rol;
            if(rol == "Supervisor")
            t_usuarios.categoria = "CAL";
            else
            t_usuarios.categoria = null;

            db.Entry(t_usuarios).State = EntityState.Modified;
            db.SaveChanges();
            /*

            if (ModelState.IsValid)
            {
                t_usuarios.email = t_usuarios.email.ToLower();
                if (t_usuarios.supervisor != null)
                    t_usuarios.supervisor = t_usuarios.supervisor.ToLower();

                t_usuarios.usuario = t_usuarios.usuario.ToLower();
                t_usuarios.f_id = DateTime.Now;
                t_usuarios.u_id = @User.Identity.Name.ToString().Substring(11).ToLower();
                db.Entry(t_usuarios).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            */
            return RedirectToAction("Index", "Home");
        }

        // GET: t_usuarios/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_usuarios t_usuarios = db.t_usuarios.Find(id);
            if (t_usuarios == null)
            {
                return HttpNotFound();
            }
            return View(t_usuarios);
        }

        // POST: t_usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            t_usuarios t_usuarios = db.t_usuarios.Find(id);
            db.t_usuarios.Remove(t_usuarios);
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
        public JsonResult existe(string usuario)
        {
            var validateName = db.t_usuarios.FirstOrDefault
                                (x => x.usuario == usuario);
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
