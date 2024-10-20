using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime.Tree;
using Grievancemis.Manager;
using Grievancemis.Models;
using Newtonsoft.Json;

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
        public ActionResult GrievanceCaseAdd() { return View(); }
        [HttpPost]

        public ActionResult GrievanceCaseAdd(GrivanceModel grievanceModel)
        {
            try
            {
                int res = 0; System.Text.StringBuilder str = new System.Text.StringBuilder();string partymail = string.Empty, Greid = string.Empty;
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
                        var igtype = 0;var istype = 0;
                        igtype = Convert.ToInt32(tbl_Grievance.GrievanceType);
                        istype = Convert.ToInt32(tbl_Grievance.StateId);
                        var GType = db.M_GrievanceType.Where(b => b.Id == igtype).FirstOrDefault();
                        var SType = db.m_State_Master.Where(b => b.LGD_State_Code == istype).FirstOrDefault();
                        str.Append("<table border='1'>");
                        str.Append("<tr><td>Email</td><td>Name</td><td>Phone Number</td></tr>");
                        str.Append("<tr><td>"+ tbl_Grievance.Email+"</td><td>" + tbl_Grievance.Name + "</td><td>"+ tbl_Grievance.PhoneNo + "</td></tr>");
                        str.Append("<tr><td>Grievance Type</td><td>State Name</td><td>Title</td></tr>");
                        str.Append("<tr><td>" + GType.GrievanceType + "</td><td>" + SType.StateName + "</td><td>" + tbl_Grievance.Title + "</td></tr>");
                        str.Append("<tr><td>Location</td><td colspan='2'>Grievance_Message</td></tr>");
                        str.Append("<tr><td>" + tbl_Grievance.Location + "</td><td colspan='2'>" + tbl_Grievance.Grievance_Message + "</td></tr>");
                        str.Append("</table>");
                        partymail = tbl_Grievance.Email;
                        Greid = Convert.ToString(tbl_Grievance.Id);
                        // Handle file upload
                        if (grievanceModel.DocUpload != null && grievanceModel.DocUpload.ContentLength > 0)
                        {
                            string[] fileNames = grievanceModel.DocUpload.FileName.Split(',');
                            string[] fileExtensions = new string[fileNames.Length];
                            string[] filePaths = new string[fileNames.Length];

                            for (int i = 0; i < fileNames.Length; i++)
                            {
                                string fileName = Path.GetFileName(fileNames[i]);
                                string fileExtension = Path.GetExtension(fileName).ToLower();

                                // Check if the file extension is allowed
                                if (fileExtension == ".zip" || fileExtension == ".png" || fileExtension == ".jpg" || fileExtension == ".pdf" || fileExtension == ".doc" || fileExtension == ".jpeg")
                                {
                                    // Check if the file size is not more than 20MB
                                    if (grievanceModel.DocUpload.ContentLength <= 20971520)
                                    {
                                        string path = Path.Combine(Server.MapPath("~/Doc_Upload"), fileName);

                                        // Save the file
                                        grievanceModel.DocUpload.SaveAs(path);

                                        // Save the file path in the database
                                        filePaths[i] = Path.Combine("Doc_Upload", fileName);
                                        fileExtensions[i] = fileExtension;
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
                            }

                            // Save the file paths in the Tbl_Grievance table
                            tbl_Grievance.DocUpload = string.Join(",", filePaths);

                            // Save the file paths in the Tbl_Grievance_Documents table
                            for (int i = 0; i < filePaths.Length; i++)
                            {
                                Tbl_Grievance_Documents tbl_Grievance_Documents = new Tbl_Grievance_Documents
                                {
                                    GrievanceId = tbl_Grievance.Id,
                                    DocumentPath = filePaths[i],
                                    IsActive = true,
                                    CreatedOn = DateTime.Now
                                };
                                db.Tbl_Grievance_Documents.Add(tbl_Grievance_Documents);
                            }
                        }
                        db.Tbl_Grievance.Add(tbl_Grievance);
                        res = db.SaveChanges();
                    }
                    if (res > 0)
                    {
                        DataTable dt = new DataTable();
                        dt = SP_Model.GetTeamMailID();
                        if (dt.Rows.Count > 0) {

                            res = CommonModel.SendSucessfullMailForUser(dt.Rows[0]["EmailList"].ToString(),str.ToString(), partymail);
                            res = CommonModel.SendMailPartUser(partymail, Greid);                            
                        }
                        if (res > 0)
                        {
                            return Json(new { success = true, message = "Data saved and mail sended successfully!" });
                        }
                        else
                        {
                            return Json(new { success = true, message = "Data saved successfully!" });
                        }
                        
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

        public ActionResult RevertCList()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RevertCPost(FilterModel filterModel)
        {
            try
            {
                if (filterModel.GrievanceId_fk == Guid.Empty || filterModel.RevertTypeId == 0 && string.IsNullOrWhiteSpace(filterModel.TeamRevertMessage))
                {
                    return Json(new { success = false, message = "All fileds are required." });
                }
                using (var db = new Grievance_DBEntities())
                {
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
                    int res = db.SaveChanges();
                    if (res > 0)
                    {
                        DataTable dt = new DataTable();
                        dt = SP_Model.GetRevartMail(teamRevertComplain.GrievanceId_fk.ToString());
                        if (dt.Rows.Count > 0)
                        {
                            res = CommonModel.SendMailRevartPartUser(dt.Rows[0]["Email"].ToString(), dt.Rows[0]["GrievanceId_fk"].ToString(), dt.Rows[0]["Name"].ToString(), dt.Rows[0]["RevertStatus"].ToString());
                        }
                        if (res > 0)
                        {
                            return Json(new { success = true, message = "Data saved and mail sended successfully!" });
                        }
                        else
                        {
                            return Json(new { success = true, message = "Data saved successfully!" });
                        }
                    }
                    else
                    {
                        return Json(new { success = true, message = "Record not Submitted." });
                    }
                }
            } 
            catch (Exception ex)
            {

                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        public ActionResult RevartList()
        {
            return View();
        }
        public ActionResult GetRevartList(RevertComplaint RevertComplaint)
        {
            try
            {
                bool IsCheck = false;
                var dt = SP_Model.GetRevartList(RevertComplaint);
                if (dt.Rows.Count > 0)
                {
                    IsCheck = true;
                }
                var html = ConvertViewToString("_ComData", dt);
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



        [HttpPost]
        public ActionResult OPtSendMail(string EmailId, string OPTCode)
        {
            var res = -1;
            if (!string.IsNullOrWhiteSpace(EmailId) && string.IsNullOrWhiteSpace(OPTCode))
            {
                var vildemailid = EmailId.Trim().Split('@')[1];
                if (vildemailid.ToLower() == "pciglobal.in" || vildemailid.ToLower() == "gmail.com" || vildemailid.ToLower() == "projectconcernindia.org")
                {
                    res = CommonModel.SendMailForUser(EmailId);
                    if (res == 1)
                    {
                        return Json(new { success = true, message = "Please check the mail sent otp code.", resdata = 1 });
                    }
                    return Json(new { success = false, message = "EmailId not verify.", resdata = res });
                }
                else
                {
                    return Json(new { success = false, message = "EmailId Invalid.", resdata = "" });
                }
            }
            else if (!string.IsNullOrWhiteSpace(EmailId) && !string.IsNullOrWhiteSpace(OPTCode))
            {
                Grievance_DBEntities _db = new Grievance_DBEntities();
                var vildemailid = EmailId.Trim().Split('@')[1];
                if (vildemailid.ToLower() == "pciglobal.in" || vildemailid.ToLower() == "gmail.com" || vildemailid.ToLower() == "projectconcernindia.org")
                {
                    var tbl = _db.Tbl_LoginVerification.Where(x => x.EmailId.ToLower() == EmailId.Trim().ToLower() && x.IsActive == true && x.VerificationCode.ToLower() == OPTCode.ToLower().Trim())?.FirstOrDefault();// && x.Date == DateTime.Now.Date
                    if (tbl != null)
                    {
                        tbl.IsValidEmailId = true;
                        tbl.IsActive = false;
                        res = _db.SaveChanges();
                        if (res == 1)
                        {
                            return Json(new { success = true, message = "EmailId Verified.", resdata = 2 });
                        }

                    }
                    else
                    {
                        if (tbl != null)
                        {
                            tbl.IsValidEmailId = false;
                            tbl.IsActive = false;
                        }
                    }
                    res = _db.SaveChanges();
                    if (res == 1)
                    {
                        return Json(new { success = false, message = "EmailId Invalid.", resdata = 3 });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "EmailId Invalid.", resdata = "" });
                }
            }

            return Json(new { success = false, message = "EmailId Invalid.", resdata = "" });
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