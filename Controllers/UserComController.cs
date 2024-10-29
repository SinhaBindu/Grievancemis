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
        public ActionResult GetDownGImgDocZip(FilterModel filtermodel)
        {
            DataTable dt = new DataTable();
            dt = SP_Model.GetUserGList(filtermodel);
            if (dt == null || dt.Rows.Count == 0)
            {
                return HttpNotFound("No documents found for the specified grievance.");
            }
            // Define the folder path where images and PDFs are stored
            string folderPath = Server.MapPath("~/Doc_Upload"); // Example path, change as needed
            var random = new Random();
            int month = random.Next(1, 1200);
            // Define the path for the temporary zip file
            string zipFileName = $"{filtermodel}.zip";
            string zipPath = Path.Combine(folderPath, zipFileName);

            // Make sure the TempZips directory exists, if not, create it
            if (!Directory.Exists(Server.MapPath("~/Doc_Upload/ImageZip")))
            {
                Directory.CreateDirectory(Server.MapPath("~/Doc_Upload/ImageZip"));
            }
            byte[] fileBytes;
            // Create the zip file
            using (var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
            {
                // Get all image and PDF files from the folder
                //var files = Directory.GetFiles(folderPath, ".");
                // .Where(f => f.EndsWith(".jpg") || f.EndsWith(".jpeg") || f.EndsWith(".png") || f.EndsWith(".pdf") || f.EndsWith(".pdf"));

                foreach (DataRow dr in dt.Rows)
                {
                    string filePath = dr["DocumentPath"].ToString();
                    string fileName = Path.GetFileName(filePath);
                    // Add the file to the zip
                    //var file = Directory.GetFiles(filePath, ".");
                    string filePathc = Path.Combine(folderPath, fileName);

                    zip.CreateEntryFromFile(filePathc, fileName);
                }
                // Loop through each file and add it to the zip
                //foreach (var file in files)
                //{
                //    // Get the file name from the full path
                //    string fileName = Path.GetFileName(file);
                //    // Add the file to the zip
                //    zip.CreateEntryFromFile(file, fileName);
                //}
            }

            // Return the zip file as a download
            fileBytes = System.IO.File.ReadAllBytes(zipPath);
            if (System.IO.File.Exists(zipPath))
                System.IO.File.Delete(zipPath);

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