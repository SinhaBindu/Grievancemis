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
    
    public partial class Tbl_Grievance
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Tbl_Grievance()
        {
            this.Tbl_Grievance_Documents = new HashSet<Tbl_Grievance_Documents>();
            this.Tbl_TeamRevertComplain = new HashSet<Tbl_TeamRevertComplain>();
            this.Tbl_UserRevertComplain = new HashSet<Tbl_UserRevertComplain>();
        }
    
        public System.Guid Id { get; set; }
        public Nullable<long> CaseId { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public Nullable<System.DateTime> ComplainRegDate { get; set; }
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public Nullable<int> GrievanceType { get; set; }
        public string Other { get; set; }
        public Nullable<int> StateId { get; set; }
        public string Location { get; set; }
        public Nullable<bool> Issent { get; set; }
        public string Title { get; set; }
        public string Grievance_Message { get; set; }
        public Nullable<bool> IsConsent { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> IsDeletedOn { get; set; }
        public string DocUpload { get; set; }
        public Nullable<int> RoleId { get; set; }
        public string UserRegId { get; set; }
        public Nullable<int> RevertType_Id { get; set; }
        public Nullable<System.DateTime> RevertTypeDate { get; set; }
        public Nullable<int> Duration { get; set; }
        public Nullable<bool> IsSentMailDuration { get; set; }
        public Nullable<System.DateTime> SentMailDurationDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tbl_Grievance_Documents> Tbl_Grievance_Documents { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tbl_TeamRevertComplain> Tbl_TeamRevertComplain { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tbl_UserRevertComplain> Tbl_UserRevertComplain { get; set; }
    }
}
