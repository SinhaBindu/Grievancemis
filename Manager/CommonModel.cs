using Grievancemis.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using SubSonic.Schema;
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
using static Grievancemis.Manager.CommonModel;

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
                    //items.Insert(0, new SelectListItem { Value = "", Text = "Select", Selected = true });
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
        public static List<SelectListItem> GetRevertType(int IsAll = 0)
        {
            Grievance_DBEntities db_ = new Grievance_DBEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            if (IsAll == 1)
                list.Add(new SelectListItem { Value = "0", Text = "All" });
            else if (IsAll == 0)
            {
                list.Add(new SelectListItem { Value = "0", Text = "Select" });
            }
            list = new SelectList(db_.M_RevertType.Where(x => x.IsActive == true && x.Id != 99).ToList(), "Id", "RevertType").OrderBy(x => x.Text).ToList();
            return list.OrderBy(x => Convert.ToInt32(x.Value)).ToList(); // Sort according to the custom order

            //List<SelectListItem> list = new List<SelectListItem>();
            //list.Add(new SelectListItem { Value = "0", Text = "Please Select" });
            //list.Add(new SelectListItem { Value = "1", Text = "In Process" });
            //list.Add(new SelectListItem { Value = "2", Text = "Redressed" });
            //// Sort items based on a custom order
            //var order = new List<string> { "0", "1", "2" }; // Define the desired order by Value
            //return list.OrderBy(x => order.IndexOf(x.Value)).ToList(); // Sort according to the custom order
        }
        public static List<SelectListItem> GetUserRevertType(int IsAll = 0)
        {

            Grievance_DBEntities db_ = new Grievance_DBEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            if (IsAll == 1)
                list.Add(new SelectListItem { Value = "0", Text = "All" });
            else if (IsAll == 0)
            {
                list.Add(new SelectListItem { Value = "0", Text = "Select" });
            }
            list = new SelectList(db_.M_RevertType.Where(x => x.IsActive == true && x.Id == 99).ToList(), "Id", "RevertType").OrderBy(x => x.Text).ToList();
            //  var order = new List<string> { "0", "99" }; // Define the desired order by Value
            return list.OrderBy(x => Convert.ToInt32(x.Value)).ToList(); // Sort according to the custom order

            //List<SelectListItem> list = new List<SelectListItem>();
            //list.Add(new SelectListItem { Value = "0", Text = "Please Select" });
            ////list.Add(new SelectListItem { Value = "1", Text = "Clarification" });
            //list.Add(new SelectListItem { Value = "99", Text = "Closed" });
            //// Sort items based on a custom order
            //var order = new List<string> { "0", "99" }; // Define the desired order by Value
            //return list.OrderBy(x => order.IndexOf(x.Value)).ToList(); // Sort according to the custom order
        }
        public static List<SelectListItem> GetMasterRevertType(int IsAll = 0)
        {
            Grievance_DBEntities db_ = new Grievance_DBEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            if (IsAll == 1)
                list.Add(new SelectListItem { Value = "0", Text = "All" });
            else if (IsAll == 0)
            {
                list.Add(new SelectListItem { Value = "0", Text = "Select" });
            }
            list = new SelectList(db_.M_RevertType.Where(x => x.IsActive == true).ToList(), "Id", "RevertType").OrderBy(x => x.Text).ToList();
            //  var order = new List<string> { "0", "99" }; // Define the desired order by Value
            return list.OrderBy(x => Convert.ToInt32(x.Value)).ToList(); // Sort according to the custom order
        }
        //public static List<SelectListItem> GetGenderType()
        //{
        //    List<SelectListItem> list = new List<SelectListItem>();
        //    list.Add(new SelectListItem { Value = "1", Text = "Male" });
        //    list.Add(new SelectListItem { Value = "2", Text = "Female" });
        //    list.Add(new SelectListItem { Value = "3", Text = "Other's" });

        //    // Sort items based on a custom order
        //    var order = new List<string> { "1", "2", "3" }; // Define the desired order by Value
        //    return list.OrderBy(x => order.IndexOf(x.Value)).ToList(); // Sort according to the custom order
        //}
        //public static List<SelectListItem> GetRevertType()
        //{
        //    List<SelectListItem> list = new List<SelectListItem>();
        //    list.Add(new SelectListItem { Value = "0", Text = "Please Select" });
        //    list.Add(new SelectListItem { Value = "1", Text = "Clarification" });
        //    list.Add(new SelectListItem { Value = "2", Text = "Closed" });
        //    return list.OrderByDescending(x => x.Text).ToList();
        //}
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
        public static List<SelectListItem> GetpanelList(bool includeSelectOption = false)
        {
            StoredProcedure sp = new StoredProcedure("GetUsersByRole");
            DataTable dt = sp.ExecuteDataSet().Tables[0];

            // Convert DataTable to a list of SelectListItem
            List<SelectListItem> roleList = new List<SelectListItem>();

            if (includeSelectOption)
            {
                roleList.Add(new SelectListItem { Text = "--Select--", Value = "" });
            }

            foreach (DataRow row in dt.Rows)
            {
                roleList.Add(new SelectListItem
                {
                    Text = row["Name"].ToString(),
                    Value = row["Id"].ToString()
                });
            }

            return roleList;
        }
        //public static SelectList GetpanelList(bool includeSelectOption = false)
        //{
        //    StoredProcedure sp = new StoredProcedure("GetUsersByRole");
        //    DataTable dt = sp.ExecuteDataSet().Tables[0];

        //    // Convert DataTable to a list of SelectListItem
        //    List<SelectListItem> roleList = new List<SelectListItem>();

        //    if (includeSelectOption)
        //    {
        //        roleList.Add(new SelectListItem { Text = "--Select--", Value = "" });
        //    }

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        roleList.Add(new SelectListItem
        //        {
        //            Text = row["Name"].ToString(),
        //            Value = row["Id"].ToString()
        //        });
        //    }

        //    return new SelectList(roleList, "Value", "Text");
        //}
        #endregion
        public static bool ValidateImageSizeDocoument(HttpPostedFileBase file)
        {
            byte[] image = new byte[file.ContentLength];
            file.InputStream.Read(image, 0, image.Length);
            // Convert MB to bytes (1 MB = 1024 * 1024 bytes)
            //int maxSizeInBytes = 5242880;//maxMB * 1024 * 1024;
            int maxSizeInBytes = 20971520;//maxMB * 1024 * 1024;/20mb

            // Check if the image size is less than or equal to the specified limit
            if (image.Length <= maxSizeInBytes)
            {
                return true; // Valid size
            }

            return false; // Invalid size
        }
        public static FileModel saveFile(HttpPostedFileBase item, string Fldpath, string FileName)
        {
            FileModel fileModel = new FileModel();
            if (ValidateImageSizeDocoument(item))
            {
                string URL = "";
                string filepath = string.Empty;
                if (item != null && item.ContentLength > 0)
                {
                    if (Fldpath != URL)
                    {
                        URL = Fldpath;
                    }
                    URL = "/Doc_Upload/" + Fldpath + "/";
                    string folderPath = HttpContext.Current.Server.MapPath("~" + URL);

                    var supportedTypes = new[] { "pdf", "xls", "xlsx", "jpeg", "png", "jpg" };

                    var fileName = Path.GetFileName(item.FileName);
                    // var rondom = Guid.NewGuid() + fileName;

                    // var fileExt = System.IO.Path.GetExtension(rondom).Substring(1).ToLower();

                    //if (!supportedTypes.Contains(fileExt.ToLower()))
                    //{
                    //   // Danger("File Extension Is InValid - Upload Only PDF/EXCEL/JPEG/PNG/JPG File");
                    //   // return RedirectToAction("VendorDetails", new { id = d.guid });
                    //}

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // string Document = Path.Combine("~/Uploads/VendorDoc/" + rondom);

                    item.SaveAs(folderPath + fileName);
                    filepath = URL + fileName;
                    fileModel.FolderPath = URL;
                }
                fileModel.IsvalidFile = true;
                fileModel.FilePathFull = filepath;
                return fileModel;
            }
            else
            {
                fileModel.IsvalidFile = false;
                return fileModel;
            }

        }

        public class FileModel
        {
            public string FilePathFull { get; set; }
            public string FolderPath { get; set; }
            public bool IsvalidFile { get; set; }
        }
        #region Error Expestion
        public static void ExpSubmit(string Table, string Controller, string Action, string Method, string ExMessage)
        {
            Grievance_DBEntities gdb = new Grievance_DBEntities();
            Tbl_ExceptionHandle tblexp = new Tbl_ExceptionHandle();
            tblexp.Id_pk = Guid.NewGuid();
            tblexp.Table = Table;
            tblexp.Controller = Controller;
            tblexp.Action = Action;
            tblexp.Method = Method;
            tblexp.E_Exception = ExMessage;
            tblexp.IsActive = true;
            tblexp.CreatedBy = MvcApplication.CUser!=null? MvcApplication.CUser.UserId:null;
            tblexp.CreatedOn = DateTime.Now;
            gdb.Tbl_ExceptionHandle.Add(tblexp);
            gdb.SaveChanges();
        }
        #endregion

        //OTP mail method
        public static int SendMailForUser(string Toemailid, DataTable dt)
        {
            Grievance_DBEntities _db = new Grievance_DBEntities();
            //int noofsend = 0;
            string To = "", Subject = "", Body = "", ReceiverName = "Hi ", maxID = "", RandomValue = "", OTPCode = "";
            string OtherEmailID = "sinhabinduk@gmail.com,sinhaharshit829@gmail.com";
            Grievance_DBEntities db_ = new Grievance_DBEntities();
            string bodydata = string.Empty;
            string bodyTemplate = string.Empty;
            var tblget = new Tbl_LoginVerification();//_db.Tbl_LoginVerification.Where(x => x.EmailId == Toemailid.Trim()).OrderByDescending(x=>x.CreatedOn)?.FirstOrDefault();
            var tbl_v = tblget != null ? tblget : new Tbl_LoginVerification();
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/Shared/MailTemplate.html")))
            {
                bodyTemplate = reader.ReadToEnd();
            }
            try
            {
                if (dt.Rows.Count > 0)
                {
                    RandomValue = dt.Rows[0]["VerificationCode"].ToString();
                    maxID = dt.Rows[0]["IntId"].ToString();
                    tblget.Id=Guid.Parse(dt.Rows[0]["ID"].ToString());
                }
                else
                {
                    Random random = new Random();
                    var stspantime = DateTime.Now.TimeOfDay;
                    var st = new TimeSpan(stspantime.Hours, stspantime.Minutes, stspantime.Seconds);
                    var endtimespan = st.Add(new TimeSpan(1,0 , 0));
                    var et= new TimeSpan(endtimespan.Hours, endtimespan.Minutes, endtimespan.Seconds);
                    // Generate a random double between 0.0 and 1.0
                    int randomNumber = random.Next(0, 1011455462); // Generates a number between 0.0 and 1.0
                    tbl_v.Id = Guid.NewGuid();
                    tbl_v.EmailId = Toemailid.Trim();
                    tbl_v.VerificationCode = randomNumber.ToString();
                    tbl_v.CreatedOn = DateTime.Now;
                    //tbl_v.StartTime = st;//DateTime.Now.TimeOfDay;
                    //tbl_v.EndTime = et;// tbl_v.StartTime.Value.Add(new TimeSpan(1, 0, 0));
                    tbl_v.Date = DateTime.Now.Date;
                    tbl_v.IsActive = true;
                    tbl_v.IsValidEmailId = false;
                    tbl_v.Resend = false;
                    tbl_v.CreatedAt = DateTime.Now;
                    tbl_v.ExpiryAt = DateTime.Now.AddHours(1);
                    _db.Tbl_LoginVerification.Add(tbl_v);
                    _db.SaveChanges();
                    RandomValue = randomNumber.ToString();
                }
                To = Toemailid;
                bodydata = bodyTemplate.Replace("{bodytext}", "Enter the above OTP Code to log in to the grievance portal. OTP would be valid for next 1 hour.")
                    //.Replace("{EmailID}", To)
                    .Replace("{OTPCode}", RandomValue);
                MailMessage mail = new MailMessage();
                //mail.To.Add("bindu@careindia.org");
                mail.To.Add(To + "," + OtherEmailID);
                mail.From = new MailAddress("pci4tech@gmail.com", "Grievance Query");
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
                //smtp.Credentials = new System.Net.NetworkCredential("kgbvjh4care@gmail.com", "yklzeazktmknvcbu");// yklz eazk tmkn vcbu//Pasw-Care@321 // Enter seders User name and password  - App passwords google     
                smtp.Credentials = new System.Net.NetworkCredential("pci4tech@gmail.com", "tylodouomqitatre");// yklz eazk tmkn vcbu//Pasw-Care@321 // Enter seders User name and password   - App passwords google
                smtp.EnableSsl = true;
                smtp.Send(mail);
                if (dt.Rows.Count > 0)
                {
                    var tbl_vup = _db.Tbl_LoginVerification.Find(tblget.Id);
                    tbl_vup.Issent = true;
                    tbl_vup.SentOn = DateTime.Now;
                    _db.SaveChanges();
                }
                else
                {
                    if (tbl_v != null)
                    {
                        if (tbl_v.Id != Guid.Empty)
                        {
                            var tbl_vup = _db.Tbl_LoginVerification.Find(tbl_v.Id);
                            tbl_vup.Issent = true;
                            tbl_vup.SentOn = DateTime.Now;
                            _db.SaveChanges();
                        }
                    }

                }
                return 1;

            }
            catch (Exception ex)
            {
                CommonModel.ExpSubmit("Tbl_LoginVerification" , "Grievnce", "SendMailForUser_OTP", "SendMailForUser_commonmodel", ex.Message+ "_"+Toemailid);
                return 0;

            }
        }// end OTP
        public static int SendSucessfullMailForUserTeam(string Toemailid, string bodytext, string partymail, string Greid, string CurrentStatus)
        {
            Grievance_DBEntities _db = new Grievance_DBEntities();
            int noofsend = 0;
            string To = "", Subject = "", Body = "", ReceiverName = "Dear Panel Member"
                , SenderName = "", RandomValue = "", OTPCode = "";
            string OtherEmailID = "sinhabinduk@gmail.com,sinhaharshit829@gmail.com";
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
                string[] tokens = Toemailid.Split(',');
                To = tokens[0];
                //Body = "The grievance <b> Case ID : "+ Greid + "</ b> Status is " + CurrentStatus + ". </ br> Login in to grievance portal <b> Web Link : <a href=" + CommonModel.GetBaseUrl() +" ></a> </b> for details.";
                Body = "The grievance <b> Case ID : " + Greid + "</ b> Status is " + CurrentStatus + ". </ br> <b>Visit : <a href=" + CommonModel.GetBaseUrl() + " style='font-size:medium !important;'>Click Here</a> </b> for details.";

                bodydata = bodyTemplate.Replace("{Dearusername}", ReceiverName)
                    //.Replace("{bodytext}", bodytext)
                    .Replace("{bodytext}", Body);
                // .Replace("{CaseID}", Greid);
                //.Replace("{OTPCode}", tbl_v.VerificationCode);

                MailMessage mail = new MailMessage();
                //mail.To.Add("bindu@careindia.org");
                //mail.To.Add(To + "," + OtherEmailID + "," + partymail);
                string stcc = string.Empty;
                for (int i = 1; i < tokens.Length; i++)
                {
                    stcc = tokens[i] + ",";
                }
                stcc = stcc.Substring(0, stcc.Length - 1);
                mail.To.Add(To);
                //mail.CC.Add(stcc);
                mail.Bcc.Add(stcc + "," + OtherEmailID);
                mail.From = new MailAddress("pci4tech@gmail.com", "Grievance Query");
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
                //smtp.Credentials = new System.Net.NetworkCredential("kgbvjh4care@gmail.com", "yklzeazktmknvcbu");// yklz eazk tmkn vcbu//Pasw-Care@321 // Enter seders User name and password       
                smtp.Credentials = new System.Net.NetworkCredential("pci4tech@gmail.com", "tylodouomqitatre");// yklz eazk tmkn vcbu//Pasw-Care@321 // Enter seders User name and password       
                smtp.EnableSsl = true;
                smtp.Send(mail);
                var tblu = _db.Tbl_Grievance.Where(x => x.CaseId == Convert.ToInt32(Greid))?.FirstOrDefault();
                tblu.Issent = true;
                tblu.UpdatedOn = DateTime.Now;
                db_.SaveChanges();

                _db.SaveChanges();
                return 1;

            }
            catch (Exception ex)
            {
                CommonModel.ExpSubmit("Tbl_Grievance", "Grievnce", "PanalMailTemplate", "SendSucessfullMailForUserTeam_commonmodel", ex.Message + "_" + Toemailid);
                return 0;
            }
        }
        public static int SendMailPartUser(string Toemailid, string gvid, string Name, string CurrentStatus)
        {
            Grievance_DBEntities _db = new Grievance_DBEntities();
            int noofsend = 0;
            string To = "", Subject = "", Body = "", ReceiverName = "Dear "
                , SenderName = "", RandomValue = "", OTPCode = "";
            string OtherEmailID = "sinhabinduk@gmail.com,sinhaharshit829@gmail.com";
            Grievance_DBEntities db_ = new Grievance_DBEntities();
            string bodydata = string.Empty;
            string bodyTemplate = string.Empty;
            Guid AssessmentScheduleId_pk = Guid.Empty;
            Guid ParticipantId = Guid.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/Shared/UserTemplateMail.html")))
            {
                bodyTemplate = reader.ReadToEnd();
            }
            try
            {
                To = Toemailid;
                Body = "The grievance <b> Case ID : " + gvid + "</ b> Status is " + CurrentStatus + ". </ br> <b>Visit : <a href=" + CommonModel.GetBaseUrl() + " style='font-size:medium !important;'>Click Here</a> </b> for details.";

                bodydata = bodyTemplate.Replace("{Dearusername}", ReceiverName + Name)
                    .Replace("{bodytext}", Body)
                    //.Replace("{bodytext}", "Thank's For Your Co-Operation. Your Grievance has been sucessfully Registered with Us.We'll Reach out to You as soon as possible.")
                    .Replace("{EmailID}", To)
                    .Replace("{newusername}", Name);
                // .Replace("{gvid}", gvid);
                MailMessage mail = new MailMessage();
                //mail.To.Add("bindu@careindia.org");
                mail.To.Add(To);
                mail.Bcc.Add(OtherEmailID);
                mail.From = new MailAddress("pci4tech@gmail.com", "Grievance Query");
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
                //smtp.Credentials = new System.Net.NetworkCredential("kgbvjh4care@gmail.com", "yklzeazktmknvcbu");// yklz eazk tmkn vcbu//Pasw-Care@321 // Enter seders User name and password       
                smtp.Credentials = new System.Net.NetworkCredential("pci4tech@gmail.com", "tylodouomqitatre");// yklz eazk tmkn vcbu//Pasw-Care@321 // Enter seders User name and password       
                smtp.EnableSsl = true;
                smtp.Send(mail);

                //var tblu = _db.Tbl_Grievance.Where(x => x.CaseId == Convert.ToInt32(gvid))?.FirstOrDefault();
                //tblu.Issent = true;
                //tblu.UpdatedOn = DateTime.Now;
                db_.SaveChanges();
                return 1;

            }
            catch (Exception ex)
            {
                CommonModel.ExpSubmit("Tbl_Grievance", "Grievnce", "UserTemplateMail", "SendMailPartUser_commonmodel", ex.Message + "_" + Toemailid);
                return 0;
            }
        }
        public static int SendMailRevartPartUser(string ToTeamemailids, string Toemailid, string gvid, string name, string TeamRevertMessage, string status)
        {
            Grievance_DBEntities _db = new Grievance_DBEntities();
            int noofsend = 0;
            string To = "", Subject = "", Body = "", ReceiverName = ""
                , SenderName = "", RandomValue = "", OTPCode = "";
            string OtherEmailID = "sinhabinduk@gmail.com,sinhaharshit829@gmail.com";
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
                if (MvcApplication.CUser != null)
                {
                    ReceiverName = MvcApplication.CUser.RoleId == "3" ? "Dear User" : "Dear Panel Member";
                }
                To = Toemailid;
                //Body = "The grievance <b> Case ID : " + gvid + "</ b> Status is " + status + ". </ br> <b>Visit : <a href=" + CommonModel.GetBaseUrl() + " style='font-size:medium !important;'></a> </b> for details.";
                Body = @"The grievance <b> Case ID : " + gvid + "</b> Status is " + status + ". <br/><b>Visit: <a href='" + CommonModel.GetBaseUrl() + "' style='font-size:medium !important;'>Click Here</a></b> for details.";

                bodydata = bodyTemplate.Replace("{Dearusername}", ReceiverName)
                    // .Replace("{bodytext}", name + ", Your Grievance Status has been Changed To <b>" + status + "</b>.We'll Inform You on next Update.")
                    .Replace("{bodytext}", Body);
                // .Replace("{EmailID}", To)
                //.Replace("{newusername}", Name);
                //.Replace("{CaseID}", gvid)
                //.Replace("{Status}", status)
                // .Replace("{message}", TeamRevertMessage);

                //using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/Shared/MailTemplate.html")))
                //{
                //    bodyTemplate = reader.ReadToEnd();
                //}
                //bodyTemplate = "<table width=\"100%\" border=\"0\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\">\r\n\t\t<tbody>\r\n <tr>\r\n\t\t\t<td align=\"center\"> " + bodydata + "\r\n\t\t\t\t\r\n  \t</tbody></tr>\r\n</table>";
                MailMessage mail = new MailMessage();
                //mail.To.Add("bindu@careindia.org");
                mail.To.Add(To);// + "," + OtherEmailID
                mail.Bcc.Add(ToTeamemailids + "," + OtherEmailID);
                mail.From = new MailAddress("pci4tech@gmail.com", "Grievance Query");
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
                //smtp.Credentials = new System.Net.NetworkCredential("kgbvjh4care@gmail.com", "yklzeazktmknvcbu");// yklz eazk tmkn vcbu//Pasw-Care@321 // Enter seders User name and password       
                smtp.Credentials = new System.Net.NetworkCredential("pci4tech@gmail.com", "tylodouomqitatre");// yklz eazk tmkn vcbu//Pasw-Care@321 // Enter seders User name and password       
                smtp.EnableSsl = true;
                smtp.Send(mail);
                return 1;

            }
            catch (Exception ex)
            {
                CommonModel.ExpSubmit("Tbl_TeamRevertComplain", "Complain_UserCom", "RevartMail", "SendMailRevartPartUser_commonmodel", ex.Message + "_" + Toemailid);
                return 0;
            }
        }
    }
}