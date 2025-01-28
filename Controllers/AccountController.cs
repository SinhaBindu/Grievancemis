using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Grievancemis.Models;
using Grievancemis.Manager;
using Grievancemis.Helpers;
using System.Data.Entity;
using System.Security.Cryptography;
using Microsoft.Ajax.Utilities;

namespace Grievancemis.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            // return View();
            return RedirectToAction("GrievanceCaseAdd", "Grievnce");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }
        [AllowAnonymous]
        public async Task<ActionResult> LoginTeamUser(string EmailId)
        {
            LoginViewModel model = new LoginViewModel();
            if (Session["EmailId"] == null || string.IsNullOrWhiteSpace(Session["EmailId"].ToString()))
            {
                return RedirectToAction("GrievanceCaseAdd", "Grievnce");
            }
            else
            {
                EmailId= Session["EmailId"].ToString();
                model.Email = !string.IsNullOrWhiteSpace(EmailId) ? EmailId.Trim() : string.Empty;
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("GrievanceCaseAdd", "Grievnce");
                }
                var getpas = SP_Model.SP_AspnetUser(model.Email);
                if (getpas.Rows.Count > 0)
                    model.Password = getpas.Rows[0]["Passw"].ToString();
                else
                    return RedirectToAction("GrievanceCaseAdd", "Grievnce");

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                var result = await SignInManager.PasswordSignInAsync(EmailId.Trim(), model.Password, model.RememberMe, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        var user = MvcApplication.CUser;
                        return RedirectToAction("Index", "Report");
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    default:
                        return RedirectToAction("GrievanceCaseAdd", "Grievnce");
                        // ModelState.AddModelError("", "Invalid login attempt.");
                        // return View(model);
                }

            }
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetLoginTeamUser(string EmailId, string RId)
        {
            LoginViewModel model = new LoginViewModel();
            if (Session["EmailId"] == null || string.IsNullOrWhiteSpace(Session["EmailId"].ToString()))
            {
                return RedirectToAction("GrievanceCaseAdd", "Grievnce");
            }
            else
            {
                model.Email = !string.IsNullOrWhiteSpace(EmailId) ? EmailId.Trim() : string.Empty;
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("GrievanceCaseAdd", "Grievnce");
                }
                var getpas = SP_Model.SP_AspnetUser(model.Email);
                if (getpas.Rows.Count > 0)
                    model.Password = getpas.Rows[0]["Passw"].ToString();
                else
                    return RedirectToAction("GrievanceCaseAdd", "Grievnce");

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                var result = await SignInManager.PasswordSignInAsync(EmailId.Trim(), model.Password, model.RememberMe, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        var user = MvcApplication.CUser;
                        return RedirectToAction("Index", "Report");
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    default:
                        return RedirectToAction("GrievanceCaseAdd", "Grievnce");
                        // ModelState.AddModelError("", "Invalid login attempt.");
                        // return View(model);
                }

            }
           
        }


        //
        // // POST: /Account/Login
        // [HttpPost]
        // [AllowAnonymous]
        //// [ValidateAntiForgeryToken]
        // public async Task<ActionResult> Login_Grievance(string EmailId, string OPTCode)
        // {
        //     //if (!ModelState.IsValid)
        //     //{
        //     //    return View(model);
        //     //}

        //     //// This doesn't count login failures towards account lockout
        //     //// To enable password failures to trigger account lockout, change to shouldLockout: true
        //     //var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
        //     //switch (result)
        //     //{
        //     //    case SignInStatus.Success:
        //     //        return RedirectToLocal(returnUrl);
        //     //    case SignInStatus.LockedOut:
        //     //        return View("Lockout");
        //     //    case SignInStatus.RequiresVerification:
        //     //        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
        //     //    case SignInStatus.Failure:
        //     //    default:
        //     //        ModelState.AddModelError("", "Invalid login attempt.");
        //     //        return View(model);
        //     //}
        //     var res = -1;
        //     if (!string.IsNullOrWhiteSpace(EmailId) && string.IsNullOrWhiteSpace(OPTCode))//send OTP
        //     {
        //         var vildemailid = EmailId.Trim().Split('@')[1];
        //         if (vildemailid.ToLower() == "pciglobal.in" || vildemailid.ToLower() == "gmail.com" || vildemailid.ToLower() == "projectconcernindia.org")
        //         {
        //             var dt = SP_Model.GetOTPCheckLoginMail(EmailId.Trim(), OPTCode);
        //             res = CommonModel.SendMailForUser(EmailId.Trim(), dt);
        //             if (res == 1)
        //             {
        //                 return Json(new { success = true, message = "Please check the mail sent otp code.", resdata = 1 });
        //             }
        //             return Json(new { success = false, message = "EmailId not verify.", resdata = res });
        //         }
        //         else
        //         {
        //             return Json(new { success = false, message = "EmailId Invalid.", resdata = "" });
        //         }
        //     }
        //     else if (!string.IsNullOrWhiteSpace(EmailId) && !string.IsNullOrWhiteSpace(OPTCode))//EmailId Verified
        //     {
        //         var rolid = ""; var password = ""; var aspId = "";
        //         var aspdt = SP_Model.SP_AspnetUser(EmailId.Trim());
        //         Grievance_DBEntities _db = new Grievance_DBEntities();
        //         var vildemailid = EmailId.Trim().Split('@')[1];
        //         if (vildemailid.ToLower() == "pciglobal.in" || vildemailid.ToLower() == "gmail.com" || vildemailid.ToLower() == "projectconcernindia.org")
        //         {
        //             var tbl = _db.Tbl_LoginVerification.Where(x => x.EmailId.ToLower() == EmailId.Trim().ToLower() && x.IsActive == true && x.VerificationCode.ToLower() == OPTCode.ToLower().Trim() && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(DateTime.Now))?.FirstOrDefault();// && x.Date == DateTime.Now.Date
        //             if (tbl != null)
        //             {
        //                 tbl.IsValidEmailId = true;
        //                 //tbl.IsActive = false;
        //                 res = _db.SaveChanges();
        //                 if (aspdt.Rows.Count > 0)
        //                 {
        //                     rolid = aspdt.Rows[0]["RoleId"].ToString();
        //                     password = aspdt.Rows[0]["Passw"].ToString();
        //                     aspId = aspdt.Rows[0]["Id"].ToString();
        //                 }
        //                 else
        //                 {
        //                     RegisterViewModel model1 = new RegisterViewModel
        //                     {
        //                         Email = EmailId.Trim(),
        //                         Password = "User@123",
        //                         RoleName = "User",
        //                         RoleID = "2"
        //                     };
        //                     await RegisterCust(model1);
        //                     var aspdt1 = SP_Model.SP_AspnetUser(EmailId.Trim());
        //                     if (aspdt.Rows.Count > 0)
        //                     {
        //                         rolid = aspdt.Rows[0]["RoleId"].ToString();
        //                         password = aspdt.Rows[0]["Passw"].ToString();
        //                         aspId = aspdt.Rows[0]["Id"].ToString();
        //                     }
        //                 }

        //                 if (res == 1 || tbl.IsValidEmailId == true)
        //                 {
        //                     //return RedirectToAction("GetGrievanceList", "Complaine");
        //                     //return Json(new { success = true, message = "EmailId Verified.", resdata = 2 });
        //                     //var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
        //                     //switch (result)
        //                     //{

        //                     var signInResult = await SignInManager.PasswordSignInAsync(EmailId.Trim(), password, isPersistent: true, shouldLockout: false);

        //                     switch (signInResult)
        //                     {
        //                         case SignInStatus.Success:
        //                             res = 1;
        //                             // Retrieve the user’s current claims identity
        //                            // var identity = (ClaimsIdentity)User.Identity;

        //                             //// Remove the existing Name claim if it exists
        //                             //var nameClaim = identity.FindFirst(ClaimTypes.Name);
        //                             //if (nameClaim != null)
        //                             //{
        //                             //    identity.RemoveClaim(nameClaim);
        //                             //}

        //                             //// Add a new Name claim with the updated name
        //                             //identity.AddClaim(new Claim(ClaimTypes.Name, "NewUserNameHere"));

        //                             // Update the authentication cookie with the new claims
        //                             //await HttpContext.GetOwinContext().Authentication.SignIn(
        //                             //    new AuthenticationProperties { IsPersistent = true }, // Make cookie persistent if needed
        //                             //    identity // Use the updated identity directly
        //                             //);
        //                            // await SignInUser(identity, true);

        //                            // var g = identity.Name;
        //                             break;

        //                         case SignInStatus.LockedOut:
        //                         case SignInStatus.RequiresVerification:
        //                             res = 0;
        //                             break;
        //                     }
        //                     var f = User.Identity.Name;

        //                     Session["EmailId"] = EmailId.Trim();
        //                     var usercheck = MvcApplication.CUser;
        //                     if (usercheck != null && res > 0)
        //                     {
        //                         if (usercheck.RoleId == "2")//User
        //                         {
        //                             var getemail = SP_Model.SP_AspnetUserCaseFirstTimeCheck(EmailId, aspId);
        //                             if (getemail.Rows.Count > 0)
        //                             {
        //                                 return Json(new { success = true, message = "EmailId Verified.", redirect = "/UserCom/UserGList", resdata = 99, });
        //                             }
        //                             else
        //                                 return Json(new { success = true, message = "EmailId Verified.", resdata = 2, });
        //                         }
        //                         if (usercheck.RoleId == "3")//TeamMember
        //                         {
        //                             return Json(new { success = true, message = "EmailId Verified.", redirect = "/Complain/GrievanceList", resdata = 100 });
        //                         }
        //                         if (usercheck.RoleId == "1")//Admin
        //                         {
        //                             return Json(new { success = true, message = "EmailId Verified.", redirect = "/Complain/GrievanceList", resdata = 101 });
        //                         }
        //                     }
        //                 }
        //                 return Json(new { success = true, message = "EmailId Verified.", resdata = 2, });
        //             }
        //             else
        //             {
        //                 if (tbl != null)
        //                 {
        //                     tbl.IsValidEmailId = false;
        //                     //tbl.IsActive = false;
        //                 }
        //             }
        //             res = _db.SaveChanges();
        //             if (res == 1)
        //             {
        //                 return Json(new { success = false, message = "EmailId Invalid.", resdata = 3 });
        //             }
        //         }
        //         else
        //         {
        //             return Json(new { success = false, message = "EmailId Invalid.", resdata = "" });
        //         }
        //     }
        //     return Json(new { success = false, message = "EmailId Invalid.", resdata = "" });
        // }
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

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            Grievance_DBEntities db_ = new Grievance_DBEntities();
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(model.Id))
                {
                    model.Password = model.RoleID == "3" ? "Team@123" : model.RoleID == "2" ? "User@123" : model.RoleID == "4" ? "Head@123" : "Admin@1234";
                    //model.Password = !string.IsNullOrWhiteSpace(model.Password) ? model.Password : model.PhoneNumber.Trim();
                    var tbLu = db_.AspNetUsers.Find(model.Id);

                    var passwordHasher = new Microsoft.AspNet.Identity.PasswordHasher();
                    tbLu.PasswordHash = passwordHasher.HashPassword(model.Password);

                    tbLu.UserName = model.PhoneNumber.Trim();
                    tbLu.Name = model.Name.Trim();
                    tbLu.Email = model.Email.Trim();
                    //tbLu.UserName = model.UserName;
                    //tbLu.PasswordHash = model.Password;
                    int res = db_.SaveChanges();

                    var userRoles = UserManager.GetRoles(tbLu.Id);
                    var rolename = db_.AspNetRoles.Find(model.RoleID).Name;
                    foreach (var item in userRoles)
                    {
                        if (model.RoleID != item)
                        {
                            UserManager.RemoveFromRoles(tbLu.Id, item);
                            UserManager.AddToRole(tbLu.Id, rolename);
                        }
                    }
                    return RedirectToAction("UserDetaillist", "Master");
                }
                else
                {
                    var user = new ApplicationUser { PhoneNumber = model.PhoneNumber.Trim(), UserName = model.Email.Trim(), Email = model.Email.Trim() };
                    model.Password = model.RoleID == "3" ? "Team@123" : model.RoleID == "2" ? "User@123" : model.RoleID == "4" ? "Head@123" : "Admin@1234"; //!string.IsNullOrWhiteSpace(model.Password) ? model.Password : model.PhoneNumber;
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                        var rolename = db_.AspNetRoles.Find(model.RoleID).Name;
                        var result1 = UserManager.AddToRole(user.Id, rolename);
                        if (db_.AspNetUsers.Any(x => x.Id == user.Id.Trim()))
                        {
                            var tbLu = db_.AspNetUsers.Find(user.Id);
                            tbLu.Name = model.Name.Trim();
                            tbLu.CreatedBy = User.Identity.Name;
                            tbLu.CreatedOn = DateTime.Now;
                            int res = db_.SaveChanges();
                            UserManager.AddToRole(user.Id, rolename);
                        }
                        // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                        // return RedirectToAction("Index", "Home");
                        return RedirectToAction("UserDetaillist", "Master");
                    }
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);


            ////////////////////////////////////////////

            //if (ModelState.IsValid)
            //{
            //    var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            //    var result = await UserManager.CreateAsync(user, model.Password);
            //    if (result.Succeeded)
            //    {
            //        await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

            //        // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
            //        // Send an email with this link
            //        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            //        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            //        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

            //        return RedirectToAction("Index", "Home");
            //    }
            //    AddErrors(result);
            //}

            //// If we got this far, something failed, redisplay form
            //return View(model);
        }


        public ActionResult Register_Lock(RegisterViewModel model)
        {
            var lockoutEndDate = DateTime.Now.Date;
            UserManager.SetLockoutEndDate(model.Id, DateTimeOffset.Now.Date);
            UserManager.SetLockoutEnabled(model.Id, false);
            return RedirectToLocal("~/Master/UserDetaillist");
        }
        public ActionResult Register_Enable(RegisterViewModel model)
        {
            var lockoutEndDate = DateTime.Now.Date;
            UserManager.SetLockoutEndDate(model.Id, DateTimeOffset.Now.Date);
            UserManager.SetLockoutEnabled(model.Id, true);
            return RedirectToLocal("~/Master/UserDetaillist");
        }
        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("GrievanceCaseAdd", "Grievnce");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
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
        #endregion
    }
}