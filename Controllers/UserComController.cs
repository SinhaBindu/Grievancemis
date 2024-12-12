using Grievancemis.Manager;
using Grievancemis.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using static Grievancemis.Manager.CommonModel;
using static ImageResizer.Plugins.Basic.Image404;
namespace Grievancemis.Controllers
{
    [Authorize]
    [SessionCheckAttribute]
    public class UserComController : Controller
    {
        private Grievance_DBEntities db = new Grievance_DBEntities();
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
        public ActionResult UserRevertCPost(FilterModel filterModel)
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
                if (filterModel.GrievanceId_fk == Guid.Empty || string.IsNullOrWhiteSpace(filterModel.TeamRevertMessage))
                {
                    return Json(new { success = false, message = "All fields are required.", Data = 0 });
                }
                if (filterModel.RevertId == 0)
                {
                    return Json(new { success = false, message = "Revert Id fields are required.", Data = 0 });
                }
                if (filterModel.Revertcb_value.ToString() == string.Empty || string.IsNullOrWhiteSpace(filterModel.Revertcb_value.ToString()))
                {
                    return Json(new { success = false, message = "All fields are required.", Data = 0 });
                }
                if (filterModel.Revertcb_value == 2)
                {
                    if (filterModel.UserRevertId == 0 && filterModel.RevertTypeId == 2)
                    {
                        return Json(new { success = false, message = "Revert Type fields are required.", Data = 0 });
                    }
                }
                //DataTable dtcheck = SP_Model.GetSPCheckRevertAlready();
                //if (dtcheck.Rows.Count > 0)
                //{
                //    return Json(new { success = false, message = "This record is already exists.....", resdata = 1 });
                //}

