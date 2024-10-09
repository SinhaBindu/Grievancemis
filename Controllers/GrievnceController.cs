using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime.Tree;
using Grievancemis.Manager;
using Grievancemis.Models;

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
                if (vildemailid.ToLower() == "pciglobal.in" || vildemailid.ToLower() == "gmail.com")
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
                if (vildemailid.ToLower() == "pciglobal.in" || vildemailid.ToLower() == "gmail.com")
                {
                    var tbl = _db.Tbl_LoginVerification.Where(x => x.EmailId.ToLower() == EmailId.Trim().ToLower() && x.IsActive == true && x.VerificationCode.ToLower() == OPTCode.ToLower().Trim())?.FirstOrDefault();// && x.Date == DateTime.Now.Date
                    if (tbl != null)
                    {
                        tbl.IsValidEmailId = true;
                        tbl.IsActive = false;
                        res = _db.SaveChanges();
                        if (res == 1)
                        {
                            return Json(new { success = true, message = "EmailId Invalid.", resdata = 2 });
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
        public ActionResult GrievanceForm()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GrievanceForm(GrivanceModel grievanceModel)
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
                            Email = grievanceModel.Email.Trim(),
                            Name = grievanceModel.Name.Trim(),
                            PhoneNo = grievanceModel.PhoneNo,
                            GrievanceType = grievanceModel.GrievanceType,
                            StateId = grievanceModel.StateId,
                            Location = grievanceModel.Location.Trim(),
                            Title = grievanceModel.Title.Trim(),
                            Grievance_Message = grievanceModel.GrievanceMessage.Trim(),
                            IsConsent = grievanceModel.IsConsent,
                            IsActive = true,
                            CreatedOn = DateTime.Now
                        };
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
        //private void SendEmailToUser(string email, string code)
        //{
        //    var fromMail = new MailAddress("your-email@gmail.com", "Your Name");
        //    var fromEmailpassword = "your-email-password";
        //    var toEmail = new MailAddress(email);
        //    var smtpClient = new SmtpClient();
        //    smtpClient.Host = "smtp.gmail.com";
        //    smtpClient.Port = 587;
        //    smtpClient.EnableSsl = true;
        //    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        //    smtpClient.UseDefaultCredentials = false;
        //    smtpClient.Credentials = new NetworkCredential(fromMail.Address, fromEmailpassword);
        //    var Message = new MailMessage(fromMail, toEmail);
        //    Message.Subject = "Your Verification Code";
        //    Message.Body = "Your verification code is: " + code;
        //    Message.IsBodyHtml = true;
        //    smtpClient.Send(Message);
        //}

        //public ActionResult SendCode(string email)
        //{
        //    try
        //    {
        //        // Generate a 6-digit random code
        //        string code = new string(Enumerable.Repeat("0123456789", 6).Select(s => s[new Random().Next(s.Length)]).ToArray());

        //        // Send the code to the user's email
        //        SendEmailToUser(email, code);

        //        // Store the code in the Tbl_LoginVerification table
        //        using (var db = new Grievance_DBEntities())
        //        {
        //            Tbl_LoginVerification tbl_LoginVerification = new Tbl_LoginVerification
        //            {
        //                Email = email,
        //                VerificationCode = code

        //            };
        //            db.Tbl_LoginVerification.Add(tbl_LoginVerification);
        //            db.SaveChanges();
        //        }

        //        // Return a success message
        //        return Json(new { success = true, message = "Verification code sent to your email" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "Error sending verification code: " + ex.Message });
        //    }
        //}

        //public ActionResult VerifyCode(string email, string code)
        //{
        //    try
        //    {
        //        // Get the stored verification code from the Tbl_LoginVerification table
        //        using (var db = new Grievance_DBEntities())
        //        {
        //            var storedCode = db.Tbl_LoginVerification.Where(x => x.Email == email).FirstOrDefault().VerificationCode;

        //            // Compare the user-entered code with the stored code
        //            if (code == storedCode)
        //            {
        //                // If the codes match, return a success message
        //                return Json(new { success = true, message = "Verification code is correct" });
        //            }
        //            else
        //            {
        //                // If the codes do not match, return an error message
        //                return Json(new { success = false, message = "Verification code is incorrect" });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "Error verifying verification code: " + ex.Message });
        //    }
        //}
        //public ActionResult GetGrievances()
        //{
        //    DataTable dt = SP_Model.GetGrievanceList();
        //    return PartialView("_GrievanceInput", dt);
        //}
        public ActionResult GetGrievances(string stateFilter, string typeFilter)
        {
            DataTable dt = SP_Model.GetGrievancefilterList(stateFilter, typeFilter);
            return PartialView("_GrievanceInput", dt);
        }
    }
}