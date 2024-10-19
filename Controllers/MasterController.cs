using Grievancemis.Manager;
using Grievancemis.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Grievancemis.Controllers
{
    public class MasterController : Controller
    {
        // GET: Master
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UserDetaillist()
        {
            RegisterViewModel model = new RegisterViewModel();
            return View(model);
        }
        public ActionResult GetUserDetailData(int? RoleId, int CheckBox = 0)
        {
            try
            {
                RoleId = RoleId == null ? 0 : RoleId;
                bool IsCheck = false;
                var tbllist = SP_Model.SPGetUserlist(RoleId);
                if (tbllist != null)
                {
                    IsCheck = true;
                }
                if (CheckBox == 1)
                {
                    var html1 = ConvertViewToString("_UserDetailMPData", tbllist);
                    var res1 = Json(new { IsSuccess = IsCheck, Data = html1 }, JsonRequestBehavior.AllowGet);
                    res1.MaxJsonLength = int.MaxValue;
                    return res1;
                }
                var html = ConvertViewToString("_UserDetailData", tbllist);
                var res = Json(new { IsSuccess = IsCheck, Data = html }, JsonRequestBehavior.AllowGet);
                res.MaxJsonLength = int.MaxValue;
                return res;

            }
            catch (Exception ex)
            {
                string er = ex.Message;
                return Json(new { IsSuccess = false, Data = "" }, JsonRequestBehavior.AllowGet); throw;
            }
        }

        private string ConvertViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);
                vResult.View.Render(vContext, writer);
                return writer.ToString();
            }
        }

    }
}