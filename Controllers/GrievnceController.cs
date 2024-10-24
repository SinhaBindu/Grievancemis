﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime.Tree;
using Grievancemis.Manager;
using Grievancemis.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;

namespace Grievancemis.Controllers
{
    public class GrievnceController : Controller
    {
        private Grievance_DBEntities db = new Grievance_DBEntities();
        // GET: Grievnce

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        // private RoleManager _userManager;
        public GrievnceController()
        {
        }

        public GrievnceController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GrievanceCaseAdd()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GrievanceCaseAdd(GrivanceModel grievanceModel)
        {
            try
            {
                int res = 0; System.Text.StringBuilder str = new System.Text.StringBuilder(); string partymail = string.Empty, Greid = string.Empty;
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
                        var igtype = 0; var istype = 0;
                        igtype = Convert.ToInt32(tbl_Grievance.GrievanceType);
                        istype = Convert.ToInt32(tbl_Grievance.StateId);
                        var GType = db.M_GrievanceType.Where(b => b.Id == igtype)?.FirstOrDefault();
                        var SType = db.m_State_Master.Where(b => b.LGD_State_Code == istype)?.FirstOrDefault();
                        str.Append("<table border='1'>");
                        str.Append("<tr><td>Email</td><td>Name</td><td>Phone Number</td></tr>");
                        str.Append("<tr><td>" + tbl_Grievance.Email + "</td><td>" + tbl_Grievance.Name + "</td><td>" + tbl_Grievance.PhoneNo + "</td></tr>");
                        str.Append("<tr><td>Grievance Type</td><td>State Name</td><td>Title</td></tr>");
                        str.Append("<tr><td>" + GType.GrievanceType + "</td><td>" + SType.StateName + "</td><td>" + tbl_Grievance.Title + "</td></tr>");
                        str.Append("<tr><td>Location</td><td colspan='2'>Message</td></tr>");
                        str.Append("<tr><td>" + tbl_Grievance.Location + "</td><td colspan='2'>" + tbl_Grievance.Grievance_Message + "</td></tr>");
                        str.Append("</table>");
                        partymail = tbl_Grievance.Email.Trim();
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
                        if (dt.Rows.Count > 0)
                        {

                            res = CommonModel.SendSucessfullMailForUserTeam(dt.Rows[0]["EmailList"].ToString(), str.ToString(), partymail);
                            res = CommonModel.SendMailPartUser(partymail, Greid, grievanceModel.Name);
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

        [HttpPost]
        public async Task<ActionResult> OPtSendMail(string EmailId, string OPTCode)
        {
            var res = -1;
            if (!string.IsNullOrWhiteSpace(EmailId) && string.IsNullOrWhiteSpace(OPTCode))//send OTP
            {
                var vildemailid = EmailId.Trim().Split('@')[1];
                if (vildemailid.ToLower() == "pciglobal.in" || vildemailid.ToLower() == "gmail.com" || vildemailid.ToLower() == "projectconcernindia.org")
                {
                    var dt = SP_Model.GetOTPCheckLoginMail(EmailId.Trim(), OPTCode);
                    res = CommonModel.SendMailForUser(EmailId.Trim(), dt);
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
            else if (!string.IsNullOrWhiteSpace(EmailId) && !string.IsNullOrWhiteSpace(OPTCode))//EmailId Verified
            {
                var rolid = ""; var password = ""; var aspId = "";
                var aspdt = SP_Model.SP_AspnetUser(EmailId.Trim());
                Grievance_DBEntities _db = new Grievance_DBEntities();
                var vildemailid = EmailId.Trim().Split('@')[1];
                if (vildemailid.ToLower() == "pciglobal.in" || vildemailid.ToLower() == "gmail.com" || vildemailid.ToLower() == "projectconcernindia.org")
                {
                    var tbl = _db.Tbl_LoginVerification.Where(x => x.EmailId.ToLower() == EmailId.Trim().ToLower() && x.IsActive == true && x.VerificationCode.ToLower() == OPTCode.ToLower().Trim() && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(DateTime.Now))?.FirstOrDefault();// && x.Date == DateTime.Now.Date
                    if (tbl != null)
                    {
                        tbl.IsValidEmailId = true;
                        //tbl.IsActive = false;
                        res = _db.SaveChanges();
                        if (aspdt.Rows.Count > 0)
                        {
                            rolid = aspdt.Rows[0]["RoleId"].ToString();
                            password = aspdt.Rows[0]["Passw"].ToString();
                            aspId = aspdt.Rows[0]["Id"].ToString();
                        }
                        else
                        {
                            RegisterViewModel model = new RegisterViewModel
                            {
                                Email = EmailId.Trim(),
                                Password = "User@123",
                                RoleName = "User",
                                RoleID = "2"
                            };
                            await RegisterCust(model);
                            var aspdt1 = SP_Model.SP_AspnetUser(EmailId.Trim());
                            if (aspdt.Rows.Count > 0)
                            {
                                rolid = aspdt.Rows[0]["RoleId"].ToString();
                                password = aspdt.Rows[0]["Passw"].ToString();
                                aspId = aspdt.Rows[0]["Id"].ToString();
                            }
                        }

                        if (res == 1 || tbl.IsValidEmailId == true)
                        {
                            //return RedirectToAction("GetGrievanceList", "Complaine");
                            //return Json(new { success = true, message = "EmailId Verified.", resdata = 2 });

                            var signInResult = await SignInManager.PasswordSignInAsync(EmailId, password, isPersistent: true, shouldLockout: false);

                            switch (signInResult)
                            {
                                case SignInStatus.Success:
                                    res = 1; break;
                                case SignInStatus.LockedOut:
                                case SignInStatus.RequiresVerification:
                                    res = 0; break;
                            }

                            Session["EmailId"] = EmailId.Trim();
                            var usercheck = MvcApplication.CUser;
                            if (usercheck != null && res > 0)
                            {
                                if (usercheck.RoleId == "2")//User
                                {
                                    var getemail = SP_Model.SP_AspnetUserCaseFirstTimeCheck(EmailId, aspId);
                                    if (getemail.Rows.Count > 0)
                                    {
                                        return Json(new { success = true, message = "EmailId Verified.", redirect = "/UserCom/UserGList", resdata = 99, });
                                    }
                                    else
                                        return Json(new { success = true, message = "EmailId Verified.", resdata = 2, });
                                }
                                if (usercheck.RoleId == "3")//TeamMember
                                {
                                    return Json(new { success = true, message = "EmailId Verified.", redirect = "/Complain/GrievanceList", resdata = 100 });
                                }
                                if (usercheck.RoleId == "1")//Admin
                                {
                                    return Json(new { success = true, message = "EmailId Verified.", redirect = "/Complain/GrievanceList", resdata = 101 });
                                }
                            }
                        }
                        return Json(new { success = true, message = "EmailId Verified.", resdata = 2, });
                    }
                    else
                    {
                        if (tbl != null)
                        {
                            tbl.IsValidEmailId = false;
                            //tbl.IsActive = false;
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

        public async Task<string> RegisterCust(RegisterViewModel model)
        {
            Grievance_DBEntities db_ = new Grievance_DBEntities();

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email.Trim(), Email = model.Email.Trim() };//PhoneNumber = model.PhoneNumber.Trim(),
                model.Password = !string.IsNullOrWhiteSpace(model.Password) ? model.Password : model.PhoneNumber;
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    var rolename = db_.AspNetRoles.Find(model.RoleID).Name;
                    var result1 = UserManager.AddToRole(user.Id, rolename);
                    if (db_.AspNetUsers.Any(x => x.Id == user.Id.Trim()))
                    {
                        var tbLu = db_.AspNetUsers.Find(user.Id);
                        tbLu.Name = model.Email.Trim();
                        tbLu.CreatedBy = User.Identity.Name;
                        tbLu.CreatedOn = DateTime.Now;
                        int res = db_.SaveChanges();
                        UserManager.AddToRole(user.Id, rolename);
                    }

                    // Return success (1) if user creation and role assignment succeeded
                    return "1";
                }

                // Return failure (2) if user creation failed
                return "2";
            }

            // Return (0) if model state is invalid
            return "0";
        }

    }
}