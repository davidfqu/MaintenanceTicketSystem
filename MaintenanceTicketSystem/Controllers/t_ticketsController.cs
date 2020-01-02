using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MaintenanceTicketSystem.Models;
using System.Globalization;
using System.IO;

namespace MaintenanceTicketSystem.Controllers
{
    public class t_ticketsController : Controller
    {
        private TicketSystemEntities db = new TicketSystemEntities();

        // GET: t_tickets
        public ActionResult Index()
        {
            var t_tickets = db.t_tickets.Include(t => t.t_areas).Include(t => t.t_catego).Include(t => t.t_equipos).Include(t => t.t_usuarios).Include(t => t.t_fallas).Include(t => t.t_usuarios1).Include(t => t.t_estatus).Include(t => t.t_depto);
            return View(t_tickets.ToList());
        }

        public ActionResult Dashboard()
        {
            if(!(Session["UserRol"].ToString() == "Admin" || Session["UserRol"].ToString() == "Supervisor"))
            {
                return RedirectToAction("Index", "Home", null);
            }
                var t_tickets = db.t_tickets.Include(t => t.t_areas).Include(t => t.t_catego).Include(t => t.t_equipos).Include(t => t.t_usuarios).Include(t => t.t_fallas).Include(t => t.t_usuarios1).Include(t => t.t_estatus).Include(t => t.t_depto);

            //Graficas pie de categorias

            var cat = t_tickets.Where(x => x.depto == "MAN").GroupBy(x => x.categoria).Select(g => new { cat = g.Key, cerrados = g.Sum(i => i.estatus == "CE" ? 1 : 0), abiertos = g.Sum(i => i.estatus != "CE" ? 1 : 0) }).ToList();
            try
            {
                ViewBag.calcerrados = cat.ElementAt(0).cerrados.ToString();
                ViewBag.calabiertos = cat.ElementAt(0).abiertos.ToString();

                ViewBag.faccerrados = cat.ElementAt(1).cerrados.ToString();
                ViewBag.facabiertos = cat.ElementAt(1).abiertos.ToString();

                ViewBag.sealcerrados = cat.ElementAt(2).cerrados.ToString();
                ViewBag.sealabiertos = cat.ElementAt(2).abiertos.ToString();
            }
            catch
            {
                ViewBag.calcerrados = "0";
                ViewBag.calabiertos = "0";

                ViewBag.faccerrados = "0";
                ViewBag.facabiertos = "0";

                ViewBag.sealcerrados = "0";
                ViewBag.sealabiertos = "0";
            }
            

            //grafica de tickets abiertos

            var ticketsabiertos = db.t_tickets.Where(x => x.estatus != "CE" && x.depto == "MAN").OrderBy(x => x.fecha).ToList();
            int n = 0;
            string dataTA = "[";
            string labelsTA = "[";
            string bgcolorTA = "[";
            string bocolorTA = "[";
            string cfac = "'rgba(255, 179, 64, 1)'";
            string cseal = "'rgba(0, 77, 198, 1)'";
            string ccal = "'rgba(224, 0, 0, 1)'";
            string bfac = "'rgba(255, 179, 64, 1)'";
            string bseal = "'rgba(0, 77, 198, 1)'";
            string bcal = "'rgba(224, 0, 0, 1)'";

            DateTime hoy = DateTime.Now;
            DateTime fechaticket;
            double dif;

            foreach (var item in ticketsabiertos)
            {
                fechaticket = (DateTime)item.fecha;
                dif = Math.Round((hoy - fechaticket).TotalDays);
                if (item.categoria == "FAC")
                {
                    bgcolorTA = bgcolorTA + cfac;
                    bocolorTA = bocolorTA + bfac;
                }
                    

                if (item.categoria == "SL")
                {
                    bgcolorTA = bgcolorTA + cseal;
                    bocolorTA = bocolorTA + bseal;
                }
                    

                if (item.categoria == "CAL")
                {
                    bgcolorTA = bgcolorTA + ccal;
                    bocolorTA = bocolorTA + bcal;
                }

                dataTA = dataTA + dif.ToString();
                labelsTA = labelsTA + item.folio.ToString();
                
                n++;
                if (n != 5)
                {
                    dataTA = dataTA + ",";
                    labelsTA = labelsTA + ",";
                    bgcolorTA = bgcolorTA + ",";
                    bocolorTA = bocolorTA + ",";
                }
                else
                {
                    dataTA = dataTA + "]";
                    labelsTA = labelsTA + "]";
                    bgcolorTA = bgcolorTA + "]";
                    bocolorTA = bocolorTA + "]";
                    break;
                }
                   
            }

            var ticketsporatender = db.t_tickets.Where(x => x.estatus != "CE" &&  x.f_compromiso != null && x.depto == "MAN").OrderBy(x => x.f_compromiso).Take(10).ToList();

            var ticketscerrados = db.t_tickets.Where(x => x.estatus == "CE" && x.fecha != null && x.f_cerrado != null && x.depto == "MAN").ToList();
            TimeSpan diff;
            TimeSpan diffp;
            DateTime date1, date2;
            double promediohrs, totalhrs =0;
            int no = 0;

            foreach (var item in ticketscerrados)
            {
                
                    date1 = (DateTime)item.fecha;
                    date2 = (DateTime)item.f_cerrado;

                    diff = date2.Subtract(date1);
                    totalhrs = totalhrs + diff.TotalHours;
                   no++;
                
            }

            promediohrs = Math.Round(totalhrs / no);

            ViewBag.ciclovida = promediohrs.ToString();
            ViewBag.ticketsPA = ticketsporatender;
            ViewBag.datagraph = dataTA;
            ViewBag.labelsgraph = labelsTA;
            ViewBag.bgcolorgraph = bgcolorTA;
            ViewBag.bocolorgraph = bocolorTA;



            return View();
        }

