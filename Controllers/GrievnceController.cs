using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Grievancemis.Models;

namespace Grievancemis.Controllers
{
    public class GrievnceController : Controller
    {
        private Grievance_DBEntities db = new Grievance_DBEntities();
        // GET: Grievnce
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GrievanceForm()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GrievanceForm(GrivanceModel grievanceModel)
        {
            try
            {
                int res = 0;
                if (ModelState.IsValid)
                {
                    using (var db = new Grievance_DBEntities())
                    {
                        Tbl_Grievance tbl_Grievance = new Tbl_Grievance
                        {
                            Id = Guid.NewGuid(),
                            Email = grievanceModel.Email,
                            Name = grievanceModel.Name,
                            PhoneNo = grievanceModel.PhoneNo,
                            GrievanceType = grievanceModel.GrievanceType,
                            StateId = grievanceModel.StateId,
                            Location = grievanceModel.Location,
                            Title = grievanceModel.Title,
                            Grievance_Message = grievanceModel.GrievanceMessage,
                            IsConsent = grievanceModel.IsConsent,
                            IsActive = true,
                            CreatedOn = DateTime.Now
                        };
                        db.Tbl_Grievance.Add(tbl_Grievance);
                        res = db.SaveChanges();
                    }
                    if (res > 0)
                    {
                        return Json(new { success = true, message = "Data saved successfully!" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Failed to save data. Please try again." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Invalid input. Please check your form data." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        public ActionResult GetGrievances()
        {
            using (var db = new Grievance_DBEntities())
            {
                var grievances = db.Tbl_Grievance.ToList();
                var grievanceModels = grievances.Select(g => new GrivanceModel
                {
                    Email = g.Email,
                    Name = g.Name,
                    PhoneNo = g.PhoneNo,
                    GrievanceType = g.GrievanceType.Value,
                    StateId = g.StateId.Value,
                    Location = g.Location,
                    Title = g.Title,
                    GrievanceMessage = g.Grievance_Message,
                    IsConsent = g.IsConsent.Value
                }).ToList();
                return View("_GrievanceInput", grievanceModels);
            }
        }

    }
}