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
}