        public ActionResult getEquipo(string id)
        {
            var ddlEquipos = db.t_equipos.Where(x => x.categoria == id).OrderBy(x => x.descripcion).ToList();
            List<SelectListItem> liEquipos = new List<SelectListItem>();

            if (ddlEquipos != null)
            {
                foreach (var x in ddlEquipos)
                {
                    liEquipos.Add(new SelectListItem { Text = x.descripcion, Value = x.equipo });
                }
            }

            return Json(new SelectList(liEquipos, "Value", "Text", JsonRequestBehavior.AllowGet));
    
        }

        public ActionResult getCategorias(string id)
        {
            var ddlCategorias = db.t_catego.Where(x => x.depto == id).OrderBy(x => x.descripcion).ToList();
            List<SelectListItem> liCategorias = new List<SelectListItem>();

            if (ddlCategorias != null)
            {
                foreach (var x in ddlCategorias)
                {
                    liCategorias.Add(new SelectListItem { Text = x.descripcion, Value = x.categoria });
                }
            }

            return Json(new SelectList(liCategorias, "Value", "Text", JsonRequestBehavior.AllowGet));

        }

        public ActionResult getInfoUsuario(string id)
        {
            var ddlUsuarios = db.t_usuarios.Where(x => x.usuario == id).ToList();
            

            string[] infoUsuario = new string[7];
            infoUsuario[0] = ddlUsuarios[0].usuario.ToString();
            infoUsuario[1] = ddlUsuarios[0].depto_tress.ToString();
            infoUsuario[2] = ddlUsuarios[0].puesto.ToString();
            infoUsuario[3] = ddlUsuarios[0].turno.ToString();
            infoUsuario[4] = ddlUsuarios[0].email.ToString();
            infoUsuario[5] = ddlUsuarios[0].rol.ToString();
            infoUsuario[6] = ddlUsuarios[0].nombre.ToString();

            return Json(infoUsuario, JsonRequestBehavior.AllowGet);
        }

        // GET: t_tickets/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_tickets t_tickets = db.t_tickets.Find(id);
            if (t_tickets == null)
            {
                return HttpNotFound();
            }
            return View(t_tickets);
        }



        // GET: t_tickets/Create
        public ActionResult Create()
        {

            List<SelectListItem> urgencia = new List<SelectListItem>();
            urgencia.Add(new SelectListItem() { Text = "", Value = "0" });
            urgencia.Add(new SelectListItem() { Text = "Afecta Producción", Value = "1" });
            urgencia.Add(new SelectListItem() { Text = "Afecta Seguridad", Value = "2" });


            ViewBag.area = new SelectList(db.t_areas.OrderBy(x => x.descripcion), "area", "descripcion");
            ViewBag.depto = new SelectList(db.t_depto.OrderBy(x => x.descripcion), "depto", "descripcion");
            ViewBag.categoria = new SelectList(db.t_catego, "categoria", "descripcion");
            ViewBag.categoria2 = new SelectList(db.t_catego.Where(x => x.depto == "MAN"), "descripcion", "nota");
            ViewBag.categoria3 = new SelectList(db.t_catego.Where(x => x.depto == "SIS"), "descripcion", "nota");
            ViewBag.equipo = new SelectList(db.t_equipos.OrderBy(x => x.descripcion), "equipo", "descripcion");
            ViewBag.u_id = new SelectList(db.t_usuarios, "usuario", "nombre");
            ViewBag.tecnico = new SelectList(db.t_usuarios.Where(x => x.rol == "4") , "usuario", "nombre");
            ViewBag.falla = new SelectList(db.t_fallas.OrderBy(x => x.descripcion), "falla", "descripcion");
            ViewBag.urgencia = new SelectList(urgencia, "Value", "Text");

            return View();
        }

