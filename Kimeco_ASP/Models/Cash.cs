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
    
    public partial class Cash
    {
        public int ID { get; set; }
        public string C_Date { get; set; }
        public string Company { get; set; }
        public string ProjectName { get; set; }
        public string Staff { get; set; }
        public string C_Content { get; set; }
        public Nullable<decimal> Input { get; set; }
        public Nullable<decimal> Output { get; set; }
        public string Invoice { get; set; }
        public string Ref { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> CreateBy { get; set; }
        public Nullable<bool> Status { get; set; }
        public string Note { get; set; }
    }
}
