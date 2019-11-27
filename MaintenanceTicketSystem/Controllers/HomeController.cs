using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Net;
using MaintenanceTicketSystem.Models;
using WebMatrix.Data;
using PagedList;
using System.Globalization;

namespace MaintenanceTicketSystem.Controllers
{
    public class HomeController : Controller
    {
        private TicketSystemEntities db = new TicketSystemEntities();
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, string searchOption, int? page, string accion = "", decimal numticket = 0)
        {
            t_config t_config = db.t_config.Find("01");
            
            string username = User.Identity.Name.ToString().Substring(11).ToLower();
            //string username = "cangulo";

            if(username == "mxc01")
            {
                Session["UserRol"] = "Usuario";
                Session["UserAccount"] = "mxc01";
                return RedirectToAction("IndexTecnico", "Home");
            }


            var ddlUsuarios = db.t_usuarios.Where(x => x.usuario == username).ToList();

            if (ddlUsuarios.Any())
            {
                ViewBag.IsUser = true;
                if(ddlUsuarios[0].email.ToString() == t_config.gte_email.ToString())
                    Session["IsManager"] = true;
                else
                    Session["IsManager"] = false;

                Session["UserAccount"] = ddlUsuarios[0].usuario.ToString();
                Session["UserName"] = ddlUsuarios[0].nombre.ToString();
                Session["UserRol"] = ddlUsuarios[0].rol.ToString();
                Session["UserEmail"] = ddlUsuarios[0].email.ToString();
                Session["Category"] = "user";
                Session["CategoryDesc"] = "user";
                var nombreusuario = ddlUsuarios[0].nombre.ToString().Split(' ');
                Session["UserFirstName"] = nombreusuario[0];

                var t_tickets = db.t_tickets.Include(t => t.t_areas).Include(t => t.t_catego).Include(t => t.t_equipos).Include(t => t.t_usuarios).Include(t => t.t_estatus);
                var t_tickets2 = db.t_tickets.Include(t => t.t_areas).Include(t => t.t_catego).Include(t => t.t_equipos).Include(t => t.t_usuarios).Include(t => t.t_estatus);

                if (ddlUsuarios[0].rol.ToString() == "Usuario")
                 
                    {
                        t_tickets = t_tickets.Where(x => x.u_id == username).OrderBy(x => x.fecha).OrderBy(x => x.t_estatus.orden);
                    }
                    
                if (ddlUsuarios[0].rol.ToString() == "Supervisor")
                {
                    string categoriaUsuario = ddlUsuarios[0].t_catego.descripcion.ToString();
                    Session["Category"] = ddlUsuarios[0].categoria.ToString();
                    Session["CategoryDesc"] = ddlUsuarios[0].t_catego.descripcion.ToString();
                    t_tickets = t_tickets.Where(x => x.t_catego.descripcion == categoriaUsuario);
                    t_tickets = t_tickets.Concat(t_tickets2.Where(x => x.u_id == username));
                    t_tickets = t_tickets.Distinct().OrderByDescending(x => x.urgencia).OrderBy(x => x.t_estatus.orden);
                }
                if (ddlUsuarios[0].rol.ToString() == "Admin")
                {
                 //   t_tickets = t_tickets.OrderBy(x => x.t_estatus.orden).OrderBy(x => x.urgencia).OrderBy(x => x.prioridad).OrderBy(x => x.fecha);
                    t_tickets = t_tickets.OrderBy(x => x.fecha).OrderByDescending(x => x.urgencia).OrderBy(x => x.t_estatus.orden);
                }
                
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
                    if (searchOption == "Folio")
                    {
                        
                        t_tickets = t_tickets.Where(x => x.folio.ToString() == searchString);
                    }
                    

                    if (searchOption == "Usuario")
                    {
                        
                        t_tickets = t_tickets.Where(x => x.u_id.Contains(searchString));
                    }
                       

                    if (searchOption == "Descripción")
                    {
                        
                        t_tickets = t_tickets.Where(x => x.descripcion.Contains(searchString));
                    }

                    if (searchOption == "Técnico asignado")
                    {

                        t_tickets = t_tickets.Where(x => x.tecnico.Contains(searchString));
                    }

                    if (searchOption == "Estatus")
                    {

                        t_tickets = t_tickets.Where(x => x.t_estatus.descripcion.Contains(searchString));
                    }
                }

                ViewBag.CurrentSort = sortOrder;
              
                ViewBag.FolioSortParm = sortOrder == "Folio" ? "Folio_desc" : "Folio";
                ViewBag.CategoriaSortParm = sortOrder == "Categoría" ? "Categoría_desc" : "Categoría";
                ViewBag.EstatusSortParm = sortOrder == "Estatus" ? "Estatus_desc" : "Estatus";
                ViewBag.FCompromisoSortParm = sortOrder == "Fecha Compromiso" ? "Fecha Compromiso_desc" : "Fecha Compromiso";
               


                if (sortOrder != null)
                {
                    switch (sortOrder)
                    {
                        case "Folio":
                            t_tickets = t_tickets.OrderBy(x => x.folio);
                            break;
                        case "Categoría":
                            t_tickets = t_tickets.OrderBy(x => x.categoria);
                            break;
                        case "Categoría_desc":
                            t_tickets = t_tickets.OrderByDescending(x => x.categoria);
                            break;
                        case "Estatus":
                            t_tickets = t_tickets.OrderBy(x => x.t_estatus.orden);
                            break;
                        case "Estatus_desc":
                            t_tickets = t_tickets.OrderByDescending(x => x.t_estatus.orden);
                            break;
                        case "Fecha Compromiso":
                            t_tickets = t_tickets.OrderBy(x => x.f_compromiso);
                            break;
                        case "Fecha Compromiso_desc":
                            t_tickets = t_tickets.OrderByDescending(x => x.f_compromiso);
                            break;

                        case "Folio_desc":
                            t_tickets = t_tickets.OrderByDescending(x => x.folio);
                            break;
                        default:
                            break;
                    }
                }

                if (accion == "crear" && numticket != 0)
                {
                    ViewBag.ticketcreado = true;
                    ViewBag.numticket = numticket;
                }
                if (accion == "editar" && numticket != 0)
                {
                    ViewBag.ticketeditado = true;
                    ViewBag.numticket = numticket;
                }

                int tpendientesauto=  TicketsPendientesAutorizar(username);

                ViewBag.tpendientes = tpendientesauto.ToString();

                int pageSize = 20;
                int pageNumber = (page ?? 1);
                return View(t_tickets.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                ViewBag.IsUser = false;
                return View();
            }
        }

