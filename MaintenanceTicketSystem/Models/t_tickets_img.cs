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
    
    public partial class t_tickets_img
    {
        public decimal folio { get; set; }
        public decimal consec { get; set; }
        public byte[] imagen { get; set; }
        public string nota { get; set; }
    
        public virtual t_tickets t_tickets { get; set; }
    }
}
