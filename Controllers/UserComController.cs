using Grievancemis.Manager;
using Grievancemis.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace Grievancemis.Controllers
{
    [Authorize]
    public class UserComController : Controller
    {
        // GET: UserCom
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UserGList()
        {
            return View();
        }
        public ActionResult GetUserGList(FilterModel filtermodel)
        {
            try
            {
                bool IsCheck = false;
                var dt = SP_Model.GetUserGList(filtermodel);
                if (dt.Rows.Count > 0)
                {
                    IsCheck = true;
                }
                var html = ConvertViewToString("_UserGData", dt);
                var res = Json(new { IsSuccess = IsCheck, Data = html }, JsonRequestBehavior.AllowGet);
                res.MaxJsonLength = int.MaxValue;
                return res;

            }
            catch (Exception ex)
            {
                string er = ex.Message;
                return Json(new { IsSuccess = false, Data = "There are communication error...." }, JsonRequestBehavior.AllowGet); throw;
            }
        }
        public ActionResult GetDownGImgDocZip(Guid grievanceId)
        {
            DataTable dt = SP_Model.GetUserGList(new FilterModel { Id = grievanceId.ToString() });
            if (dt.Rows.Count == 0)
            {
                return HttpNotFound("No documents found for the specified grievance.");
            }

            string folderPath = Server.MapPath("~/Doc_Upload");
            string zipFileName = $"{grievanceId}.zip";
            string zipPath = Path.Combine(folderPath, zipFileName);

            if (!Directory.Exists(Server.MapPath("~/Doc_Upload/ImageZip")))
            {
                Directory.CreateDirectory(Server.MapPath("~/Doc_Upload/ImageZip"));
            }

            // Use a HashSet to track added file names
            HashSet<string> addedFiles = new HashSet<string>();

            using (var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string filePath = dr["DocumentPath"].ToString();
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        string fileName = Path.GetFileName(filePath);
                        string filePathc = Path.Combine(folderPath, fileName);

                        // Check if the file has already been added
                        if (!addedFiles.Contains(fileName) && System.IO.File.Exists(filePathc))
                        {
                            zip.CreateEntryFromFile(filePathc, fileName);
                            addedFiles.Add(fileName); // Mark this file as added
                        }
                    }
                }
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(zipPath);
            System.IO.File.Delete(zipPath); // Clean up the zip file after download

            return File(fileBytes, "application/zip", zipFileName);
        }
        public ActionResult UserRevertCPost(FilterModel filterModel)
        {
            try
            {
                if (filterModel.GrievanceId_fk == Guid.Empty || filterModel.RevertTypeId == 0 || string.IsNullOrWhiteSpace(filterModel.TeamRevertMessage))
                {
                    return Json(new { success = false, message = "All fields are required." });
                }

                using (var db = new Grievance_DBEntities())
                {
                    // Create a new record in Tbl_TeamRevertComplain
                    Tbl_TeamRevertComplain teamRevertComplain = new Tbl_TeamRevertComplain
                    {
                        GrievanceId_fk = filterModel.GrievanceId_fk,
                        RevertTypeId = filterModel.RevertTypeId,
                        TeamRevertMessage = filterModel.TeamRevertMessage,
                        RoleId = Convert.ToInt32(MvcApplication.CUser.RoleId),
                        IsActive = true,
                        TeamRevert_Date = DateTime.Now.Date,
                        CreatedBy = User.Identity.Name,
                        CreatedOn = DateTime.Now
                    };

                    db.Tbl_TeamRevertComplain.Add(teamRevertComplain);
                    db.SaveChanges(); // Save changes to get the GrievanceId_fk

                    // Handle email sending
                    DataTable dt = SP_Model.GetRevartMail(teamRevertComplain.GrievanceId_fk.ToString());
                    if (dt.Rows.Count > 0)
                    {
                        string grievanceId = dt.Rows[0]["GrievanceId_fk"].ToString();
                        string email = dt.Rows[0]["Email"].ToString();
                        string name = dt.Rows[0]["Name"].ToString();
                        string revertMessage = dt.Rows[0]["TeamRevertMessage"].ToString();
                        string revertStatus = dt.Rows[0]["RevertStatus"].ToString();

                        // Send email notification
                        int emailResult = CommonModel.SendMailRevartPartUser(email, grievanceId, name, revertMessage, revertStatus);

                        if (emailResult > 0)
                        {
                            return Json(new { success = true, message = "Data saved and email sent successfully!" });
                        }
                    }

                    return Json(new { success = true, message = "Data saved successfully!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        
        public ActionResult URevartList()
        {
            return View();
        }

        public ActionResult GetURevartList(FilterModel filtermodel)
        {
            try
            {
                bool IsCheck = false;
                var dt = SP_Model.GetRevartList(filtermodel);
                if (dt.Rows.Count > 0)
                {
                    IsCheck = true;
                }
                var html = ConvertViewToString("_UComData", dt);
                var res = Json(new { IsSuccess = IsCheck, Data = html }, JsonRequestBehavior.AllowGet);
                res.MaxJsonLength = int.MaxValue;
                return res;

            }
            catch (Exception ex)
            {
                string er = ex.Message;
                return Json(new { IsSuccess = false, Data = "There are communication error...." }, JsonRequestBehavior.AllowGet); throw;
            }
        }
        public ActionResult DetailsCId(string id)
        {
            try
            {
                var dt = SP_Model.GetTeamRevertDetailsByCID(id.ToString());

                if (dt.Rows.Count == 0)
                {
                    return HttpNotFound("Details not found.");
                }

                var model = new FilterModel
                {
                    Reverts = new List<RevertComplaint>()
                };

                foreach (DataRow row in dt.Rows)
                {
                    model.Reverts.Add(new RevertComplaint
                    {
                        RevertTypeId = row["RevertTypeId"] != DBNull.Value ? (int)row["RevertTypeId"] : 0,
                        TeamRevertMessage = row["TeamRevertMessage"].ToString(),
                        TeamRevert_Date = row["TeamRevert_Date"] != DBNull.Value ? (DateTime)row["TeamRevert_Date"] : DateTime.MinValue,
                        RoleId = row["RoleId"] != DBNull.Value ? (int)row["RoleId"] : 0
                    });
                }

                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception details for debugging
                string errorMessage = $"Error: {ex.Message}, StackTrace: {ex.StackTrace}";
                return View("Error", new HandleErrorInfo(ex, "UserCom", "DetailsCId"));
            }
        }





        private string ConvertViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);
                vResult.View.Render(vContext, writer);
                return writer.ToString();
            }
        }
    }
}