        // POST: t_tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "folio,planta,fecha,f_requerida,u_id,f_id,area,equipo,falla,categoria,estatus,f_revisado,n_revisado,f_aprovacion,n_aprovacion,f_proceso,n_proceso,f_espera,n_espera,f_detenido,n_detenido,f_cerrado,n_cerrado,tecnico,turno,prioridad,urgencia,depto,descripcion,actividades,duracion,f_compromiso,req_autoriza,sup_autoriza,sup_fautoriza,req_autoriza2,sup_autoriza2,sup_fautoriza2,ind_entrega,f_entrega,recibe,sup_revisado,ind_cancelado,f_cancela,nota_cancel,req_autoriza3,sup_autoriza3,sup_fautoriza3,notas,ind_autoriza,ind_autoriza2,ind_autoriza3,nota_autoriza,nota_autoriza2,nota_autoriza3,req_autoriza4,sup_autoriza4,sup_fautoriza4,nota_autoriza4,ind_autoriza4,imagen_path,f_terminado,n_terminado,req_autoriza5,sup_autoriza5,sup_fautoriza5,nota_autoriza5,ind_autoriza5")] t_tickets t_tickets, HttpPostedFileBase ImageFile)
        {

            try
            {
                var queryultimofolio = db.t_tickets.OrderByDescending(x => x.folio).First<t_tickets>().folio.ToString();
                t_tickets.folio = int.Parse(queryultimofolio) + 1;
            }
            catch (Exception e)
            {
                t_tickets.folio = 1;
            }

            if (t_tickets.urgencia == null)
                t_tickets.urgencia = "0";

            t_tickets.fecha = System.DateTime.Now;
            t_tickets.estatus = "RE";
            t_tickets.f_revisado = System.DateTime.Now;

            if (ModelState.IsValid)
            {
                if (ImageFile != null)
                {
                    string path = Path.Combine(Server.MapPath("~/FotosTickets"), t_tickets.folio.ToString() + Path.GetExtension(ImageFile.FileName));
                    ImageFile.SaveAs(path);
                    t_tickets.imagen_path = "~/FotosTickets/" + t_tickets.folio.ToString() + Path.GetExtension(ImageFile.FileName);
                }


                db.t_tickets.Add(t_tickets);
                db.SaveChanges();
                
                return RedirectToAction("Index","Home", new {accion = "crear", numticket = t_tickets.folio });
            }
            
            ViewBag.area = new SelectList(db.t_areas, "area", "descripcion", t_tickets.area);
            ViewBag.categoria = new SelectList(db.t_catego, "categoria", "descripcion", t_tickets.categoria);
            ViewBag.equipo = new SelectList(db.t_equipos, "equipo", "categoria", t_tickets.equipo);
            ViewBag.u_id = new SelectList(db.t_usuarios, "usuario", "planta", t_tickets.u_id);
            ViewBag.tecnico = new SelectList(db.t_usuarios, "usuario", "planta", t_tickets.tecnico);
            return View(t_tickets);
        }

