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


        #region Master 

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

        public static int SendMailForParticipants(string BatchId, string ToEmail)
        {
            int noofsend = 0;
            string To = "", Subject = "", Body = "", ReceiverName = ""
                , SenderName = "", RandomValue = "", Password = "";
            string ASDT = ""; string DurationTime = ""; string BatchName = "";
            string TrainerName = ""; string DistrictAgencyTrainingCenter = "";
            string OtherEmailID = ""; string maxdateExam = ""; string maxdateExamTimeStartEnd = "";
            Grievance_DBEntities db_ = new Grievance_DBEntities();
          //  DataTable dt = SPManager.SP_MailSendParticipantWise(BatchId, ParticipantIds);
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
                //if (dt.Rows.Count > 0)
                //{
                //    AssessmentScheduleId_pk = Guid.Parse(dt.Rows[0]["AssessmentScheduleId_pk"].ToString());
                //    List<tbl_AssessmentSendLinkEmail> tbllist1 = db_.tbl_AssessmentSendLinkEmail
                //            .Where(x => x.AssessmentScheduleId_fk == AssessmentScheduleId_pk).ToList();
                //    foreach (DataRow row in dt.Rows)
                //    {
                        maxdateExam = row["ExamDt"].ToString();
                        maxdateExamTimeStartEnd = row["StartTime"].ToString() + "/" + row["EndTime"].ToString();
                        To = row["EmailID"].ToString();
                        ParticipantId = Guid.Parse(row["ParticipantId_fk"].ToString());
                        OtherEmailID = row["OtherEmailID"].ToString();
                        //SenderName = row["EmailID"].ToString();
                        BatchName = row["BatchName"].ToString();
                        Subject = row["CourseName"].ToString();
                        RandomValue = row["RandomValue"].ToString();
                        ReceiverName = row["Name"].ToString();
                        TrainerName = row["TrainerName"].ToString();
                        var URL = CommonModel.GetBaseUrl() + "/ParticipantUser/Login?RandomValue=" + RandomValue;
                        DistrictAgencyTrainingCenter = row["DistrictAgencyTrainingCenter"].ToString();
                        Password = row["Password"].ToString();
                        ASDT = CommonModel.FormateDtDMY(row["ExamDt"].ToString());
                        DurationTime = "Start :" + CommonModel.GetTimeSpanVal(row["StartTime"].ToString())
                            + " To End :" + CommonModel.GetTimeSpanVal(row["EndTime"].ToString());
                        //bodyTemplate += "Hi " + ReceiverName + "," + " <br /> " + Body;
                        bodydata = bodyTemplate.Replace("{Dearusername}", ReceiverName)
                            .Replace("{bodytext}", Body)
                            .Replace("{CourseName}", Subject)
                            .Replace("{EmailID}", To)
                            .Replace("{Password}", Password)
                            .Replace("{RandomValue}", RandomValue)
                            .Replace("{newusername}", "Assessment")
                            .Replace("{ASDT}", ASDT)
                            .Replace("{DurationTime}", DurationTime)
                            .Replace("{BatchName}", BatchName)
                            .Replace("{TrainerName}", TrainerName)
                            .Replace("{URL}", URL)
                            .Replace("{DistrictAgencyTrainingCenter}", DistrictAgencyTrainingCenter);
                        //using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/Shared/MailTemplate.html")))
                        //{
                        //    bodyTemplate = reader.ReadToEnd();
                        //}
                        //bodyTemplate = "<table width=\"100%\" border=\"0\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\">\r\n\t\t<tbody>\r\n <tr>\r\n\t\t\t<td align=\"center\"> " + bodydata + "\r\n\t\t\t\t\r\n  \t</tbody></tr>\r\n</table>";
                        MailMessage mail = new MailMessage();
                        //mail.To.Add("bindu@careindia.org");
                        mail.To.Add(To + "," + OtherEmailID);
                        mail.From = new MailAddress("kgbvjh4care@gmail.com", "Hunar MIS");
                        //mail.From = new MailAddress("hunarmis2024@gmail.com");
                        mail.Subject = Subject + " ( Assessment Date : ) " + ASDT;// + " ( " + SenderName + " )";

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
                        noofsend++;


                        tbl_SendMail tbl = new tbl_SendMail();
                        tbl.Id = Guid.NewGuid();
                        tbl.MTo = row["EmailID"].ToString();
                        tbl.ToOtherMail = OtherEmailID;
                        tbl.Batch_Id = Convert.ToInt32(row["BatchId"].ToString());
                        tbl.AssessmentId = AssessmentScheduleId_pk;
                        tbl.ParticipantId = Guid.Parse(row["ParticipantId_fk"].ToString());
                        tbl.MFrom = "kgbvjh4care@gmail.com";
                        //tbl.MFrom = "careindiabtsp@gmail.com";
                        tbl.Subject = Subject + " ( " + SenderName + " )";
                        tbl.Boby = bodyTemplate;
                        tbl.ReceiverName = ReceiverName;
                        tbl.SenderName = row["ParticipantId_fk"].ToString(); //SenderName;
                        tbl.IsSented = true;
                        tbl.ExtraCol1 = maxdateExam;
                        tbl.ExtraCol2 = maxdateExamTimeStartEnd;
                        tbl.CreatedBy = MvcApplication.CUser.UserId;
                        tbl.CreatedOn = DateTime.Now;
                        db_.tbl_SendMail.Add(tbl);
                        db_.SaveChanges();

                        Guid Partid = Guid.Parse(row["ParticipantId_fk"].ToString());
                        tbl_AssessmentSendLinkEmail tbl1 = tbllist1
                            .Where(x => x.ParticipantId_fk == Partid).FirstOrDefault();
                        tbl1.IsEmailSend = 2;
                        db_.SaveChanges();

                    }
                    return noofsend;
                }
                return -1;
            }
            catch (Exception ex)
            {
                tbl_SendMail tbl = new tbl_SendMail();
                tbl.Id = Guid.NewGuid();
                tbl.MTo = To;
                tbl.ToOtherMail = OtherEmailID;
                tbl.AssessmentId = AssessmentScheduleId_pk;
                tbl.AssessmentId = ParticipantId;
                tbl.Batch_Id = Convert.ToInt32(BatchId);
                //tbl.MFrom = "careindiabtsp@gmail.com";
                tbl.MFrom = "kgbvjh4care@gmail.com";
                tbl.Subject = Subject + " ( " + SenderName + " )";
                tbl.Boby = bodyTemplate;
                tbl.ReceiverName = ReceiverName;
                //tbl.ParticipantId = Guid.Parse(SenderName);
                tbl.SenderName = ParticipantIds;
                tbl.IsSented = false;
                tbl.ExtraCol1 = maxdateExam;
                tbl.ExtraCol2 = maxdateExamTimeStartEnd;
                tbl.CreatedBy = MvcApplication.CUser.UserId;
                tbl.CreatedOn = DateTime.Now;
                db_.tbl_SendMail.Add(tbl);
                db_.SaveChanges();
                return -2;
            }
        }


        #endregion


    }
}