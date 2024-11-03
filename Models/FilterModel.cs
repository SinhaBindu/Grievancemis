using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Grievancemis.Models
{
    public class FilterModel
    {
        [Display(Name = "Grievance Type")]
        public string TypeGId { get; set; }
        [Display(Name = "State")]
        public string StateId { get; set; }
        [Display(Name = "State")]
        public string State { get; set; }
        [Display(Name = "From Date")]
        public string FromDt { get; set; }
        [Display(Name = "To Date")]
        public string ToDt { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        public string DOB { get; set; }
        [Display(Name = "Role")]
        public string RoleId { get; set; }
        [Display(Name = "Role")]
        public string Roles { get; set; }
        [Display(Name = "Name")]
        public string UserId { get; set; }
        public string CutUser { get; set; }
        [Display(Name = "Month")]
        public int MonthId { get; set; }
        [Display(Name = "Month")]
        public string Month { get; set; }
        [Display(Name = "Year")]
        public int YearId { get; set; }
        [Display(Name = "Year")]
        public string Year { get; set; }

        //public Guid Regid { get; set; }

        public Guid GrievanceId_fk { get; set; }

        [Display(Name = "Revert Type")]
        //[Required]
        public int RevertTypeId { get; set; }


        [Display(Name = "Message")]
        //[Required]
        public string TeamRevertMessage { get; set; }


        [Display(Name = "Email")]
        //[Required]
        public string EmailID { get; set; }


        [Display(Name = "Document Upload")]
        public HttpPostedFileBase DocUpload { get; set; }

        public string Id { get; set; }

        public List<RevertComplaint> Reverts { get; set; }
        //public int MaxRevertTypeId { get; set; }



        //[Display(Name = "Attendance Date")]
        //public DateTime AttendanceDt { get; set; }
        //[Display(Name = "Attendance Start Time")]
        //public TimeSpan AttendanceStartTime { get; set; }
        //[Display(Name = "Attendance End Time")]
        //public TimeSpan AttendanceEndTime { get; set; }
        //public string AssessmentScheduleId { get; set; }
        //public string EmailId { get; set; }
        //public string Password { get; set; }
        //public string RandomValue { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; }
    }
}