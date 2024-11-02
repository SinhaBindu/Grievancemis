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
                        IsActive = true,
                        TeamRevert_Date = DateTime.Now.Date,
                        CreatedBy = User.Identity.Name,
                        CreatedOn = DateTime.Now
                    };

                    db.Tbl_TeamRevertComplain.Add(teamRevertComplain);
                    db.SaveChanges(); // Save changes to get the GrievanceId_fk

                    // Handle file uploads
                    var files = Request.Files;
                    if (files.Count > 0)
                    {
                        string uploadFolderPath = Server.MapPath("~/Doc_Upload/User Upload Images"); // Path to save uploaded files

                        if (!Directory.Exists(uploadFolderPath))
                        {
                            Directory.CreateDirectory(uploadFolderPath); // Create directory if it doesn't exist
                        }

                        foreach (string fileKey in files)
                        {
                            HttpPostedFileBase file = files[fileKey];
                            if (file != null && file.ContentLength > 0)
                            {
                                string fileName = Path.GetFileName(file.FileName);
                                string filePath = Path.Combine(uploadFolderPath, fileName);
                                file.SaveAs(filePath); // Save the file

                                // Save the file path in Tbl_Grievance_Documents
                                Tbl_Grievance_Documents grievanceDocument = new Tbl_Grievance_Documents
                                {
                                    GrievanceId = filterModel.GrievanceId_fk,
                                    DocumentPath = filePath,
                                    IsActive = true,
                                    CreatedBy = User.Identity.Name,
                                    CreatedOn = DateTime.Now
                                };

                                db.Tbl_Grievance_Documents.Add(grievanceDocument);
                            }
                        }
                        db.SaveChanges(); // Save changes for documents
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
        public ActionResult DetailsCId()
        {
            // Fetch the details based on the CaseId (id)
            //var details = SP_Model.GetDetailsByCaseId(id); // Implement this method in your SP_Model

            //if (details == null)
            //{
            //    return HttpNotFound("Details not found.");
            //}

            return View(); // Return a view that displays the details
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