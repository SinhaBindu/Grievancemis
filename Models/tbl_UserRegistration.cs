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
    
    public partial class tbl_UserRegistration
    {
        public int RegId { get; set; }
        public string EmailId { get; set; }
        public string PhoneNo { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
        public string RoleId { get; set; }
    }
}