        // GET: t_tickets/Edit/5
        public ActionResult Edit(decimal id, bool? autorizar)
        {
            t_config t_config = db.t_config.Find("01");
           string username = User.Identity.Name.ToString().Substring(11).ToLower();
            // string username = "hgraysom";
            string deptoUsuario = "user";
            var ddlUsuarios = db.t_usuarios.Where(x => x.usuario == username).ToList();

            if (ddlUsuarios.Any())
            {
                ViewBag.IsUser = true;
                if (ddlUsuarios[0].email.ToString() == t_config.gte_email.ToString())
                    Session["IsManager"] = true;
                else
                    Session["IsManager"] = false;
                Session["UserAccount"] = ddlUsuarios[0].usuario.ToString();
                Session["UserName"] = ddlUsuarios[0].nombre.ToString();
                Session["UserRol"] = ddlUsuarios[0].rol.ToString();
                Session["Category"] = "user";
                Session["CategoryDesc"] = "user";
                Session["Depto"] = "user";
                var nombreusuario = ddlUsuarios[0].nombre.ToString().Split(' ');
                Session["UserFirstName"] = nombreusuario[0];

                if (ddlUsuarios[0].rol.ToString() == "Supervisor")
                {
                    if (ddlUsuarios[0].depto.ToString() == "MAN")
                    {
                        string categoriaUsuario = ddlUsuarios[0].t_catego.descripcion.ToString();
                        Session["Depto"] = "MAN";
                        Session["Category"] = ddlUsuarios[0].categoria.ToString();
                        Session["CategoryDesc"] = ddlUsuarios[0].t_catego.descripcion.ToString();

                    }
                    if (ddlUsuarios[0].depto.ToString() == "SIS")
                    {
                        deptoUsuario = ddlUsuarios[0].depto.ToString();
                        Session["Depto"] = "SIS";
                        Session["Category"] = "SIS";
                        Session["CategoryDesc"] = "Sistemas";
                    }
                }
                if (ddlUsuarios[0].rol.ToString() == "Admin")
                {
                    //   t_tickets = t_tickets.OrderBy(x => x.t_estatus.orden).OrderBy(x => x.urgencia).OrderBy(x => x.prioridad).OrderBy(x => x.fecha);
                    deptoUsuario = ddlUsuarios[0].depto.ToString();
                    Session["Depto"] = deptoUsuario;
                }


                ViewBag.autorizar = false;

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                t_tickets t_tickets = db.t_tickets.Find(id);
                if (t_tickets == null)
                {
                    return HttpNotFound();
                }

                List<SelectListItem> prioridad = new List<SelectListItem>();
                prioridad.Add(new SelectListItem() { Text = t_tickets.prioridad, Value = t_tickets.prioridad });

                prioridad.Add(new SelectListItem() { Text = "Baja", Value = "Baja" });
                prioridad.Add(new SelectListItem() { Text = "Media", Value = "Media" });
                prioridad.Add(new SelectListItem() { Text = "Alta", Value = "Alta" });

                if (t_tickets.prioridad != null)
                {
                    for (int i = prioridad.Count() - 1; i >= 0; i--)
                    {
                        if (prioridad[i].Value.ToString() == t_tickets.prioridad)
                        {
                            prioridad.RemoveAt(i);
                            break;
                        }
                    }
                }


                var areaje = new SelectList(db.t_areas, "area", "descripcion", t_tickets.area);


                if (autorizar != null)
                {
                    ViewBag.autorizar = true;
                    if(!((t_tickets.sup_autoriza == username && t_tickets.ind_autoriza== null)|| (t_tickets.sup_autoriza2 == username && t_tickets.ind_autoriza2 == null) || (t_tickets.sup_autoriza3 == username &&  t_tickets.ind_autoriza3 == null) || (t_tickets.sup_autoriza4 == username && t_tickets.ind_autoriza == null) || (t_tickets.sup_autoriza5 == username && t_tickets.ind_autoriza5 == null)))
                        ViewBag.IsUser = false;
                }
                else
                {
                    if (!((t_tickets.u_id == username) || (Session["UserRol"].ToString() == "Supervisor" && t_tickets.categoria == Session["Category"].ToString() && t_tickets.depto == "MAN") || (Session["UserRol"].ToString() == "Admin" && t_tickets.depto == "MAN") || (Session["UserAccount"].ToString() == "mxc01" && t_tickets.depto == "MAN")))
                        ViewBag.IsUser = false;
  
                }

                
                ViewBag.editarFecha = t_config.editar_fechaseg.ToString();

                ViewBag.area = new SelectList(db.t_areas, "area", "descripcion", t_tickets.area);
                ViewBag.categoria = new SelectList(db.t_catego, "categoria", "descripcion", t_tickets.categoria);
                ViewBag.equipo = new SelectList(db.t_equipos, "equipo", "descripcion", t_tickets.equipo);
                ViewBag.u_id = new SelectList(db.t_usuarios, "usuario", "planta", t_tickets.u_id);
                ViewBag.tecnico = new SelectList(getTecnicos("", t_tickets.tecnico), "Value", "Text");
                ViewBag.falla = new SelectList(db.t_fallas, "falla", "descripcion");
                ViewBag.prioridad = new SelectList(prioridad, "Value", "Text");

                return View(t_tickets);
            }
            else
            {
                ViewBag.IsUser = false;
                return View();
            }
        }

