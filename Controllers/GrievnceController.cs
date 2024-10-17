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


                        //// Handle file upload
                        //if (grievanceModel.DocUpload != null && grievanceModel.DocUpload.ContentLength > 0)
                        //{
                        //    string fileName = Path.GetFileName(grievanceModel.DocUpload.FileName);
                        //    string fileExtension = Path.GetExtension(fileName).ToLower();

                        //    // Check if the file extension is allowed
                        //    if (fileExtension == ".zip" || fileExtension == ".png" || fileExtension == ".jpg" || fileExtension == ".pdf" || fileExtension == ".doc" || fileExtension == ".jpeg")
                        //    {
                        //        // Check if the file size is not more than 20MB
                        //        if (grievanceModel.DocUpload.ContentLength <= 20971520)
                        //        {
                        //            string path = Path.Combine(Server.MapPath("~/Doc_Upload"), fileName);

                        //            // Save the file
                        //            grievanceModel.DocUpload.SaveAs(path);

                        //            // Save the file path in the database
                        //            tbl_Grievance.DocUpload = Path.Combine("Doc_Upload", fileName);
                        //        }
                        //        else
                        //        {
                        //            return Json(new { success = false, message = "File size is too large. Only files up to 20MB are allowed." });
                        //        }
                        //    }
                        //    else
                        //    {
                        //        return Json(new { success = false, message = "Invalid file extension. Only zip, png, jpg, pdf, doc, and jpeg files are allowed." });
                        //    }
                        //}


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
        
        public ActionResult GrievanceList()
        {
            return View();
        }
        //public ActionResult GetGrievanceList(string stateFilter, string typeFilter)
        //{
        //    try
        //    {
        //        var items = SP_Model.GetGrievanceList(stateFilter, typeFilter);
        //        ViewBag.task = Task;
        //        if (items != null)
        //        {
        //            var data = JsonConvert.SerializeObject(items);
        //            var html = ConvertViewToString("_GrievanceData", items);
        //            return Json(new { IsSuccess = true, reshtml = html, res = data }, JsonRequestBehavior.AllowGet);
        //        }
        //        return Json(new { IsSuccess = false, res = "" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception)
        //    {
        //        return Json(new { IsSuccess = false, res = "There was a communication error." }, JsonRequestBehavior.AllowGet);
        //    }
        //}


        public ActionResult GetGrievanceList(string stateFilter, string typeFilter)
        {
            DataTable dt = SP_Model.GetGrievanceList(stateFilter, typeFilter);
            return PartialView("_GrievanceData", dt);
        }
    }
}