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
    
    public partial class m_District_Master
    {
        public int DistrictId_pk { get; set; }
        public Nullable<int> StateId_fk { get; set; }
        public string StateName { get; set; }
        public string DistrictName { get; set; }
    }
}