        public ActionResult EditSis(decimal id, bool? autorizar)
        {
            t_config t_config = db.t_config.Find("01");
            string username = User.Identity.Name.ToString().Substring(11).ToLower();
           // string username = "hgraysom";
            var ddlUsuarios = db.t_usuarios.Where(x => x.usuario == username).ToList();
            string deptoUsuario = "user";
           
            if (ddlUsuarios.Any())
            {
                ViewBag.IsUser = true;
                if (ddlUsuarios[0].email.ToString() == t_config.gte_email.ToString())
                    Session["IsManager"] = true;
                else
                    Session["IsManager"] = false;
                Session["UserAccount"] = ddlUsuarios[0].usuario.ToString();
                Session["UserName"] = ddlUsuarios[0].nombre.ToString();
                Session["UserRol"] = ddlUsuarios[0].rol.ToString();
                Session["Category"] = "user";
                Session["CategoryDesc"] = "user";
                Session["Depto"] = "user";
                var nombreusuario = ddlUsuarios[0].nombre.ToString().Split(' ');
                Session["UserFirstName"] = nombreusuario[0];

                if (ddlUsuarios[0].rol.ToString() == "Supervisor")
                {
                    if (ddlUsuarios[0].depto.ToString() == "MAN")
                    {
                        string categoriaUsuario = ddlUsuarios[0].t_catego.descripcion.ToString();
                        Session["Depto"] = "MAN";
                        Session["Category"] = ddlUsuarios[0].categoria.ToString();
                        Session["CategoryDesc"] = ddlUsuarios[0].t_catego.descripcion.ToString();

                    }
                    if (ddlUsuarios[0].depto.ToString() == "SIS")
                    {
                       deptoUsuario = ddlUsuarios[0].depto.ToString();
                        Session["Depto"] = "SIS";
                        Session["Category"] = "SIS";
                        Session["CategoryDesc"] = "Sistemas";
                    }
                }

                if (ddlUsuarios[0].rol.ToString() == "Admin")
                {
                    //   t_tickets = t_tickets.OrderBy(x => x.t_estatus.orden).OrderBy(x => x.urgencia).OrderBy(x => x.prioridad).OrderBy(x => x.fecha);
                    deptoUsuario = ddlUsuarios[0].depto.ToString();
                    Session["Depto"] = deptoUsuario;
                }

                ViewBag.autorizar = false;

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                t_tickets t_tickets = db.t_tickets.Find(id);
                if (t_tickets == null)
                {
                    return HttpNotFound();
                }

                List<SelectListItem> prioridad = new List<SelectListItem>();
                prioridad.Add(new SelectListItem() { Text = t_tickets.prioridad, Value = t_tickets.prioridad });

                prioridad.Add(new SelectListItem() { Text = "Baja", Value = "Baja" });
                prioridad.Add(new SelectListItem() { Text = "Media", Value = "Media" });
                prioridad.Add(new SelectListItem() { Text = "Alta", Value = "Alta" });

                if (t_tickets.prioridad != null)
                {
                    for (int i = prioridad.Count() - 1; i >= 0; i--)
                    {
                        if (prioridad[i].Value.ToString() == t_tickets.prioridad)
                        {
                            prioridad.RemoveAt(i);
                            break;
                        }
                    }
                }


                var areaje = new SelectList(db.t_areas, "area", "descripcion", t_tickets.area);


                if (autorizar != null)
                {
                    ViewBag.autorizar = true;
                    if (!((t_tickets.sup_autoriza == username && t_tickets.ind_autoriza == null) || (t_tickets.sup_autoriza2 == username && t_tickets.ind_autoriza2 == null) || (t_tickets.sup_autoriza3 == username && t_tickets.ind_autoriza3 == null) || (t_tickets.sup_autoriza4 == username && t_tickets.ind_autoriza4 == null) || (t_tickets.sup_autoriza5 == username && t_tickets.ind_autoriza5 == null)))
                        ViewBag.IsUser = false;
                }
                else
                {
                    if (!((t_tickets.u_id == username) || (Session["UserRol"].ToString() == "Supervisor"  && t_tickets.depto == "SIS") || (Session["UserRol"].ToString() == "Admin" && t_tickets.depto == "SIS") ))
                        ViewBag.IsUser = false;

                }


                ViewBag.editarFecha = t_config.editar_fechaseg.ToString();

                ViewBag.area = new SelectList(db.t_areas, "area", "descripcion", t_tickets.area);
                ViewBag.categoria = new SelectList(db.t_catego, "categoria", "descripcion", t_tickets.categoria);
                ViewBag.equipo = new SelectList(db.t_equipos, "equipo", "descripcion", t_tickets.equipo);
                ViewBag.u_id = new SelectList(db.t_usuarios, "usuario", "planta", t_tickets.u_id);
                ViewBag.tecnico = new SelectList(getTecnicos("", t_tickets.tecnico), "Value", "Text");
                ViewBag.falla = new SelectList(db.t_fallas, "falla", "descripcion");
                ViewBag.prioridad = new SelectList(prioridad, "Value", "Text");
                ViewBag.sup_revisado = new SelectList(db.t_usuarios.Where(x => (x.rol == "Supervisor" || x.rol == "Admin") && x.depto == "SIS"), "usuario", "nombre");

                return View(t_tickets);
            }
            else
            {
                ViewBag.IsUser = false;
                return View();
            }
        }

