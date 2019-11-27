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
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    public partial class t_equipos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public t_equipos()
        {
            this.t_tickets = new HashSet<t_tickets>();
        }

        public string categoria { get; set; }
        [MaxLength(10, ErrorMessage = "M�ximo 10 caracteres")]
        [Required(ErrorMessage = "Campo obligatorio")]
        [Remote("existe", "t_equipos",
                ErrorMessage = "Esta clave ya existe")]
        public string equipo { get; set; }
        public string descripcion { get; set; }

        public virtual t_catego t_catego { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<t_tickets> t_tickets { get; set; }
    }
}
