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
using Microsoft.AspNet.Identity;



namespace Grievancemis.Controllers
{
    [Authorize]
    [SessionCheckAttribute]
    public class ComplainController : Controller
    {
        // GET: Complain
        private Grievance_DBEntities db = new Grievance_DBEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GrievanceList()
        {
            return View();
        }
        public ActionResult GetGrievanceList(FilterModel filtermodel)
        {
            try
            {
                bool IsCheck = false;
                var dt = SP_Model.GetGrievanceList(filtermodel);
                if (dt.Rows.Count > 0)
                {
                    IsCheck = true;
                }
                var html = ConvertViewToString("_GrievanceData", dt);
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
        public ActionResult GetDownGImgDocZip(Guid? grievanceId)
        {
            DataTable dt = SP_Model.GetGrievanceDoc(new FilterModel { GrievanceId = grievanceId.ToString() });
            if (dt.Rows.Count == 0)
            {
                return HttpNotFound("No documents found for the specified grievance.");
            }

            string folderPath = Server.MapPath("~/Doc_Upload");
            string zipFileName = $"{grievanceId}.zip";
            //string zipPath = Path.Combine(folderPath, zipFileName);

            if (!Directory.Exists(Server.MapPath("~/Doc_Upload/ImageZip")))
            {
                Directory.CreateDirectory(Server.MapPath("~/Doc_Upload/ImageZip"));
            }

            // Use a HashSet to track added file names
            HashSet<string> addedFiles = new HashSet<string>();
            var random = new Random();
            int month = random.Next(1, 1200);
            // Define the path for the temporary zip file
            string zipPath = Server.MapPath("~/Doc_Upload/CombinedFilesZip" + month + DateTime.Now.ToDateTimeDDMMYYYY() + ".zip");

            using (var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string filePath = dr["DocumentPath"].ToString();
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        //string fileName = Path.GetFileName(filePath);
                        //string filePathc = Path.Combine(folderPath, fileName);

                        //// Check if the file has already been added
                        //if (!addedFiles.Contains(fileName) && System.IO.File.Exists(filePathc))
                        //{
                        //    zip.CreateEntryFromFile(filePathc, fileName);
                        //    addedFiles.Add(fileName); // Mark this file as added
                        //}
                        string fileName = Path.GetFileName(filePath);
                        // Add the file to the zip
                        //var file = Directory.GetFiles(filePath, "*.*");
                        string filePathc = Path.Combine(folderPath + "/GID" + grievanceId, fileName);

                        zip.CreateEntryFromFile(filePathc, fileName);
                    }
                }
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(zipPath);
            System.IO.File.Delete(zipPath); // Clean up the zip file after download

            return File(fileBytes, "application/zip", zipFileName);
        }

        public ActionResult RevertCList()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RevertCPost(FilterModel filterModel)
        {
            try
            {
                if (MvcApplication.CUser == null)
                {
                    return Json(new { success = false, message = "All fileds are required.", Data = 201 });
                }
                if (MvcApplication.CUser.RoleId == "0" || string.IsNullOrWhiteSpace(MvcApplication.CUser.RoleId))
                {
                    return Json(new { success = false, message = "All fileds are required.", Data = 201 });
                }
                //DataTable dtcheck = SP_Model.GetSPCheckRevertAlready();
                //if (dtcheck.Rows.Count > 0)
                //{
                //    return Json(new { success = false, message = "This record is already exists.....", resdata = 1 });
                //}
                string Greid = string.Empty;
                if (filterModel.GrievanceId_fk == Guid.Empty || filterModel.RevertTypeId == 0 || string.IsNullOrWhiteSpace(filterModel.TeamRevertMessage))
                {
                    return Json(new { success = false, message = "All fileds are required.", Data = 0 });
                }
                using (var db = new Grievance_DBEntities())
                {
                    Tbl_TeamRevertComplain teamRevertComplain = new Tbl_TeamRevertComplain();

                    teamRevertComplain.GrievanceId_fk = filterModel.GrievanceId_fk;
                    teamRevertComplain.RevertTypeId = filterModel.RevertTypeId;
                    teamRevertComplain.TeamRevertMessage = filterModel.TeamRevertMessage;
                    teamRevertComplain.TeamRoleId = Convert.ToInt32(MvcApplication.CUser.RoleId);
                    teamRevertComplain.IsActive = true;
                    teamRevertComplain.TeamRevert_Date = DateTime.Now.Date;
                    teamRevertComplain.CreatedBy = MvcApplication.CUser.UserId;
                    teamRevertComplain.CreatedOn = DateTime.Now;

                    db.Tbl_TeamRevertComplain.Add(teamRevertComplain);
                    int res = db.SaveChanges();
                    var tblgrive = db.Tbl_Grievance.Find(filterModel.GrievanceId_fk);
                    tblgrive.RevertType_Id = filterModel.RevertTypeId;
                    tblgrive.RevertTypeDate = DateTime.Now.Date;
                    db.SaveChanges();
                    if (res > 0)
                    {
                        DataTable dt = new DataTable();
                        dt = SP_Model.GetRevartMail(teamRevertComplain.GrievanceId_fk.ToString(), teamRevertComplain.TeamId_pk.ToString());
                        if (dt.Rows.Count > 0)
                        {
                            if (Greid.Length <= 0)
                            {
                                Greid = SP_Model.Usp_GetCIdRevart(dt.Rows[0]["GrievanceId_fk"].ToString()).Rows[0]["CaseId"].ToString();
                            }
                            DataTable dtteamemails = SP_Model.GetTeamMailID();//RolesIdcont.Community
                            res = CommonModel.SendMailRevartPartUser(dtteamemails.Rows[0]["EmailList"].ToString(), dt.Rows[0]["Email"].ToString(), Greid, dt.Rows[0]["Name"].ToString(), dt.Rows[0]["TeamRevertMessage"].ToString(), dt.Rows[0]["RevertStatus"].ToString());
                        }
                        if (res > 0)
                        {
                            teamRevertComplain.TeamIsSent = true;
                            db.SaveChanges();
                            return Json(new { success = true, message = "Data saved and mail sended successfully!", Data = 0 });
                        }
                        else
                        {
                            return Json(new { success = true, message = "Data saved successfully!", Data = 0 });
                        }
                    }
                    else
                    {
                        return Json(new { success = true, message = "Record not Submitted.", Data = 0 });
                    }
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "An error occurred: " + ex.Message, Data = 0 });
            }
        }
        public ActionResult RevartList(string GId)
        {
            FilterModel model = new FilterModel();
            model.GrievanceId = GId;
            return View(model);
        }
        public ActionResult GetRevartList(FilterModel filtermodel)
        {
            try
            {
                bool IsCheck = false;
                var dt = SP_Model.GetRevartList(filtermodel);
                if (dt.Rows.Count > 0)
                {
                    IsCheck = true;
                }
                var html = ConvertViewToString("_RevertData", dt);
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