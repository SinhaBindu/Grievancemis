using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Grievancemis.Manager;

namespace Grievancemis.Controllers
{
    [Authorize]
    [SessionCheckAttribute]
    public class ReportController : Controller
    {
        // GET: Report
        //public ActionResult Dashboard()
        //{
        //    return View();
        //}
        public ActionResult Index()
        {
            DataSet ds = SP_Model.GetDashboard();
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                ViewBag.TotalComplain = row["TotalCount"];
                ViewBag.NoofClarificationData = row["ClarificationSub"];
                ViewBag.NoOfClosedData = row["ClosedSub"];
                ViewBag.NoOfnewData = row["Newsub"];
            }
            else
            {
                ViewBag.TotalComplain = 0;
                ViewBag.NoofClarificationData = 0;
                ViewBag.NoOfClosedData = 0;
                ViewBag.NoOfnewData = 0;
            }
            return View();
        }
        
    }
}