                using (var db_ = new Grievance_DBEntities())
                {
                    // Create a new record in Tbl_TeamRevertComplain
                    Tbl_TeamRevertComplain teamRevertComplain = db_.Tbl_TeamRevertComplain.Find(filterModel.RevertId);
                    if (teamRevertComplain != null)
                    {
                        teamRevertComplain.GrievanceId_fk = filterModel.GrievanceId_fk;
                        if (filterModel.RevertTypeId == 2)
                        {
                            teamRevertComplain.UserRevertId = filterModel.UserRevertId;
                        }
                        teamRevertComplain.Revertcb_value = filterModel.Revertcb_value;
                        ////teamRevertComplain.RevertTypeId = filterModel.RevertTypeId;
                        teamRevertComplain.UserRevertMessage = filterModel.TeamRevertMessage;
                        teamRevertComplain.UserRoleId = Convert.ToInt32(MvcApplication.CUser.RoleId);
                        teamRevertComplain.UserRevert_Date = DateTime.Now.Date;
                        teamRevertComplain.UpdatedBy = MvcApplication.CUser.UserId;
                        teamRevertComplain.UpdatedOn = DateTime.Now;

                        var URLPath = string.Empty;
                        if (filterModel.DocUpload != null && filterModel.DocUpload.ContentLength > 0)
                        {
                            FileModel modelfile = CommonModel.saveFile(filterModel.DocUpload, "GID" + teamRevertComplain.GrievanceId_fk, filterModel.RevertId.ToString() + DateTime.Now.Date.ToDateTimeDDMMYYYY());
                            string fileExtension = Path.GetExtension(filterModel.DocUpload.FileName).ToLower();

                            // Check if the file extension is allowed
                            if (fileExtension == ".zip" || fileExtension == ".png" || fileExtension == ".jpg" || fileExtension == ".pdf" || fileExtension == ".doc" || fileExtension == ".jpeg")
                            {
                                if (modelfile.IsvalidFile)
                                {
                                    URLPath = modelfile.FilePathFull;
                                }
                                else
                                {
                                    return Json(new { success = false, message = "File size is too large. Only files up to 20MB are allowed." });
                                }
                            }
                            else
                            {
                                return Json(new { success = false, message = "Invalid file extension. Only zip, png, jpg, pdf, doc, and jpeg files are allowed." });
                            }

                            // Save the file paths in the Tbl_Grievance_Documents table
                            Tbl_Grievance_Documents tbl_Grievance_Documents = new Tbl_Grievance_Documents
                            {
                                GrievanceId = teamRevertComplain.GrievanceId_fk,
                                RevertId = filterModel.RevertId,
                                DocumentPath = URLPath,
                                IsActive = true,
                                CreatedOn = DateTime.Now
                            };
                            db.Tbl_Grievance_Documents.Add(tbl_Grievance_Documents);
                            db.SaveChanges();
                        }

                        //db.Tbl_TeamRevertComplain.Add(teamRevertComplain);
                        int resD = db_.SaveChanges(); // Save changes to get the GrievanceId_fk
                        if (resD > 0)
                        {
                            if (filterModel.RevertTypeId == 2)
                            {
                                var maintbl = db_.Tbl_Grievance.Find(teamRevertComplain.GrievanceId_fk);
                                maintbl.RevertType_Id = filterModel.UserRevertId;
                                maintbl.RevertTypeDate = DateTime.Now;
                                db_.SaveChanges();
                            }
                        }

                        // Handle email sending
                        DataTable dt = SP_Model.GetRevartMail(teamRevertComplain.GrievanceId_fk.ToString(), filterModel.RevertId.ToString());
                        if (dt.Rows.Count > 0)
                        {
                            string CaseId = dt.Rows[0]["CaseId"].ToString();
                            string email = dt.Rows[0]["Email"].ToString();
                            string name = dt.Rows[0]["Name"].ToString();
                            string revertMessage = dt.Rows[0]["UserRevertMessage"].ToString();
                            //string revertStatus = //(filterModel.RevertTypeId == 1) ? "Clarification" : "Closed";
                            string revertStatus = dt.Rows[0]["RevertStatus"].ToString();

                            DataTable dtteamemails = SP_Model.GetTeamMailID(Convert.ToInt16(RolesIdcont.Community));
                            // Send email notification
                            int emailResult = 0;
                            if (filterModel.Revertcb_value == 1)//Head Chief Executive Officer & Country Director Get Email Id
                            {
                                dtteamemails = SP_Model.GetTeamMailID(Convert.ToInt16(RolesIdcont.Head));//RolesIdcont.Head
                            }
                            emailResult = CommonModel.SendMailRevartPartUser(dtteamemails.Rows[0]["EmailList"].ToString(), email, CaseId, name, revertMessage, revertStatus);

                            if (emailResult > 0)
                            {
                                teamRevertComplain.UserIsSent = true;
                                db_.SaveChanges();
                                return Json(new { success = true, message = "Data saved and email sent successfully!", Data = 0 });
                            }
                        }

                        return Json(new { success = true, message = "Data saved successfully!", Data = 0 });
                    }
                    return Json(new { success = false, message = "An error occurred:", Data = 0 });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message, Data = 0 });
            }
        }
        public ActionResult URevartList(string GId)
        {
            FilterModel model = new FilterModel();
            model.GrievanceId = GId;
            return View(model);
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
                var html = ConvertViewToString("_URevertData", dt);
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
        public ActionResult AddNGrievance()
        {
            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            GrivanceModel model = new GrivanceModel();
            model.Email = MvcApplication.CUser.EmailId.Trim();
            model.Name = MvcApplication.CUser.Name.Trim();
            model.PhoneNo = MvcApplication.CUser.Phone.Trim();
            return View(model);
        }
        [HttpPost]
        public ActionResult AddNGrievance(GrivanceModel grievanceModel)
        {
            try
            {
                //DataTable dtcheck = SP_Model.GetSPCheckGrievanceAlready(grievanceModel.Email.Trim(), DateTime.Now.Date.ToDateTimeyyyyMMdd());
                //if (dtcheck.Rows.Count > 0)
                //{
                //    return Json(new { success = false, message = "This record is already exists.....", resdata = 1 });
                //}

                int res = 0; System.Text.StringBuilder str = new System.Text.StringBuilder(); string partymail = string.Empty, Greid = string.Empty, stGuid = string.Empty;
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
                            Gender = grievanceModel.Gender,
                            ComplainRegDate = DateTime.Now.Date,
                            GrievanceType = grievanceModel.GrievanceType,
                            StateId = grievanceModel.StateId,
                            //Location = grievanceModel.Location,
                            Other = grievanceModel.Other,
                            Title = grievanceModel.Title,
                            Grievance_Message = grievanceModel.GrievanceMessage,
                            IsConsent = grievanceModel.IsConsent,
                            IsActive = true,
                            CreatedOn = DateTime.Now
                        };
                        var igtype = 0; var istype = 0;
                        igtype = Convert.ToInt32(tbl_Grievance.GrievanceType);
                        istype = Convert.ToInt32(tbl_Grievance.StateId);
                        var GType = db.M_GrievanceType.Where(b => b.Id == igtype)?.FirstOrDefault();
                        var SType = db.m_State_Master.Where(b => b.LGD_State_Code == istype)?.FirstOrDefault();

                        partymail = tbl_Grievance.Email.Trim();
                        Greid = Convert.ToString(tbl_Grievance.CaseId);
                        stGuid = Convert.ToString(tbl_Grievance.Id);
                        // Handle file upload
                        var URLPath = string.Empty;
                        if (grievanceModel.DocUpload != null && grievanceModel.DocUpload.ContentLength > 0)
                        {
                            FileModel modelfile = CommonModel.saveFile(grievanceModel.DocUpload, "GID" + stGuid, tbl_Grievance.Id.ToString() + DateTime.Now.Date.ToDateTimeDDMMYYYY());
                            string fileExtension = Path.GetExtension(grievanceModel.DocUpload.FileName).ToLower();

                            // Check if the file extension is allowed
                            if (fileExtension == ".zip" || fileExtension == ".png" || fileExtension == ".jpg" || fileExtension == ".pdf" || fileExtension == ".doc" || fileExtension == ".jpeg")
                            {
                                if (modelfile.IsvalidFile)
                                {
                                    URLPath = modelfile.FilePathFull;
                                }
                                else
                                {
                                    return Json(new { success = false, message = "File size is too large. Only files up to 20MB are allowed." });
                                }
                            }
                            else
                            {
                                return Json(new { success = false, message = "Invalid file extension. Only zip, png, jpg, pdf, doc, and jpeg files are allowed." });
                            }

                            // Save the file paths in the Tbl_Grievance table
                            tbl_Grievance.DocUpload = URLPath;

                            // Save the file paths in the Tbl_Grievance_Documents table
                            Tbl_Grievance_Documents tbl_Grievance_Documents = new Tbl_Grievance_Documents
                            {
                                GrievanceId = tbl_Grievance.Id,
                                DocumentPath = URLPath,
                                IsActive = true,
                                CreatedOn = DateTime.Now
                            };
                            db.Tbl_Grievance_Documents.Add(tbl_Grievance_Documents);
                        }
                        db.Tbl_Grievance.Add(tbl_Grievance);
                        res = db.SaveChanges();

                        partymail = tbl_Grievance.Email.Trim();
                        Greid = Convert.ToString(tbl_Grievance.CaseId);
                        if (!string.IsNullOrWhiteSpace(stGuid))
                        {
                            Greid = SP_Model.Usp_GetCaseIDwithguid(stGuid).Rows[0]["CaseId"].ToString();
                        }
                        str.Append("<table border='1'>");
                        str.Append("<tr><td>Gender</td><td>" + tbl_Grievance.Gender + "</td></tr>");
                        str.Append("<tr><td>Email</td><td>Name</td><td>Phone Number</td></tr>");
                        str.Append("<tr><td>" + tbl_Grievance.Email + "</td><td>" + tbl_Grievance.Name + "</td><td>" + tbl_Grievance.PhoneNo + "</td></tr>");
                        str.Append("<tr><td>Grievance Type</td><td>State Name</td><td>Title</td></tr>");
                        str.Append("<tr><td>" + GType.GrievanceType + "</td><td>" + SType.StateName + "</td><td>" + tbl_Grievance.Title + "</td></tr>");
                        str.Append("<tr><td>Other</td><td colspan='2'>Message</td></tr>");
                        str.Append("<tr><td>" + tbl_Grievance.Other + "</td><td colspan='2'>" + tbl_Grievance.Grievance_Message + "</td></tr>");
                        str.Append("</table>");
                    }
                    if (res > 0)
                    {
                        var asp = db.AspNetUsers.Where(x => x.Email == grievanceModel.Email.Trim())?.FirstOrDefault();
                        if (asp != null)
                        {
                            asp.Name = !string.IsNullOrWhiteSpace(grievanceModel.Name) ? grievanceModel.Name.Trim() : string.Empty;
                            asp.PhoneNumber = !string.IsNullOrWhiteSpace(grievanceModel.PhoneNo) ? grievanceModel.PhoneNo.Trim() : string.Empty;
                            db.SaveChanges();
                        }
                        DataTable dt = new DataTable();
                        dt = SP_Model.GetTeamMailID();
                        if (dt.Rows.Count > 0)
                        {
                            res = CommonModel.SendSucessfullMailForUserTeam(dt.Rows[0]["EmailList"].ToString(), str.ToString(), partymail, Greid, "New Case");
                            res = CommonModel.SendMailPartUser(partymail, Greid, grievanceModel.Name, "New Case");
                        }
                        if (res > 0)
                        {
                            return Json(new { success = true, message = "Your request is registered with Grievance reference id : " + Greid + "<br />" + "Mail sended successfully!" });//Data saved and mail sended successfully!
                        }
                        else
                        {
                            return Json(new { success = true, message = "Your request is registered with Grievance reference id :" + Greid });//Data saved successfully!
                        }

                    }
                    else
                    {
                        return Json(new { success = false, message = "Failed to save data. Please try again." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Invalid input. Please check your form data. All Fileds requied" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
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