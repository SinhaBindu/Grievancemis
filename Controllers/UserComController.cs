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
        //public ActionResult GetDownGImgDocZip(FilterModel filtermodel)
        //{
        //    DataTable dt = new DataTable();
        //    dt = SP_Model.GetUserGList(filtermodel);
        //    if (dt == null || dt.Rows.Count == 0)
        //    {
        //        return HttpNotFound("No documents found for the specified grievance.");
        //    }
        //    // Define the folder path where images and PDFs are stored
        //    string folderPath = Server.MapPath("~/Doc_Upload"); // Example path, change as needed
        //    var random = new Random();
        //    int month = random.Next(1, 1200);
        //    // Define the path for the temporary zip file
        //    string zipFileName = $"{filtermodel}.zip";
        //    string zipPath = Path.Combine(folderPath, zipFileName);

        //    // Make sure the TempZips directory exists, if not, create it
        //    if (!Directory.Exists(Server.MapPath("~/Doc_Upload/ImageZip")))
        //    {
        //        Directory.CreateDirectory(Server.MapPath("~/Doc_Upload/ImageZip"));
        //    }
        //    byte[] fileBytes;
        //    // Create the zip file
        //    using (var zip = System.IO.Compression.ZipFile.Open(zipPath, ZipArchiveMode.Create))
        //    {
        //        // Get all image and PDF files from the folder
        //        //var files = Directory.GetFiles(folderPath, ".");
        //        // .Where(f => f.EndsWith(".jpg") || f.EndsWith(".jpeg") || f.EndsWith(".png") || f.EndsWith(".pdf") || f.EndsWith(".pdf"));

        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            string filePath = dr["FilePathFull"].ToString();
        //            string fileName = Path.GetFileName(filePath);
        //            // Add the file to the zip
        //            //var file = Directory.GetFiles(filePath, ".");
        //            string filePathc = Path.Combine(folderPath, fileName);

        //            zip.CreateEntryFromFile(filePathc, fileName);
        //        }
        //        // Loop through each file and add it to the zip
        //        //foreach (var file in files)
        //        //{
        //        //    // Get the file name from the full path
        //        //    string fileName = Path.GetFileName(file);
        //        //    // Add the file to the zip
        //        //    zip.CreateEntryFromFile(file, fileName);
        //        //}
        //    }

        //    // Return the zip file as a download
        //    fileBytes = System.IO.File.ReadAllBytes(zipPath);
        //    if (System.IO.File.Exists(zipPath))
        //        System.IO.File.Delete(zipPath);

        //    return File(fileBytes, "application/zip", zipFileName);
        //}


        //    public ActionResult GetDownGImgDocZip(FilterModel filtermodel)
        //    {
        //        // Fetch the grievance documents based on the filter model
        //        DataTable dt = SP_Model.GetUserGList(filtermodel);
        //        if (dt == null || dt.Rows.Count == 0)
        //        {
        //            return HttpNotFound("No documents found for the specified grievance.");
        //        }

        //        // Define the folder path where images and PDFs are stored
        //        string folderPath = Server.MapPath("~/Doc_Upload"); // Adjust this path as needed

        //        // Generate a unique name for the zip file
        //        string zipFileName = $"CombinedFiles_{Guid.NewGuid()}.zip"; // Use a GUID for uniqueness
        //        string zipPath = Path.Combine(folderPath, zipFileName);

        //        // Create the zip file
        //        using (var zipStream = new FileStream(zipPath, FileMode.Create))
        //        {
        //            using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
        //            {
        //                foreach (DataRow dr in dt.Rows)
        //                {
        //                    string filePath = dr["DocUpload"].ToString(); // Ensure this column name matches your DataTable
        //                    string fullPath = Path.Combine(folderPath, filePath.Trim());

        //                    // Check if the file exists before adding it to the zip
        //                    if (System.IO.File.Exists(fullPath))
        //                    {
        //                        zip.CreateEntryFromFile(fullPath, Path.GetFileName(fullPath));
        //                    }
        //                }
        //            }
        //        }

        //        // Return the zip file as a download
        //        byte[] fileBytes = System.IO.File.ReadAllBytes(zipPath);

        //        // Clean up by deleting the zip file after sending it
        //        System.IO.File.Delete(zipPath);

        //        return File(fileBytes, "application/zip", zipFileName);

        //}


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