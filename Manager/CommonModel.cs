using Grievancemis.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
//using SubSonic.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.Entity;
//using System.Data.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Grievancemis.Manager
{
    public class CommonModel
    {
        private static Grievance_DBEntities dbe = new Grievance_DBEntities();

        #region BaseUrl
        public static string GetBaseUrl()
        {
            var str = HttpContext.Current.Request.Url.Host;
            //return str;
            UrlHelper urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);

            string host = HttpContext.Current.Request.Url.Host;
            if (host == "localhost")
            {
                host = HttpContext.Current.Request.Url.Authority;
                return HttpContext.Current.Request.Url.Scheme + "://" + host;
            }
            //return urlHelper.Content("~/");
            return HttpContext.Current.Request.Url.Scheme + "://" + str;
        }
        public static string GetWebUrl()
        {
            return ConfigurationManager.AppSettings["WebUrl"];
        }

        public static bool IsEmailConfiguredToLive()
        {
            var isLive = Convert.ToBoolean(ConfigurationManager.AppSettings["IsEmailSetLive"].ToString());
            return isLive;
        }
        public static string GetLocalEmailAddress()
        {
            var emailAddr = ConfigurationManager.AppSettings["LocalEmailAddress"].ToString();
            return emailAddr;
        }

        public static string GetFileUrl(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
                return CommonModel.GetBaseUrl() + filePath.ToString().Replace("~", "");
            else
                return string.Empty;
        }

        public static string GetMultipleFileUrlFromComma(string filePaths)
        {
            //string filePath = string.Empty;
            //var filePathArray = filePaths.Split(',');
            List<string> filePathList = new List<string>();
            foreach (var path in filePaths.Split(','))
            {
                if (!string.IsNullOrEmpty(path))
                {
                    //return CommonModel.GetBaseUrl() + path.ToString().Replace("~", "");
                    filePathList.Add(CommonModel.GetBaseUrl() + path.Trim().ToString().Replace("~", ""));
                }
                //else
                //    return string.Empty;
            }
            //filePathList=.Join(',');
            return string.Join(",", filePathList);
        }

        public static string GetHeaderUSLogo(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
                return filePath.ToString().Replace("src=\"..//Content/images/USAID_Template.png\"", "src=\"" + CommonModel.GetWebUrl() + "/Content/images/USAID_Template.png\"");
            else
                return string.Empty;
        }
        public static string GetHeaderCareLogo(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
                return filePath.ToString().Replace("src=\"..//Content/images/Care_Template.png\"", "src=\"" + CommonModel.GetWebUrl() + "/Content/images/Care_Template.png\"");
            else
                return string.Empty;
        }
        #endregion

        //public static List<SelectListItem> GetGrievanceType()
        //{
        //    List<SelectListItem> list = new List<SelectListItem>();
        //    list.Add(new SelectListItem { Value = "0", Text = "Please Select" });
        //    list.Add(new SelectListItem { Value = "1", Text = "Harrassment" });
        //    list.Add(new SelectListItem { Value = "2", Text = "Unfair treatment" });
        //    list.Add(new SelectListItem { Value = "3", Text = "Major Dispute" });
        //    list.Add(new SelectListItem { Value = "4", Text = "Sexual Harrassment" });
        //    list.Add(new SelectListItem { Value = "5", Text = "Abuse" });
        //    list.Add(new SelectListItem { Value = "6", Text = "Policy Deviation" });
        //    list.Add(new SelectListItem { Value = "7", Text = "Discrimination" });
        //    list.Add(new SelectListItem { Value = "8", Text = "Unsafe Working Conditions" });
        //    return list;
        //}
        #region Master 
        public static List<SelectListItem> GetGrievanceType(bool IsAll = false)
        {
            Grievance_DBEntities db_ = new Grievance_DBEntities();
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                //if (HttpContext.Current.User.IsInRole(RoleNameCont.State) || HttpContext.Current.User.IsInRole(RoleNameCont.Admin))
                //{
                items = new SelectList(db_.M_GrievanceType.ToList(), "Id", "GrievanceType").OrderBy(x => x.Text).ToList();
                //}
                //else
                //{
                //    items = new SelectList(db_.AspNetRoles, "ID", "Name").OrderBy(x => x.Text).ToList();
                //}
                if (IsAll)
                {
                    items.Insert(0, new SelectListItem { Value = "", Text = "Select", Selected = true });
                }
                return items;
            }
            catch (Exception)
            {
                //DO To
                throw;
            }
        }
        public static List<SelectListItem> GetRevertType()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = "0", Text = "Please Select" });
            list.Add(new SelectListItem { Value = "1", Text = "In Progress" });
            list.Add(new SelectListItem { Value = "2", Text = "Closed" });
            return list.OrderByDescending(x => x.Text).ToList();
        }
        public static List<SelectListItem> GetRoleList(bool IsAll = false)
        {
            Grievance_DBEntities db_ = new Grievance_DBEntities();
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                //if (HttpContext.Current.User.IsInRole(RoleNameCont.State) || HttpContext.Current.User.IsInRole(RoleNameCont.Admin))
                //{
                items = new SelectList(db_.AspNetRoles.ToList(), "ID", "Name").OrderBy(x => x.Text).ToList();
                //}
                //else
                //{
                //    items = new SelectList(db_.AspNetRoles, "ID", "Name").OrderBy(x => x.Text).ToList();
                //}
                if (IsAll)
                {
                    items.Insert(0, new SelectListItem { Value = "", Text = "Select", Selected = true });
                }
                return items;
            }
            catch (Exception)
            {
                //DO To
                throw;
            }
        }
        public static List<SelectListItem> GetState(bool IsAll = false)
        {
            Grievance_DBEntities _db = new Grievance_DBEntities();
            try
            {
                var items = new SelectList(_db.m_State_Master, "LGD_State_Code", "StateName").OrderBy(x => x.Text).ToList();
                if (IsAll)
                {
                    items.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                }
                items.Insert(0, new SelectListItem { Value = "", Text = "Please select" }); // Add "Please select" item
                return items;
            }
            catch (Exception)
            {
                //DO To
                throw;
            }
        }
        #endregion

        public static int SendMailForUser(string Toemailid)
        {
            Grievance_DBEntities _db = new Grievance_DBEntities();
            int noofsend = 0;
            string To = "", Subject = "", Body = "", ReceiverName = "Hi,"
                , SenderName = "", RandomValue = "", OTPCode = "";
            string ASDT = ""; string DurationTime = ""; string BatchName = "";
            string TrainerName = ""; string DistrictAgencyTrainingCenter = "";
            string OtherEmailID = "sinhabinduk@gmail.com"; string maxdateExam = ""; string maxdateExamTimeStartEnd = "";
            Grievance_DBEntities db_ = new Grievance_DBEntities();
            string bodydata = string.Empty;
            string bodyTemplate = string.Empty;
            Guid AssessmentScheduleId_pk = Guid.Empty;
            Guid ParticipantId = Guid.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/Shared/MailTemplate.html")))
            {
                bodyTemplate = reader.ReadToEnd();
            }
            try
            {
                Random random = new Random();

                // Generate a random double between 0.0 and 1.0
                int randomNumber = random.Next(0, 1011455); // Generates a number between 0.0 and 1.0

                var tblget = new Tbl_LoginVerification();//_db.Tbl_LoginVerification.Where(x => x.EmailId == Toemailid.Trim()).OrderByDescending(x=>x.CreatedOn)?.FirstOrDefault();
                var tbl_v = tblget != null ? tblget : new Tbl_LoginVerification();
                tbl_v.Id = Guid.NewGuid();
                tbl_v.EmailId = Toemailid.Trim();
                tbl_v.VerificationCode = randomNumber.ToString();
                tbl_v.CreatedOn = DateTime.Now;
                tbl_v.StartTime = DateTime.Now.TimeOfDay;
                tbl_v.EndTime = tbl_v.StartTime.Value.Add(new TimeSpan(1, 0, 0));
                tbl_v.Date = DateTime.Now.Date;
                tbl_v.IsActive = true;
                tbl_v.IsValidEmailId = false;
                _db.Tbl_LoginVerification.Add(tbl_v);
                _db.SaveChanges();

                To = Toemailid;

                bodydata = bodyTemplate.Replace("{Dearusername}", ReceiverName)
                    .Replace("{bodytext}", "When the user submits the code for verification, check if the code was generated within the last hour. If the current time exceeds the one-hour limit, the OTP is invalid.")
                    .Replace("{EmailID}", To)
                    .Replace("{OTPCode}", tbl_v.VerificationCode);
                //using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/Shared/MailTemplate.html")))
                //{
                //    bodyTemplate = reader.ReadToEnd();
                //}
                //bodyTemplate = "<table width=\"100%\" border=\"0\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\">\r\n\t\t<tbody>\r\n <tr>\r\n\t\t\t<td align=\"center\"> " + bodydata + "\r\n\t\t\t\t\r\n  \t</tbody></tr>\r\n</table>";
                MailMessage mail = new MailMessage();
                //mail.To.Add("bindu@careindia.org");
                mail.To.Add(To + "," + OtherEmailID);
                mail.From = new MailAddress("kgbvjh4care@gmail.com", "Grievance Query");
                //mail.From = new MailAddress("hunarmis2024@gmail.com");
                mail.Subject = Subject + " ( Grievance : ) ";// + " ( " + SenderName + " )";

                //bodydata = bodyTemplate.Replace("{bodytext}", Body);
                mail.Body = bodydata;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                //smtp.Credentials = new System.Net.NetworkCredential("hunarmis2024@gmail.com", "Hunar@2024");//Pasw-Care@321 // Enter seders User name and password       
                //smtp.Credentials = new System.Net.NetworkCredential("careindiabtsp@gmail.com", "gupczsbvzinhivzw");//Pasw-Care@321 // Enter seders User name and password       
                smtp.Credentials = new System.Net.NetworkCredential("kgbvjh4care@gmail.com", "yklzeazktmknvcbu");// yklz eazk tmkn vcbu//Pasw-Care@321 // Enter seders User name and password       
                smtp.EnableSsl = true;
                smtp.Send(mail);
                tbl_v.Issent = true;
                tbl_v.SentOn = DateTime.Now;
                _db.SaveChanges();
                return 1;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public static int SendSucessfullMailForUser(string Toemailid,string bodytext,string partymail)
        {
            Grievance_DBEntities _db = new Grievance_DBEntities();
            int noofsend = 0;
            string To = "", Subject = "", Body = "", ReceiverName = "Hi,"
                , SenderName = "", RandomValue = "", OTPCode = "";
            string ASDT = ""; string DurationTime = ""; string BatchName = "";
            string TrainerName = ""; string DistrictAgencyTrainingCenter = "";
            string OtherEmailID = "sinhabinduk@gmail.com"; string maxdateExam = ""; string maxdateExamTimeStartEnd = "";
            Grievance_DBEntities db_ = new Grievance_DBEntities();
            string bodydata = string.Empty;
            string bodyTemplate = string.Empty;
            Guid AssessmentScheduleId_pk = Guid.Empty;
            Guid ParticipantId = Guid.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/Shared/PanalMailTemplate.html")))
            {
                bodyTemplate = reader.ReadToEnd();
            }
            try
            {
                Random random = new Random();

                // Generate a random double between 0.0 and 1.0
                int randomNumber = random.Next(0, 1011455); // Generates a number between 0.0 and 1.0

                var tblget = new Tbl_LoginVerification();//_db.Tbl_LoginVerification.Where(x => x.EmailId == Toemailid.Trim()).OrderByDescending(x=>x.CreatedOn)?.FirstOrDefault();
                var tbl_v = tblget != null ? tblget : new Tbl_LoginVerification();
                tbl_v.Id = Guid.NewGuid();
                tbl_v.EmailId = Toemailid.Trim();
                tbl_v.VerificationCode = randomNumber.ToString();
                tbl_v.CreatedOn = DateTime.Now;
                tbl_v.StartTime = DateTime.Now.TimeOfDay;
                tbl_v.EndTime = tbl_v.StartTime.Value.Add(new TimeSpan(1, 0, 0));
                tbl_v.Date = DateTime.Now.Date;
                tbl_v.IsActive = true;
                tbl_v.IsValidEmailId = false;
                _db.Tbl_LoginVerification.Add(tbl_v);
                _db.SaveChanges();

                To = Toemailid;

                bodydata = bodyTemplate.Replace("{Dearusername}", ReceiverName)
                    .Replace("{bodytext}", bodytext)
                    .Replace("{EmailID}", To);
                    //.Replace("{OTPCode}", tbl_v.VerificationCode);
                //using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/Shared/MailTemplate.html")))
                //{
                //    bodyTemplate = reader.ReadToEnd();
                //}
                //bodyTemplate = "<table width=\"100%\" border=\"0\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\">\r\n\t\t<tbody>\r\n <tr>\r\n\t\t\t<td align=\"center\"> " + bodydata + "\r\n\t\t\t\t\r\n  \t</tbody></tr>\r\n</table>";
                MailMessage mail = new MailMessage();
                //mail.To.Add("bindu@careindia.org");
                mail.To.Add(To + "," + OtherEmailID+","+ partymail);
                mail.From = new MailAddress("kgbvjh4care@gmail.com", "Grievance Query");
                //mail.From = new MailAddress("hunarmis2024@gmail.com");
                mail.Subject = Subject + " ( Grievance : ) ";// + " ( " + SenderName + " )";

                //bodydata = bodyTemplate.Replace("{bodytext}", Body);
                mail.Body = bodydata;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                //smtp.Credentials = new System.Net.NetworkCredential("hunarmis2024@gmail.com", "Hunar@2024");//Pasw-Care@321 // Enter seders User name and password       
                //smtp.Credentials = new System.Net.NetworkCredential("careindiabtsp@gmail.com", "gupczsbvzinhivzw");//Pasw-Care@321 // Enter seders User name and password       
                smtp.Credentials = new System.Net.NetworkCredential("kgbvjh4care@gmail.com", "yklzeazktmknvcbu");// yklz eazk tmkn vcbu//Pasw-Care@321 // Enter seders User name and password       
                smtp.EnableSsl = true;
                smtp.Send(mail);
                tbl_v.Issent = true;
                tbl_v.SentOn = DateTime.Now;
                _db.SaveChanges();
                return 1;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public static int SendMailPartUser(string Toemailid,string gvid)
        {
            Grievance_DBEntities _db = new Grievance_DBEntities();
            int noofsend = 0;
            string To = "", Subject = "", Body = "", ReceiverName = "Hi,"
                , SenderName = "", RandomValue = "", OTPCode = "";
            string ASDT = ""; string DurationTime = ""; string BatchName = "";
            string TrainerName = ""; string DistrictAgencyTrainingCenter = "";
            string OtherEmailID = "sinhaharshit829@gmail.com"; string maxdateExam = ""; string maxdateExamTimeStartEnd = "";
            Grievance_DBEntities db_ = new Grievance_DBEntities();
            string bodydata = string.Empty;
            string bodyTemplate = string.Empty;
            Guid AssessmentScheduleId_pk = Guid.Empty;
            Guid ParticipantId = Guid.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/Shared/UserTemplate.html")))
            {
                bodyTemplate = reader.ReadToEnd();
            }
            try
            {
                Random random = new Random();

                // Generate a random double between 0.0 and 1.0
                int randomNumber = random.Next(0, 1011455); // Generates a number between 0.0 and 1.0

                var tblget = new Tbl_LoginVerification();//_db.Tbl_LoginVerification.Where(x => x.EmailId == Toemailid.Trim()).OrderByDescending(x=>x.CreatedOn)?.FirstOrDefault();
                var tbl_v = tblget != null ? tblget : new Tbl_LoginVerification();
                tbl_v.Id = Guid.NewGuid();
                tbl_v.EmailId = Toemailid.Trim();
                tbl_v.VerificationCode = randomNumber.ToString();
                tbl_v.CreatedOn = DateTime.Now;
                tbl_v.StartTime = DateTime.Now.TimeOfDay;
                tbl_v.EndTime = tbl_v.StartTime.Value.Add(new TimeSpan(1, 0, 0));
                tbl_v.Date = DateTime.Now.Date;
                tbl_v.IsActive = true;
                tbl_v.IsValidEmailId = false;
                _db.Tbl_LoginVerification.Add(tbl_v);
                _db.SaveChanges();

                To = Toemailid;

                bodydata = bodyTemplate.Replace("{Dearusername}", ReceiverName)
                    .Replace("{bodytext}", "Thank's For Your Co-Operation. Your Grievance has been sucessfully Registered with Us.We'll Reach to You as soon as possible.")
                    .Replace("{EmailID}", To)
                    .Replace("{OTPCode}", gvid);
                //using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/Shared/MailTemplate.html")))
                //{
                //    bodyTemplate = reader.ReadToEnd();
                //}
                //bodyTemplate = "<table width=\"100%\" border=\"0\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\">\r\n\t\t<tbody>\r\n <tr>\r\n\t\t\t<td align=\"center\"> " + bodydata + "\r\n\t\t\t\t\r\n  \t</tbody></tr>\r\n</table>";
                MailMessage mail = new MailMessage();
                //mail.To.Add("bindu@careindia.org");
                mail.To.Add(To + "," + OtherEmailID);
                mail.From = new MailAddress("kgbvjh4care@gmail.com", "Grievance Query");
                //mail.From = new MailAddress("hunarmis2024@gmail.com");
                mail.Subject = Subject + " ( Grievance : ) ";// + " ( " + SenderName + " )";

                //bodydata = bodyTemplate.Replace("{bodytext}", Body);
                mail.Body = bodydata;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                //smtp.Credentials = new System.Net.NetworkCredential("hunarmis2024@gmail.com", "Hunar@2024");//Pasw-Care@321 // Enter seders User name and password       
                //smtp.Credentials = new System.Net.NetworkCredential("careindiabtsp@gmail.com", "gupczsbvzinhivzw");//Pasw-Care@321 // Enter seders User name and password       
                smtp.Credentials = new System.Net.NetworkCredential("kgbvjh4care@gmail.com", "yklzeazktmknvcbu");// yklz eazk tmkn vcbu//Pasw-Care@321 // Enter seders User name and password       
                smtp.EnableSsl = true;
                smtp.Send(mail);
                tbl_v.Issent = true;
                tbl_v.SentOn = DateTime.Now;
                _db.SaveChanges();
                return 1;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static int SendMailRevartPartUser(string Toemailid, string gvid,string name,string status)
        {
            Grievance_DBEntities _db = new Grievance_DBEntities();
            int noofsend = 0;
            string To = "", Subject = "", Body = "", ReceiverName = "Hi,"
                , SenderName = "", RandomValue = "", OTPCode = "";
            string ASDT = ""; string DurationTime = ""; string BatchName = "";
            string TrainerName = ""; string DistrictAgencyTrainingCenter = "";
            string OtherEmailID = "sinhaharshit829@gmail.com"; string maxdateExam = ""; string maxdateExamTimeStartEnd = "";
            Grievance_DBEntities db_ = new Grievance_DBEntities();
            string bodydata = string.Empty;
            string bodyTemplate = string.Empty;
            Guid AssessmentScheduleId_pk = Guid.Empty;
            Guid ParticipantId = Guid.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/Shared/RevartMail.html")))
            {
                bodyTemplate = reader.ReadToEnd();
            }
            try
            {
                Random random = new Random();

                // Generate a random double between 0.0 and 1.0
                int randomNumber = random.Next(0, 1011455); // Generates a number between 0.0 and 1.0

                var tblget = new Tbl_LoginVerification();//_db.Tbl_LoginVerification.Where(x => x.EmailId == Toemailid.Trim()).OrderByDescending(x=>x.CreatedOn)?.FirstOrDefault();
                var tbl_v = tblget != null ? tblget : new Tbl_LoginVerification();
                tbl_v.Id = Guid.NewGuid();
                tbl_v.EmailId = Toemailid.Trim();
                tbl_v.VerificationCode = randomNumber.ToString();
                tbl_v.CreatedOn = DateTime.Now;
                tbl_v.StartTime = DateTime.Now.TimeOfDay;
                tbl_v.EndTime = tbl_v.StartTime.Value.Add(new TimeSpan(1, 0, 0));
                tbl_v.Date = DateTime.Now.Date;
                tbl_v.IsActive = true;
                tbl_v.IsValidEmailId = false;
                _db.Tbl_LoginVerification.Add(tbl_v);
                _db.SaveChanges();

                To = Toemailid;

                bodydata = bodyTemplate.Replace("{Dearusername}", ReceiverName)
                    .Replace("{bodytext}", name +", Your Grievance Status has been Changed To "+ status + ".We'll Inform You on next Update.")
                    .Replace("{EmailID}", To)
                    .Replace("{OTPCode}", gvid)
                    .Replace("{Status}", status);
                //using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/Shared/MailTemplate.html")))
                //{
                //    bodyTemplate = reader.ReadToEnd();
                //}
                //bodyTemplate = "<table width=\"100%\" border=\"0\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\">\r\n\t\t<tbody>\r\n <tr>\r\n\t\t\t<td align=\"center\"> " + bodydata + "\r\n\t\t\t\t\r\n  \t</tbody></tr>\r\n</table>";
                MailMessage mail = new MailMessage();
                //mail.To.Add("bindu@careindia.org");
                mail.To.Add(To + "," + OtherEmailID);
                mail.From = new MailAddress("kgbvjh4care@gmail.com", "Grievance Query");
                //mail.From = new MailAddress("hunarmis2024@gmail.com");
                mail.Subject = Subject + " ( Grievance : ) ";// + " ( " + SenderName + " )";

                //bodydata = bodyTemplate.Replace("{bodytext}", Body);
                mail.Body = bodydata;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                //smtp.Credentials = new System.Net.NetworkCredential("hunarmis2024@gmail.com", "Hunar@2024");//Pasw-Care@321 // Enter seders User name and password       
                //smtp.Credentials = new System.Net.NetworkCredential("careindiabtsp@gmail.com", "gupczsbvzinhivzw");//Pasw-Care@321 // Enter seders User name and password       
                smtp.Credentials = new System.Net.NetworkCredential("kgbvjh4care@gmail.com", "yklzeazktmknvcbu");// yklz eazk tmkn vcbu//Pasw-Care@321 // Enter seders User name and password       
                smtp.EnableSsl = true;
                smtp.Send(mail);
                tbl_v.Issent = true;
                tbl_v.SentOn = DateTime.Now;
                _db.SaveChanges();
                return 1;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}