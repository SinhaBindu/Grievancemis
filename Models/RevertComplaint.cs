using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Grievancemis.Models
{
    public class RevertComplaint
    {
        public int TeamId_pk { get; set; }

        public Guid GrievanceId_fk { get; set; }

        [Display(Name = "RevertType")]
        [Required]
        public int RevertTypeId { get; set; }

        [Display(Name = "Grievance Message")]
        [Required]
        public string TeamRevertMessage { get; set; }
        public int RoleId { get; set; } 


        public DateTime TeamRevert_Date { get; set; }
        public int Status { get; set; }
        public bool IsSent { get; set; } 
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}