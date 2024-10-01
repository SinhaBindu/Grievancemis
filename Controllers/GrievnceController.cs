using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Grievancemis.Models;
//using Grievancemis.Manager

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
        private void SendEmailToUser(string email, string code)
        {
            var fromMail = new MailAddress("your-email@gmail.com", "Your Name");
            var fromEmailpassword = "your-email-password";
            var toEmail = new MailAddress(email);
            var smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(fromMail.Address, fromEmailpassword);
            var Message = new MailMessage(fromMail, toEmail);
            Message.Subject = "Your Verification Code";
            Message.Body = "Your verification code is: " + code;
            Message.IsBodyHtml = true;
            smtpClient.Send(Message);
        }

        public ActionResult SendCode(string email)
        {
            try
            {
                // Generate a 6-digit random code
                string code = new string(Enumerable.Repeat("0123456789", 6).Select(s => s[new Random().Next(s.Length)]).ToArray());

                // Send the code to the user's email
                SendEmailToUser(email, code);

                // Store the code in the Tbl_LoginVerification table
                using (var db = new Grievance_DBEntities())
                {
                    Tbl_LoginVerification tbl_LoginVerification = new Tbl_LoginVerification
                    {
                        Email = email,
                        VerificationCode = code
                       
                    };
                    db.Tbl_LoginVerification.Add(tbl_LoginVerification);
                    db.SaveChanges();
                }

                // Return a success message
                return Json(new { success = true, message = "Verification code sent to your email" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error sending verification code: " + ex.Message });
            }
        }

        public ActionResult VerifyCode(string email, string code)
        {
            try
            {
                // Get the stored verification code from the Tbl_LoginVerification table
                using (var db = new Grievance_DBEntities())
                {
                    var storedCode = db.Tbl_LoginVerification.Where(x => x.Email == email).FirstOrDefault().VerificationCode;

                    // Compare the user-entered code with the stored code
                    if (code == storedCode)
                    {
                        // If the codes match, return a success message
                        return Json(new { success = true, message = "Verification code is correct" });
                    }
                    else
                    {
                        // If the codes do not match, return an error message
                        return Json(new { success = false, message = "Verification code is incorrect" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error verifying verification code: " + ex.Message });
            }
        }
        public ActionResult GetGrievances()
        {
            using (var db = new Grievance_DBEntities())
            {
                var grievances = db.Tbl_Grievance.ToList();
                var grievanceModels = grievances.Select(g => new GrivanceModel
                {
                    Id = g.Id,
                    Email = g.Email,
                    Name = g.Name,
                    PhoneNo = g.PhoneNo,
                    GrievanceType = g.GrievanceType.Value,
                    StateId = g.StateId.Value,
                    Location = g.Location,
                    Title = g.Title,
                    GrievanceMessage = g.Grievance_Message,
                    IsConsent = g.IsConsent.Value
                }).ToList();
                return PartialView("_GrievanceInput", grievanceModels);
            }
        }

    }
}