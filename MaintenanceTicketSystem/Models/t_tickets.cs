//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MaintenanceTicketSystem.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    public partial class t_tickets
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public t_tickets()
        {
            this.t_tickets_img = new HashSet<t_tickets_img>();
        }
    
        public decimal folio { get; set; }
        public string planta { get; set; }
        public Nullable<System.DateTime> fecha { get; set; }
        public Nullable<System.DateTime> f_requerida { get; set; }
        public string u_id { get; set; }
        public Nullable<System.DateTime> f_id { get; set; }
        public string area { get; set; }
        public string equipo { get; set; }
        public string falla { get; set; }
        public string categoria { get; set; }
        public string estatus { get; set; }
        public Nullable<System.DateTime> f_revisado { get; set; }
        public string n_revisado { get; set; }
        public Nullable<System.DateTime> f_aprovacion { get; set; }
        public string n_aprovacion { get; set; }
        public Nullable<System.DateTime> f_proceso { get; set; }
        public string n_proceso { get; set; }
        public Nullable<System.DateTime> f_espera { get; set; }
        public string n_espera { get; set; }
        public Nullable<System.DateTime> f_detenido { get; set; }
        public string n_detenido { get; set; }
        public Nullable<System.DateTime> f_cerrado { get; set; }
        public string n_cerrado { get; set; }
        public string tecnico { get; set; }
        public string turno { get; set; }
        public string prioridad { get; set; }
        public string urgencia { get; set; }
        public string depto { get; set; }
        public string descripcion { get; set; }
        public string actividades { get; set; }
        public Nullable<decimal> duracion { get; set; }
        public Nullable<System.DateTime> f_compromiso { get; set; }
        public string req_autoriza { get; set; }
        public string sup_autoriza { get; set; }
        public Nullable<System.DateTime> sup_fautoriza { get; set; }
        public string req_autoriza2 { get; set; }
        public string sup_autoriza2 { get; set; }
        public Nullable<System.DateTime> sup_fautoriza2 { get; set; }
        public string ind_entrega { get; set; }
        public Nullable<System.DateTime> f_entrega { get; set; }
        public string recibe { get; set; }
        public string sup_revisado { get; set; }
        public string ind_cancelado { get; set; }
        public Nullable<System.DateTime> f_cancela { get; set; }
        public string nota_cancel { get; set; }
        public string req_autoriza3 { get; set; }
        public string sup_autoriza3 { get; set; }
        public Nullable<System.DateTime> sup_fautoriza3 { get; set; }
        public string notas { get; set; }
        public string ind_autoriza { get; set; }
        public string ind_autoriza2 { get; set; }
        public string ind_autoriza3 { get; set; }
        public string nota_autoriza { get; set; }
        public string nota_autoriza2 { get; set; }
        public string nota_autoriza3 { get; set; }
        public string req_autoriza4 { get; set; }
        public string sup_autoriza4 { get; set; }
        public Nullable<System.DateTime> sup_fautoriza4 { get; set; }
        public string nota_autoriza4 { get; set; }
        public string ind_autoriza4 { get; set; }
        
        public string imagen_path { get; set; }
/*
        [DataType(DataType.Upload)]
        [Display(Name = "Upload File")]
        [Required(ErrorMessage = "Please choose file to upload.")]
        public HttpPostedFileBase ImageFile { get; set; }
  */
        public virtual t_areas t_areas { get; set; }
        public virtual t_catego t_catego { get; set; }
        public virtual t_equipos t_equipos { get; set; }
        public virtual t_usuarios t_usuarios { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<t_tickets_img> t_tickets_img { get; set; }
        public virtual t_fallas t_fallas { get; set; }
        public virtual t_usuarios t_usuarios11 { get; set; }
        public virtual t_estatus t_estatus { get; set; }
    }
}
