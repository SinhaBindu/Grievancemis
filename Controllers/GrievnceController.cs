using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime.Tree;
using Grievancemis.Helpers;
using Grievancemis.Manager;
using Grievancemis.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Newtonsoft.Json;
using static Grievancemis.Manager.CommonModel;

namespace Grievancemis.Controllers
{

    public class GrievnceController : Controller
    {
        private Grievance_DBEntities db = new Grievance_DBEntities();
        // GET: Grievnce

        private const string XsrfKey = "XsrfId";

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
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GrievanceCaseAdd()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return View();
        }
        [HttpPost]
        public ActionResult GrievanceCaseAdd(GrivanceModel grievanceModel)
        {
            try
            {
                int res = 0; System.Text.StringBuilder str = new System.Text.StringBuilder(); string partymail = string.Empty, Greid = string.Empty, stGuid = string.Empty;
                if (ModelState.IsValid)
                {
                    DataTable dtcheck = SP_Model.GetSPCheckGrievanceAlready(grievanceModel.Email.Trim(), DateTime.Now.Date.ToDateTimeyyyyMMdd());
                    if (dtcheck.Rows.Count > 0)
                    {
                        return Json(new { success = false, message = "This record is already exists.....", resdata = 1 });
                    }
                    using (var db = new Grievance_DBEntities())
                    {

                        Tbl_Grievance tbl_Grievance = new Tbl_Grievance
                        {
                            Id = Guid.NewGuid(),
                            Email = grievanceModel.Email,
                            Name = grievanceModel.Name,
                            PhoneNo = grievanceModel.PhoneNo,
                            Gender = grievanceModel.Gender,
                            GrievanceType = grievanceModel.GrievanceType,
                            StateId = grievanceModel.StateId,
                            //Location = grievanceModel.Location,
                            Other = grievanceModel.Other,
                            Title = grievanceModel.Title,
                            ComplainRegDate = DateTime.Now.Date,
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
                        //str.Append("<table border='1'>");
                        //str.Append("<tr><td>Gender</td><td>" + tbl_Grievance.Gender + "</td></tr>");
                        //str.Append("<tr><td>Email</td><td>Name</td><td>Phone Number</td></tr>");
                        //str.Append("<tr><td>" + tbl_Grievance.Email + "</td><td>" + tbl_Grievance.Name + "</td><td>" + tbl_Grievance.PhoneNo + "</td></tr>");
                        //str.Append("<tr><td>Grievance Type</td><td>State Name</td><td>Title</td></tr>");
                        //str.Append("<tr><td>" + GType.GrievanceType + "</td><td>" + SType.StateName + "</td><td>" + tbl_Grievance.Title + "</td></tr>");
                        //str.Append("<tr><td>Other</td><td colspan='2'>Message</td></tr>");
                        //str.Append("<tr><td>" + tbl_Grievance.Other + "</td><td colspan='2'>" + tbl_Grievance.Grievance_Message + "</td></tr>");
                        //str.Append("</table>");
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
                            Session["EmailId"] = grievanceModel.Email.Trim();
                            res = CommonModel.SendSucessfullMailForUserTeam(dt.Rows[0]["EmailList"].ToString(), str.ToString(), partymail, Greid, CaseOfStatus.NewCase);
                            res = CommonModel.SendMailPartUser(partymail, Greid, grievanceModel.Name, CaseOfStatus.NewCase);
                        }
                        if (res > 0)
                        {
                            return Json(new { success = true, message = "Your request is registered with Grievance reference id : " + Greid + "<br />" + "Mail sended successfully!", res = 1 });//Data saved and mail sended successfully!
                        }
                        else
                        {
                            return Json(new { success = true, message = "Your request is registered with Grievance reference id :" + Greid, res = 1 });//Data saved successfully!
                        }

                    }
                    else
                    {
                        return Json(new { success = false, message = "Failed to save data. Please try again." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Invalid input. Please check your form data, All Fileds requied." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        [HttpPost]
        public ActionResult OTPSendEmailMail(string EmailId)
        {
            Grievance_DBEntities _db = new Grievance_DBEntities();
            try
            {
                if (string.IsNullOrWhiteSpace(EmailId))
                {
                    return Json(new { success = false, message = "EmailId Empty.", resdata = "" });
                }
                else
                {
                    var vildemailid = EmailId.Trim().Split('@')[1];
                    if (!(vildemailid.ToLower() == "pciglobal.in" || vildemailid.ToLower() == "gmail.com" || vildemailid.ToLower() == "projectconcernindia.org"))
                    {
                        return Json(new { success = false, message = Enums.GetEnumDescription(Enums.eReturnReg.EmailNotValid), resdata = "" });
                    }
                    var res = -1;
                    var dt = SP_Model.Usp_SendInsertandUpdate(EmailId.Trim());
                    res = CommonModel.SendMailForUser(EmailId.Trim(), dt);
                    TempData["SendStatus"] = res;
                    TempData["SendMailFstOTP"] = dt;
                    if (res == 1 || (res == -1 || res == 0))
                    {
                        return Json(new { success = true, message = Enums.GetEnumDescription(Enums.eReturnReg.EmailidSentOTPCode), resdata = 1 });
                    }
                    return Json(new { success = false, message = Enums.GetEnumDescription(Enums.eReturnReg.EmailidNotverify), resdata = res });
                }
            }
            catch (Exception ex)
            {
                CommonModel.ExpSubmit("Tbl_LoginVerification", "Grievnce", "OTPSendEmailMail", EmailId, ex.Message);
                return Json(new { success = false, message = Enums.GetEnumDescription(Enums.eReturnReg.ExceptionError), resdata = "" });
            }
        }
        [HttpPost]
        public ActionResult ResendOTPSendMail(string EmailId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EmailId))
                {
                    return Json(new { success = false, message = "EmailId Empty.", resdata = "" });
                }
                else
                {
                    var vildemailid = EmailId.Trim().Split('@')[1];
                    if (!(vildemailid.ToLower() == "pciglobal.in" || vildemailid.ToLower() == "gmail.com" || vildemailid.ToLower() == "projectconcernindia.org"))
                    {
                        return Json(new { success = false, message = Enums.GetEnumDescription(Enums.eReturnReg.EmailNotValid), resdata = "" });
                    }
                    Grievance_DBEntities _db = new Grievance_DBEntities();
                    var tbl = new Tbl_LoginVerification();
                    var res = -1;
                    var SentRawDatadt = (DataTable)TempData["SendMailFstOTP"];
                    if (SentRawDatadt.Rows.Count > 0)
                    {
                        res = CommonModel.SendMailForUser(EmailId.Trim(), SentRawDatadt);
                        tbl = _db.Tbl_LoginVerification.Find(Guid.Parse(SentRawDatadt.Rows[0]["Id"].ToString()));
                    }
                    else
                    {
                        var dt = SP_Model.Usp_SendInsertandUpdate(EmailId.Trim());
                        res = CommonModel.SendMailForUser(EmailId.Trim(), dt);
                        tbl = _db.Tbl_LoginVerification.Find(Guid.Parse(SentRawDatadt.Rows[0]["Id"].ToString()));
                    }
                    if (res == 1 || (res == -1 || res == 0))
                    {
                        if (tbl != null)
                        {
                            tbl.Resend = true;
                            tbl.ResendDt = DateTime.Now;
                            _db.SaveChanges();
                        }
                        return Json(new { success = true, message = Enums.GetEnumDescription(Enums.eReturnReg.EmailidSentOTPCode), resdata = 1 });
                    }
                    return Json(new { success = false, message = Enums.GetEnumDescription(Enums.eReturnReg.EmailidNotverify), resdata = res });
                }
            }
            catch (Exception ex)
            {
                CommonModel.ExpSubmit("Tbl_LoginVerification", "Grievnce", "ResendOTPSendMail", EmailId, ex.Message);
                return Json(new { success = false, message = Enums.GetEnumDescription(Enums.eReturnReg.ExceptionError), resdata = "" });
            }
        }

        [HttpPost]
        public async Task<ActionResult> OTPVerifyEmailMail(string EmailId, string OPTCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EmailId))
                {
                    return Json(new { success = false, message = "EmailId Empty.", resdata = "" });
                }
                if (string.IsNullOrWhiteSpace(EmailId) && string.IsNullOrWhiteSpace(OPTCode))
                {
                    return Json(new { success = false, message = "EmailId and OTP Empty.", resdata = "" });
                }
                else
                {
                    var vildemailid = EmailId.Trim().Split('@')[1];
                    if (!(vildemailid.ToLower() == "pciglobal.in" || vildemailid.ToLower() == "gmail.com" || vildemailid.ToLower() == "projectconcernindia.org"))
                    {
                        return Json(new { success = false, message = "Email Invalid.", resdata = "" });
                    }
                }
                if (!string.IsNullOrWhiteSpace(EmailId) && !string.IsNullOrWhiteSpace(OPTCode))//EmailId Verified
                {
                    var res = 0;
                    var dtvalidotp = SP_Model.Usp_OTPValid(EmailId.Trim(), OPTCode.Trim());
                    var rolid = ""; var password = ""; var aspId = "";
                    Grievance_DBEntities _db = new Grievance_DBEntities();
                    if (dtvalidotp.Rows.Count > 0)
                    {
                        //working now
                        var aspdt = SP_Model.SP_AspnetUser(EmailId.Trim());
                        var tbl = _db.Tbl_LoginVerification.Find(Guid.Parse(dtvalidotp.Rows[0]["ID"].ToString()));// && x.Date == DateTime.Now.Date
                        if (tbl != null)
                        {
                            if (dtvalidotp.Rows[0]["OTPStatus"].ToString() == "1")
                            {
                                tbl.IsValidEmailId = true;
                                res = _db.SaveChanges();
                                //return Json(new { success = false, message = Enums.GetEnumDescription(Enums.eReturnReg.ValidOTP), resdata = 2, });
                            }
                            else if (dtvalidotp.Rows[0]["OTPStatus"].ToString() == "2")
                            {
                                return Json(new { success = false, message = Enums.GetEnumDescription(Enums.eReturnReg.OTPExpired), resdata = "", });
                            }
                            else
                            {
                                return Json(new { success = false, message = Enums.GetEnumDescription(Enums.eReturnReg.InvalidOTP), resdata = "", });
                            }
                        }
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
                                RoleName = RolesNamecont.User,
                                RoleID = RolesIdcont.User
                            };
                            string regres = await RegisterCust(model);
                            if (regres == "1")
                            {
                                var aspdt1 = SP_Model.SP_AspnetUser(EmailId.Trim());
                                if (aspdt1.Rows.Count > 0)
                                {
                                    rolid = aspdt1.Rows[0]["RoleId"].ToString();
                                    password = aspdt1.Rows[0]["Passw"].ToString();
                                    aspId = aspdt1.Rows[0]["Id"].ToString();
                                }
                            }
                            else
                            {
                                return Json(new { success = false, message = "User is not registered", resdata = "" });
                            }
                        }
                        if (res == 1 || tbl.IsValidEmailId == true)
                        {
                            password = !string.IsNullOrWhiteSpace(password) ? password : "User@123";
                            var signInResult = await SignInManager.PasswordSignInAsync(EmailId, password, isPersistent: true, shouldLockout: false);

                            switch (signInResult)
                            {
                                case SignInStatus.Success:
                                    res = 1;
                                    // Retrieve the user’s current claims identity
                                    var identity = (ClaimsIdentity)User.Identity;
                                    await SignInUser(identity, true);
                                    break;
                                case SignInStatus.LockedOut:
                                case SignInStatus.RequiresVerification:
                                    res = 0;
                                    break;
                            }
                            var f = User.Identity.Name;
                            Session["CUser"] = null;
                            Session["EmailId"] = EmailId.Trim();
                            var usercheck = MvcApplication.CUser;

                            if (usercheck != null && res > 0)
                            {
                                if (usercheck.RoleId == RolesIdcont.User)//User-2
                                {
                                    Grievance_DBEntities _DBEntities = new Grievance_DBEntities();

                                    var getemail = SP_Model.SP_AspnetUserCaseFirstTimeCheck(EmailId, aspId);
                                    if (getemail.Rows.Count > 0)
                                    {
                                        return Json(new { success = true, message = "EmailId Verified.", redirect = "/Report/Index", resdata = 99, });
                                    }
                                    else
                                        return Json(new { success = true, message = "EmailId Verified.", resdata = 2, });

                                }
                                if (usercheck.RoleId == RolesIdcont.Community)//TeamMember-3
                                {
                                    return Json(new { success = true, message = "EmailId Verified.", redirect = "/Report/Index", resdata = 100 });
                                }
                                if (usercheck.RoleId == RolesIdcont.Admin || usercheck.RoleId == RolesIdcont.Head)//Admin-1
                                {
                                    return Json(new { success = true, message = "EmailId Verified.", redirect = "/Report/Index", resdata = 101 });
                                }
                            }
                        }
                    }
                }
                return Json(new { success = false, message = "EmailId Empty or Invalid.", resdata = "" });
            }
            catch (Exception ex)
            {
                CommonModel.ExpSubmit("Tbl_LoginVerification", "Grievnce", "OTPVerifyEmailMail", EmailId, ex.Message);
                return Json(new { success = false, message = "A communication error has occurred.", resdata = "" });
            }
        }

        public async Task SignInUser(ClaimsIdentity identity, bool isPersistent)
        {
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = isPersistent
            };

            // Perform the sign-in operation with the specified identity and properties
            HttpContext.GetOwinContext().Authentication.SignIn(authProperties, identity);
        }
        public async Task<string> RegisterCust(RegisterViewModel model)
        {
            Grievance_DBEntities db_ = new Grievance_DBEntities();

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email.Trim(), Email = model.Email.Trim() };//PhoneNumber = model.PhoneNumber.Trim(),
                model.Password = !string.IsNullOrWhiteSpace(model.Password) ? model.Password : "User@123";
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


        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

    }
}