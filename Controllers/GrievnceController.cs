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
                DataTable dtcheck = SP_Model.GetSPCheckGrievanceAlready(grievanceModel.Email.Trim(), DateTime.Now.Date.ToDateTimeyyyyMMdd());
                if (dtcheck.Rows.Count > 0)
                {
                    return Json(new { success = false, message = "This record is already exists.....", resdata = 1 });
                }

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

                        var asp = db.AspNetUsers.Where(x => x.Email == grievanceModel.Email)?.FirstOrDefault();
                        asp.Name = grievanceModel.Name.Trim();
                        db.SaveChanges();
                        DataTable dt = new DataTable();
                        dt = SP_Model.GetTeamMailID();
                        if (dt.Rows.Count > 0)
                        {
                            res = CommonModel.SendSucessfullMailForUserTeam(dt.Rows[0]["EmailList"].ToString(), str.ToString(), partymail, Greid);
                            res = CommonModel.SendMailPartUser(partymail, Greid, grievanceModel.Name);
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
                                    res = 1;
                                    // Retrieve the user’s current claims identity
                                    var identity = (ClaimsIdentity)User.Identity;

                                    //// Remove the existing Name claim if it exists
                                    //var nameClaim = identity.FindFirst(ClaimTypes.Name);
                                    //if (nameClaim != null)
                                    //{
                                    //    identity.RemoveClaim(nameClaim);
                                    //}

                                    //// Add a new Name claim with the updated name
                                    //identity.AddClaim(new Claim(ClaimTypes.Name, "NewUserNameHere"));

                                    // Update the authentication cookie with the new claims
                                    //await HttpContext.GetOwinContext().Authentication.SignIn(
                                    //    new AuthenticationProperties { IsPersistent = true }, // Make cookie persistent if needed
                                    //    identity // Use the updated identity directly
                                    //);
                                    await SignInUser(identity, true);

                                    var g = identity.Name;
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
                                if (usercheck.RoleId == "2")//User
                                {
                                    var getemail = SP_Model.SP_AspnetUserCaseFirstTimeCheck(EmailId, aspId);
                                    if (getemail.Rows.Count > 0)
                                    {
                                        return Json(new { success = true, message = "EmailId Verified.", redirect = "/Report/Index", resdata = 99, });
                                    }
                                    else
                                        return Json(new { success = true, message = "EmailId Verified.", resdata = 2, });
                                }
                                if (usercheck.RoleId == "3")//TeamMember
                                {
                                    return Json(new { success = true, message = "EmailId Verified.", redirect = "/Report/Index", resdata = 100 });
                                }
                                if (usercheck.RoleId == "1")//Admin
                                {
                                    return Json(new { success = true, message = "EmailId Verified.", redirect = "/Report/Index", resdata = 101 });
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