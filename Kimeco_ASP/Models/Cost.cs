//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kimeco_ASP.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Cost
    {
        public int ID { get; set; }
        public string Item { get; set; }
        public string Unit { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<decimal> SubTotal { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<int> ConpanyID { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> CreateBy { get; set; }
        public Nullable<bool> Status { get; set; }
        public string Note { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<decimal> VAT { get; set; }
        public Nullable<decimal> Total { get; set; }
    
        public virtual Company Company { get; set; }
    }
}
