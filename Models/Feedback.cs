using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Grievancemis.Models
{

    public class Feedback
    {
        public int Feedback_Id { get; set; }

        [Display(Name = "Feedback")]
        [Required(ErrorMessage = "Feedback is required.")]
        public string Grievance_Feedback { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string FormDt { get; set; }
        public string ToDT { get; set; }
    }
}