        public ActionResult Autorizar(string sortOrder, string currentFilter, string searchString, string searchOption, int? page)
        {
            string username = Session["UserAccount"].ToString();
            
                var t_tickets = db.t_tickets.Include(t => t.t_areas).Include(t => t.t_catego).Include(t => t.t_equipos).Include(t => t.t_usuarios).Include(t => t.t_estatus);
                var t_tickets2 = db.t_tickets.Include(t => t.t_areas).Include(t => t.t_catego).Include(t => t.t_equipos).Include(t => t.t_usuarios).Include(t => t.t_estatus);
            
            t_tickets = t_tickets.Where(x => x.sup_autoriza == username).Where(x => x.ind_autoriza == null);
            t_tickets = t_tickets.Concat(t_tickets2.Where(x => x.sup_autoriza2 == username).Where(x => x.ind_autoriza2 == null));
            t_tickets = t_tickets.Concat(t_tickets2.Where(x => x.sup_autoriza3 == username).Where(x => x.ind_autoriza3 == null));
            t_tickets = t_tickets.Concat(t_tickets2.Where(x => x.sup_autoriza4 == username).Where(x => x.ind_autoriza4 == null));

            t_tickets = t_tickets.Distinct().OrderBy(x => x.fecha);

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
                    if (searchOption == "Folio")
                    {

                        t_tickets = t_tickets.Where(x => x.folio.ToString() == searchString);
                    }


                    if (searchOption == "Usuario")
                    {

                        t_tickets = t_tickets.Where(x => x.u_id.Contains(searchString));
                    }


                    if (searchOption == "Descripción")
                    {

                        t_tickets = t_tickets.Where(x => x.descripcion.Contains(searchString));
                    }

                }

