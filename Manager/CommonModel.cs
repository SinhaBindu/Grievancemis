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
        
        public static List<SelectListItem> GetGrievanceType()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = "0", Text = "Please Select" });
            list.Add(new SelectListItem { Value = "1", Text = "Harrassment" });
            list.Add(new SelectListItem { Value = "2", Text = "Unfair treatment" });
            list.Add(new SelectListItem { Value = "3", Text = "Major Dispute" });
            list.Add(new SelectListItem { Value = "4", Text = "Sexual Harrassment" });
            list.Add(new SelectListItem { Value = "5", Text = "Abuse" });
            list.Add(new SelectListItem { Value = "6", Text = "Policy Deviation" });
            list.Add(new SelectListItem { Value = "7", Text = "Discrimination" });
            list.Add(new SelectListItem { Value = "8", Text = "Unsafe Working Conditions" });
            return list;
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


    }
}