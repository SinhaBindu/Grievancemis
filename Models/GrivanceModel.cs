﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Grievancemis.Models
{
    public class GrivanceModel
    {
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "Email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Name")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Phone Number")]
        [Required]
        public string PhoneNo { get; set; }

        [Display(Name = "Grievance Type")]
        [Required]
        public int GrievanceType { get; set; }

        [Display(Name = "State")]
        [Required]
        public int StateId { get; set; }

        [Display(Name = "Location")]
        [Required]
        public string Location { get; set; }

        [Display(Name = "Is Sent")]
        [Required]
        public bool IsSent { get; set; }

        [Display(Name = "Title")]
        [Required]
        public string Title { get; set; }

        [Display(Name = "Grievance Message")]
        [Required]
        public string GrievanceMessage { get; set; }

        [Display(Name = "Is Consent")]
        [Required]
        public bool IsConsent { get; set; }

        [Display(Name = "Is Active")]
        
        public bool IsActive { get; set; }

        [Display(Name = "Created By")]
        
        public string CreatedBy { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Updated By")]
        public string UpdatedBy { get; set; }

        [Display(Name = "Updated On")]
        public DateTime UpdatedOn { get; set; }

        [Display(Name = "Is Deleted")]
        public bool IsDeleted { get; set; }

        [Display(Name = "Deleted On")]
        public DateTime? DeletedOn { get; set; }

       
        public List<GrivanceModel> Grievances { get; set; }
    }
}