                ViewBag.CurrentSort = sortOrder;

                ViewBag.FolioSortParm = sortOrder == "Folio" ? "Folio_desc" : "Folio";
                ViewBag.CategoriaSortParm = sortOrder == "Categoría" ? "Categoría_desc" : "Categoría";
                ViewBag.EstatusSortParm = sortOrder == "Estatus" ? "Estatus_desc" : "Estatus";
                ViewBag.FCompromisoSortParm = sortOrder == "Fecha Compromiso" ? "Fecha Compromiso_desc" : "Fecha Compromiso";



                if (sortOrder != null)
                {
                    switch (sortOrder)
                    {
                        case "Folio":
                            t_tickets = t_tickets.OrderBy(x => x.folio);
                            break;
                        case "Categoría":
                            t_tickets = t_tickets.OrderBy(x => x.categoria);
                            break;
                        case "Categoría_desc":
                            t_tickets = t_tickets.OrderByDescending(x => x.categoria);
                            break;
                        case "Estatus":
                            t_tickets = t_tickets.OrderBy(x => x.t_estatus.orden);
                            break;
                        case "Estatus_desc":
                            t_tickets = t_tickets.OrderByDescending(x => x.t_estatus.orden);
                            break;
                        case "Fecha Compromiso":
                            t_tickets = t_tickets.OrderBy(x => x.f_compromiso);
                            break;
                        case "Fecha Compromiso_desc":
                            t_tickets = t_tickets.OrderByDescending(x => x.f_compromiso);
                            break;

                        case "Folio_desc":
                            t_tickets = t_tickets.OrderByDescending(x => x.folio);
                            break;
                        default:
                            break;
                    }
                }


