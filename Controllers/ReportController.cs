using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Grievancemis.Manager;
using Newtonsoft.Json;

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
            //DataSet ds = SP_Model.GetDashboard();
            //if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            //{
            //    DataRow row = ds.Tables[0].Rows[0];
            //    ViewBag.TotalComplain = row["TotalCount"];
            //    ViewBag.NoofClarificationData = row["ClarificationSub"];
            //    ViewBag.NoOfClosedData = row["ClosedSub"];
            //    ViewBag.NoOfnewData = row["Newsub"];
            //}
            //else
            //{
            //    ViewBag.TotalComplain = 0;
            //    ViewBag.NoofClarificationData = 0;
            //    ViewBag.NoOfClosedData = 0;
            //    ViewBag.NoOfnewData = 0;
            //}
            return View();
        }
        public ActionResult GetDashboard()
        {
            bool IsCheck = false;
            try
            {
                DataSet ds = SP_Model.GetDashboard();
                if (ds.Tables.Count > 0)
                {
                    IsCheck = true;
                    var data = JsonConvert.SerializeObject(ds);
                    var res1 = Json(new { IsSuccess = IsCheck, Data = data }, JsonRequestBehavior.AllowGet);
                    res1.MaxJsonLength = int.MaxValue;
                    return res1;
                }
                var datares = JsonConvert.SerializeObject(ds);
                var res = Json(new { IsSuccess = IsCheck, Data = Enums.GetEnumDescription(Enums.eReturnReg.RecordNotFound), resData = "" }, JsonRequestBehavior.AllowGet);
                res.MaxJsonLength = int.MaxValue;
                return res;
            }
            catch (Exception ex)
            {
                string er = ex.Message;
                return Json(new { IsSuccess = IsCheck, Data = Enums.GetEnumDescription(Enums.eReturnReg.ExceptionError) }, JsonRequestBehavior.AllowGet); throw;
            }
        }

    }
}