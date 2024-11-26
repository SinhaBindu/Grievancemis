using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Grievancemis.Models
{
    public class AssignTo
    {
        public int AssignCase_Idpk { get; set; }

        public Guid AspUser_Idfk { get; set; }

        public int Role_Idfk { get; set; }

        public Guid Grievance_Idfk { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}