        // POST: t_tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "folio,planta,fecha,f_requerida,u_id,f_id,area,equipo,falla,categoria,estatus,f_revisado,n_revisado,f_aprovacion,n_aprovacion,f_proceso,n_proceso,f_espera,n_espera,f_detenido,n_detenido,f_cerrado,n_cerrado,tecnico,turno,prioridad,urgencia,depto,descripcion,actividades,duracion,f_compromiso,req_autoriza,sup_autoriza,sup_fautoriza,req_autoriza2,sup_autoriza2,sup_fautoriza2,ind_entrega,f_entrega,recibe,sup_revisado,ind_cancelado,f_cancela,nota_cancel,req_autoriza3,sup_autoriza3,sup_fautoriza3,notas,ind_autoriza,ind_autoriza2,ind_autoriza3,nota_autoriza,nota_autoriza2,nota_autoriza3,req_autoriza4,sup_autoriza4,sup_fautoriza4,nota_autoriza4,ind_autoriza4,imagen_path,f_terminado,n_terminado,req_autoriza5,sup_autoriza5,sup_fautoriza5,nota_autoriza5,ind_autoriza5")] t_tickets t_tickets,  string command = "0")
        {
            //si viene de una vista para autorizar
            if (command != "0")
            {
                string username = Session["UserAccount"].ToString();

                if (command == "Autorizar")
                {
                    if(t_tickets.sup_autoriza == username)
                    {
                        t_tickets.sup_fautoriza = System.DateTime.Now;
                        t_tickets.ind_autoriza = "1";
                    }
                    if (t_tickets.sup_autoriza2 == username)
                    {
                        t_tickets.sup_fautoriza2 = System.DateTime.Now;
                        t_tickets.ind_autoriza2 = "1";
                    }
                    if (t_tickets.sup_autoriza3 == username)
                    {
                        t_tickets.sup_fautoriza3 = System.DateTime.Now;
                        t_tickets.ind_autoriza3 = "1";
                    }
                    if (t_tickets.sup_autoriza4 == username)
                    {
                        t_tickets.sup_fautoriza4 = System.DateTime.Now;
                        t_tickets.ind_autoriza4 = "1";
                    }
                    if (t_tickets.sup_autoriza5 == username)
                    {
                        t_tickets.sup_fautoriza5 = System.DateTime.Now;
                        t_tickets.ind_autoriza5 = "1";
                    }
                }
                if (command == "Rechazar")
                {
                    if (t_tickets.sup_autoriza == username)
                    {
                        t_tickets.sup_fautoriza = System.DateTime.Now;
                        t_tickets.ind_autoriza = "0";
                    }
                    if (t_tickets.sup_autoriza2 == username)
                    {
                        t_tickets.sup_fautoriza2 = System.DateTime.Now;
                        t_tickets.ind_autoriza2 = "0";
                    }
                    if (t_tickets.sup_autoriza3 == username)
                    {
                        t_tickets.sup_fautoriza3 = System.DateTime.Now;
                        t_tickets.ind_autoriza3 = "0";
                    }
                    if (t_tickets.sup_autoriza4 == username)
                    {
                        t_tickets.sup_fautoriza4 = System.DateTime.Now;
                        t_tickets.ind_autoriza4 = "0";
                    }
                    if (t_tickets.sup_autoriza5 == username)
                    {
                        t_tickets.sup_fautoriza5 = System.DateTime.Now;
                        t_tickets.ind_autoriza5 = "0";
                    }
                }
            }
            
            if (ModelState.IsValid)
            {
                if (Session["UserRol"].ToString() == "Supervisor")
                {
                    t_tickets.sup_revisado = Session["UserAccount"].ToString();
                }

                if (t_tickets.estatus != "CE")
                {
                    t_tickets.falla = null;
                }
                t_tickets.f_id = System.DateTime.Now;
                //t_tickets.u_id = Session["UserAccount"].ToString();

                db.Entry(t_tickets).State = EntityState.Modified;

                db.SaveChanges();
                if (Session["UserAccount"].ToString() == "mxc01")
                {
                    return RedirectToAction("TicketsTecnico", "Home", new {tecnico = t_tickets.tecnico, accion = "editar", numticket = t_tickets.folio });
                }
                else
                    return RedirectToAction("Index", "Home", new { accion = "editar", numticket = t_tickets.folio });
            }
            ViewBag.area = new SelectList(db.t_areas, "area", "descripcion", t_tickets.area);
            ViewBag.categoria = new SelectList(db.t_catego, "categoria", "descripcion", t_tickets.categoria);
            ViewBag.equipo = new SelectList(db.t_equipos, "equipo", "categoria", t_tickets.equipo);
            ViewBag.u_id = new SelectList(db.t_usuarios, "usuario", "planta", t_tickets.u_id);
            ViewBag.tecnico = new SelectList(db.t_usuarios, "usuario", "planta", t_tickets.tecnico);

            if (command != "0")
            return RedirectToAction("Autorizar", "Home");
            else
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSis([Bind(Include = "folio,planta,fecha,f_requerida,u_id,f_id,area,equipo,falla,categoria,estatus,f_revisado,n_revisado,f_aprovacion,n_aprovacion,f_proceso,n_proceso,f_espera,n_espera,f_detenido,n_detenido,f_cerrado,n_cerrado,tecnico,turno,prioridad,urgencia,depto,descripcion,actividades,duracion,f_compromiso,req_autoriza,sup_autoriza,sup_fautoriza,req_autoriza2,sup_autoriza2,sup_fautoriza2,ind_entrega,f_entrega,recibe,sup_revisado,ind_cancelado,f_cancela,nota_cancel,req_autoriza3,sup_autoriza3,sup_fautoriza3,notas,ind_autoriza,ind_autoriza2,ind_autoriza3,nota_autoriza,nota_autoriza2,nota_autoriza3,req_autoriza4,sup_autoriza4,sup_fautoriza4,nota_autoriza4,ind_autoriza4,imagen_path,f_terminado,n_terminado,req_autoriza5,sup_autoriza5,sup_fautoriza5,nota_autoriza5,ind_autoriza5")] t_tickets t_tickets, string command = "0")
        {
            //si viene de una vista para autorizar
            if (command != "0")
            {
                string username = Session["UserAccount"].ToString();

                if (command == "Autorizar")
                {
                    if (t_tickets.sup_autoriza == username)
                    {
                        t_tickets.sup_fautoriza = System.DateTime.Now;
                        t_tickets.ind_autoriza = "1";
                    }
                    if (t_tickets.sup_autoriza2 == username)
                    {
                        t_tickets.sup_fautoriza2 = System.DateTime.Now;
                        t_tickets.ind_autoriza2 = "1";
                    }
                    if (t_tickets.sup_autoriza3 == username)
                    {
                        t_tickets.sup_fautoriza3 = System.DateTime.Now;
                        t_tickets.ind_autoriza3 = "1";
                    }
                    if (t_tickets.sup_autoriza4 == username)
                    {
                        t_tickets.sup_fautoriza4 = System.DateTime.Now;
                        t_tickets.ind_autoriza4 = "1";
                    }
                    if (t_tickets.sup_autoriza5 == username)
                    {
                        t_tickets.sup_fautoriza5 = System.DateTime.Now;
                        t_tickets.ind_autoriza5 = "1";
                    }
                }
                if (command == "Rechazar")
                {
                    if (t_tickets.sup_autoriza == username)
                    {
                        t_tickets.sup_fautoriza = System.DateTime.Now;
                        t_tickets.ind_autoriza = "0";
                    }
                    if (t_tickets.sup_autoriza2 == username)
                    {
                        t_tickets.sup_fautoriza2 = System.DateTime.Now;
                        t_tickets.ind_autoriza2 = "0";
                    }
                    if (t_tickets.sup_autoriza3 == username)
                    {
                        t_tickets.sup_fautoriza3 = System.DateTime.Now;
                        t_tickets.ind_autoriza3 = "0";
                    }
                    if (t_tickets.sup_autoriza4 == username)
                    {
                        t_tickets.sup_fautoriza4 = System.DateTime.Now;
                        t_tickets.ind_autoriza4 = "0";
                    }
                    if (t_tickets.sup_autoriza5 == username)
                    {
                        t_tickets.sup_fautoriza5 = System.DateTime.Now;
                        t_tickets.ind_autoriza5 = "0";
                    }
                }
            }

            if (ModelState.IsValid)
            {
                if (Session["UserRol"].ToString() == "Supervisor")
                {
                    t_tickets.sup_revisado = Session["UserAccount"].ToString();
                }

                if (t_tickets.estatus != "CE")
                {
                    t_tickets.falla = null;
                }
                t_tickets.f_id = System.DateTime.Now;
                //t_tickets.u_id = Session["UserAccount"].ToString();

                db.Entry(t_tickets).State = EntityState.Modified;

                db.SaveChanges();
                if (Session["UserAccount"].ToString() == "mxc01")
                {
                    return RedirectToAction("TicketsTecnico", "Home", new { tecnico = t_tickets.tecnico, accion = "editar", numticket = t_tickets.folio });
                }
                else
                    return RedirectToAction("EditSis", "t_tickets", new { id = t_tickets.folio });
                
            }
            ViewBag.area = new SelectList(db.t_areas, "area", "descripcion", t_tickets.area);
            ViewBag.categoria = new SelectList(db.t_catego, "categoria", "descripcion", t_tickets.categoria);
            ViewBag.equipo = new SelectList(db.t_equipos, "equipo", "categoria", t_tickets.equipo);
            ViewBag.u_id = new SelectList(db.t_usuarios, "usuario", "planta", t_tickets.u_id);
            ViewBag.tecnico = new SelectList(db.t_usuarios, "usuario", "planta", t_tickets.tecnico);

            if (command != "0")
                return RedirectToAction("Autorizar", "Home");
            else
                return RedirectToAction("Index", "Home");
        }

        // GET: t_tickets/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_tickets t_tickets = db.t_tickets.Find(id);
            if (t_tickets == null)
            {
                return HttpNotFound();
            }
            return View(t_tickets);
        }

