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
    
    public partial class t_usuarios
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public t_usuarios()
        {
            this.t_tickets = new HashSet<t_tickets>();
            this.t_tickets11 = new HashSet<t_tickets>();
        }
    
        public string usuario { get; set; }
        public Nullable<int> no_emp { get; set; }
        public string planta { get; set; }
        public string nombre { get; set; }
        public string email { get; set; }
        public string turno { get; set; }
        public string puesto { get; set; }
        public string supervisor { get; set; }
        public string depto { get; set; }
        public string rol { get; set; }
        public string u_id { get; set; }
        public Nullable<System.DateTime> f_id { get; set; }
        public string categoria { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<t_tickets> t_tickets { get; set; }
        public virtual t_catego t_catego { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<t_tickets> t_tickets11 { get; set; }
    }
}