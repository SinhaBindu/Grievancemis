using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Grievancemis.Manager
{
    public static class AppConstants
    {
        //public static int TotYearInYearFilter = Convert.ToInt32(ConfigurationManager.AppSettings["TotNoInYearFilter"]);
        public static string LogoPath = "";
        public static string BarCodeFilePath = "~/Uploads/Registration/BarCode/";
        public static string NoDocumentFilePath = "/Content/assets/images/No-Document.png";

    }
    public static class RolesIdcont {
        public static string Admin = "1";
        public static string User = "2";
        public static string Community = "3";
        public static string Head = "4";
    }
    public static class RolesNamecont
    {
        public static string Admin = "Admin";
        public static string User = "User";
        public static string Community = "Community";
        public static string Head = "Head";
    }
}