        // POST: t_tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            t_tickets t_tickets = db.t_tickets.Find(id);
            db.t_tickets.Remove(t_tickets);
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

        public static List<SelectListItem> getTecnicos(string categoria, string tecnico = null)
        {
            TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;
            var db = WebMatrix.Data.Database.Open("TressConn");

            var selectedQueryString = @"SELECT CB_CODIGO, cb_nombres + ' ' +CB_APE_PAT + ' ' +CB_APE_MAT as NOMBRE
   ,CB_TURNO,po.PU_DESCRIP AS PUESTO,
   CB_NIVEL2,n3.TB_ELEMENT AS AREA,CB_NIVEL4 
   from [Tress_MedlineMXL].[dbo].COLABORA co 
   inner join [Tress_MedlineMXL].[dbo].PUESTO po on co.CB_PUESTO = po.PU_CODIGO 
   inner join [Tress_MedlineMXL].[dbo].NIVEL2 n2 on co.CB_NIVEL2 = n2.TB_CODIGO 
   inner join [Tress_MedlineMXL].[dbo].NIVEL3 n3 on co.CB_NIVEL3 = n3.TB_CODIGO 
   inner join [Tress_MedlineMXL].[dbo].NIVEL4 n4 on co.CB_NIVEL4 = n4.TB_CODIGO 
   inner join [Tress_MedlineMXL].[dbo].TURNO tu on co.CB_TURNO = tu.TU_CODIGO 
   WHERE CB_ACTIVO = 'S' AND CB_NIVEL2= 'MANT' AND CB_CLASIFI = 'H' AND (PO.PU_DESCRIP LIKE 'Plant technician%' 
   or  PO.PU_DESCRIP LIKE 'Sealer Lab Technician%' or  PO.PU_DESCRIP LIKE 'Calibration%')
   ORDER BY cb_nombres";

            var datos = db.Query(selectedQueryString);

            List<SelectListItem> tecnicos= new List<SelectListItem>();

            
            tecnicos.Add(new SelectListItem() { Text = tecnico, Value = tecnico });
            
            string[] empleado = new string[6];

            if (datos.Any())
            {
                foreach (var item in datos)
                {
                   tecnicos.Add(new SelectListItem() { Text = cultInfo.ToTitleCase(item.Nombre.ToLower()), Value= cultInfo.ToTitleCase(item.Nombre.ToLower()) });
                }
                
            }

            if(tecnico != null && tecnico != "N/A")
            {
                for (int i = tecnicos.Count() - 1; i >= 0; i--)
                {
                    if (tecnicos[i].Value.ToString() == tecnico)
                    {
                        tecnicos.RemoveAt(i);
                        break;
                    }
                }
            }

            tecnicos.Add(new SelectListItem() { Text = "N/A", Value = "N/A" });

            return tecnicos;
        }
    }
}