                int pageSize = 3;
                int pageNumber = (page ?? 1);
                return View(t_tickets.ToPagedList(pageNumber, pageSize));
            
        }

        public int TicketsPendientesAutorizar (string username)
        {
            int tpendientesa = 0;
   
            var t_tickets = db.t_tickets.Include(t => t.t_areas).Include(t => t.t_catego).Include(t => t.t_equipos).Include(t => t.t_usuarios).Include(t => t.t_estatus);
            var t_tickets2 = db.t_tickets.Include(t => t.t_areas).Include(t => t.t_catego).Include(t => t.t_equipos).Include(t => t.t_usuarios).Include(t => t.t_estatus);

            t_tickets = t_tickets.Where(x => x.sup_autoriza == username).Where(x => x.ind_autoriza == null);
            t_tickets = t_tickets.Concat(t_tickets2.Where(x => x.sup_autoriza2 == username).Where(x => x.ind_autoriza2 == null));
            t_tickets = t_tickets.Concat(t_tickets2.Where(x => x.sup_autoriza3 == username).Where(x => x.ind_autoriza3 == null));
            t_tickets = t_tickets.Concat(t_tickets2.Where(x => x.sup_autoriza4 == username).Where(x => x.ind_autoriza4 == null));

            t_tickets = t_tickets.Distinct();

            tpendientesa= t_tickets.Count();

            return tpendientesa;
        }
        public ActionResult IndexTecnico()
        {
            return View();
        }

        public ActionResult TicketsTecnico(string NoEmpleado, string sortOrder, string currentFilter, string searchString, string searchOption, int? page, string accion = "", decimal numticket = 0)
        {
            string tecnico = "";
            if(NoEmpleado == null)
            {
                if(Session["Tecnico"] != null)
                tecnico = Session["Tecnico"].ToString();
            }
            else
            {
                tecnico = getTecnico(NoEmpleado);
            }
            
                    
            if(tecnico == "")
            {
               return RedirectToAction("IndexTecnico");
            }
            else
            {
                var nombreusuario = tecnico.ToString().Split(' ');
                ViewBag.nombretecnico = nombreusuario[0];

                var t_tickets = db.t_tickets.Include(t => t.t_areas).Include(t => t.t_catego).Include(t => t.t_equipos).Include(t => t.t_usuarios).Include(t => t.t_estatus);
                t_tickets = t_tickets.Where(x => x.tecnico == tecnico).Where(x => x.estatus == "PR" || x.estatus == "ES").OrderBy(x => x.folio);

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
                    if (searchOption == "Folio")
                    {

                        t_tickets = t_tickets.Where(x => x.folio.ToString() == searchString);
                    }


                    if (searchOption == "Usuario")
                    {

                        t_tickets = t_tickets.Where(x => x.u_id.Contains(searchString));
                    }


                    if (searchOption == "Descripción")
                    {

                        t_tickets = t_tickets.Where(x => x.descripcion.Contains(searchString));
                    }

                    if (searchOption == "Técnico asignado")
                    {

                        t_tickets = t_tickets.Where(x => x.tecnico.Contains(searchString));
                    }

                    if (searchOption == "Estatus")
                    {

                        t_tickets = t_tickets.Where(x => x.t_estatus.descripcion.Contains(searchString));
                    }
                }

                ViewBag.CurrentSort = sortOrder;

                ViewBag.FolioSortParm = sortOrder == "Folio" ? "Folio_desc" : "Folio";
                ViewBag.CategoriaSortParm = sortOrder == "Categoría" ? "Categoría_desc" : "Categoría";
                ViewBag.EstatusSortParm = sortOrder == "Estatus" ? "Estatus_desc" : "Estatus";
                ViewBag.FCompromisoSortParm = sortOrder == "Fecha Compromiso" ? "Fecha Compromiso_desc" : "Fecha Compromiso";



                if (sortOrder != null)
                {
                    switch (sortOrder)
                    {
                        case "Folio":
                            t_tickets = t_tickets.OrderBy(x => x.folio);
                            break;
                        case "Categoría":
                            t_tickets = t_tickets.OrderBy(x => x.categoria);
                            break;
                        case "Categoría_desc":
                            t_tickets = t_tickets.OrderByDescending(x => x.categoria);
                            break;
                        case "Estatus":
                            t_tickets = t_tickets.OrderBy(x => x.t_estatus.orden);
                            break;
                        case "Estatus_desc":
                            t_tickets = t_tickets.OrderByDescending(x => x.t_estatus.orden);
                            break;
                        case "Fecha Compromiso":
                            t_tickets = t_tickets.OrderBy(x => x.f_compromiso);
                            break;
                        case "Fecha Compromiso_desc":
                            t_tickets = t_tickets.OrderByDescending(x => x.f_compromiso);
                            break;

                        case "Folio_desc":
                            t_tickets = t_tickets.OrderByDescending(x => x.folio);
                            break;
                        default:
                            break;
                    }
                }

                if (accion == "crear" && numticket != 0)
                {
                    ViewBag.ticketcreado = true;
                    ViewBag.numticket = numticket;
                }
                if (accion == "editar" && numticket != 0)
                {
                    ViewBag.ticketeditado = true;
                    ViewBag.numticket = numticket;
                }


                int pageSize = 20;
                int pageNumber = (page ?? 1);
                return View(t_tickets.ToPagedList(pageNumber, pageSize));
            }
           

        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

 
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult LogOff()
        {
            return RedirectToAction("Contact");
        }
        public string getTecnico( string numtecnico)
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
   WHERE CB_CODIGO = '"+ numtecnico +@"' AND CB_ACTIVO = 'S' AND CB_NIVEL2= 'MANT' AND CB_CLASIFI = 'H' AND (PO.PU_DESCRIP LIKE 'Plant technician%' 
   or  PO.PU_DESCRIP LIKE 'Sealer Lab Technician%')
   ORDER BY cb_nombres";

            var datos = db.Query(selectedQueryString);

            string tecnico = "";

            Session["Tecnico"] = "";

            if (datos.Any())
            {
                tecnico = cultInfo.ToTitleCase(datos.ElementAt(0).Nombre.ToLower());
                Session["Tecnico"] = cultInfo.ToTitleCase(datos.ElementAt(0).Nombre.ToLower());
            }

            
            return tecnico;
        }

    }
}