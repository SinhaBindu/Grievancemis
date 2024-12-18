using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Grievancemis.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        [Display(Name = "Feedback")]
        [Required]
        [DataType(DataType.MultilineText)]
        public string Feedback_Id { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}