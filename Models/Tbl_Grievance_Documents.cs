//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Grievancemis.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tbl_Grievance_Documents
    {
        public int Id { get; set; }
        public System.Guid GrievanceId { get; set; }
        public Nullable<int> RevertId { get; set; }
        public string DocumentPath { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
    
        public virtual Tbl_Grievance Tbl_Grievance { get; set; }
    }
}
