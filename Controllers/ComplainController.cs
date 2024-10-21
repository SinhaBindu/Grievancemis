using Grievancemis.Manager;
using Grievancemis.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ImageResizer.Plugins.Basic.Image404;

namespace Grievancemis.Controllers
{
   
    public class ComplainController : Controller
    {
        // GET: Complain
        private Grievance_DBEntities db = new Grievance_DBEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GrievanceList()
        {
            return View();
        }
        public ActionResult GetGrievanceList(FilterModel filtermodel)
        {
            try
            {
                bool IsCheck = false;
                var dt = SP_Model.GetGrievanceList(filtermodel);
                if (dt.Rows.Count > 0)
                {
                    IsCheck = true;
                }
                var html = ConvertViewToString("_GrievanceData", dt);
                var res = Json(new { IsSuccess = IsCheck, Data = html }, JsonRequestBehavior.AllowGet);
                res.MaxJsonLength = int.MaxValue;
                return res;

            }
            catch (Exception ex)
            {
                string er = ex.Message;
                return Json(new { IsSuccess = false, Data = "There are communication error...." }, JsonRequestBehavior.AllowGet); throw;
            }
        }

        public ActionResult RevertCList()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RevertCPost(FilterModel filterModel)
        {
            try
            {
                if (filterModel.GrievanceId_fk == Guid.Empty || filterModel.RevertTypeId == 0 && string.IsNullOrWhiteSpace(filterModel.TeamRevertMessage))
                {
                    return Json(new { success = false, message = "All fileds are required." });
                }
                using (var db = new Grievance_DBEntities())
                {
                    Tbl_TeamRevertComplain teamRevertComplain = new Tbl_TeamRevertComplain
                    {
                        GrievanceId_fk = filterModel.GrievanceId_fk,
                        RevertTypeId = filterModel.RevertTypeId,
                        TeamRevertMessage = filterModel.TeamRevertMessage,
                        IsActive = true,
                        TeamRevert_Date = DateTime.Now.Date,
                        CreatedBy = User.Identity.Name,
                        CreatedOn = DateTime.Now
                    };

                    db.Tbl_TeamRevertComplain.Add(teamRevertComplain);
                    int res = db.SaveChanges();
                    if (res > 0)
                    {
                        DataTable dt = new DataTable();
                        dt = SP_Model.GetRevartMail(teamRevertComplain.GrievanceId_fk.ToString());
                        if (dt.Rows.Count > 0)
                        {
                            res = CommonModel.SendMailRevartPartUser(dt.Rows[0]["Email"].ToString(), dt.Rows[0]["GrievanceId_fk"].ToString(), dt.Rows[0]["Name"].ToString(), dt.Rows[0]["RevertStatus"].ToString());
                        }
                        if (res > 0)
                        {
                            return Json(new { success = true, message = "Data saved and mail sended successfully!" });
                        }
                        else
                        {
                            return Json(new { success = true, message = "Data saved successfully!" });
                        }
                    }
                    else
                    {
                        return Json(new { success = true, message = "Record not Submitted." });
                    }
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        public ActionResult RevartList()
        {
            return View();
        }
        public ActionResult GetRevartList(FilterModel filtermodel)
        {
            try
            {
                bool IsCheck = false;
                var dt = SP_Model.GetRevartList(filtermodel);
                if (dt.Rows.Count > 0)
                {
                    IsCheck = true;
                }
                var html = ConvertViewToString("_ComData", dt);
                var res = Json(new { IsSuccess = IsCheck, Data = html }, JsonRequestBehavior.AllowGet);
                res.MaxJsonLength = int.MaxValue;
                return res;

            }
            catch (Exception ex)
            {
                string er = ex.Message;
                return Json(new { IsSuccess = false, Data = "There are communication error...." }, JsonRequestBehavior.AllowGet); throw;
            }
        }

        [HttpPost]
        public ActionResult OPtSendMail(string EmailId, string OPTCode)
        {
            var res = -1;
            if (!string.IsNullOrWhiteSpace(EmailId) && string.IsNullOrWhiteSpace(OPTCode))
            {
                var vildemailid = EmailId.Trim().Split('@')[1];
                if (vildemailid.ToLower() == "pciglobal.in" || vildemailid.ToLower() == "gmail.com" || vildemailid.ToLower() == "projectconcernindia.org")
                {
                    res = CommonModel.SendMailForUser(EmailId);
                    if (res == 1)
                    {
                        return Json(new { success = true, message = "Please check the mail sent otp code.", resdata = 1 });
                    }
                    return Json(new { success = false, message = "EmailId not verify.", resdata = res });
                }
                else
                {
                    return Json(new { success = false, message = "EmailId Invalid.", resdata = "" });
                }
            }
            else if (!string.IsNullOrWhiteSpace(EmailId) && !string.IsNullOrWhiteSpace(OPTCode))
            {
                Grievance_DBEntities _db = new Grievance_DBEntities();
                var vildemailid = EmailId.Trim().Split('@')[1];
                if (vildemailid.ToLower() == "pciglobal.in" || vildemailid.ToLower() == "gmail.com" || vildemailid.ToLower() == "projectconcernindia.org")
                {
                    var tbl = _db.Tbl_LoginVerification.Where(x => x.EmailId.ToLower() == EmailId.Trim().ToLower() && x.IsActive == true && x.VerificationCode.ToLower() == OPTCode.ToLower().Trim())?.FirstOrDefault();// && x.Date == DateTime.Now.Date
                    if (tbl != null)
                    {
                        tbl.IsValidEmailId = true;
                        tbl.IsActive = false;
                        res = _db.SaveChanges();
                        if (res == 1)
                        {
                            return Json(new { success = true, message = "EmailId Verified.", resdata = 2 });
                        }

                    }
                    else
                    {
                        if (tbl != null)
                        {
                            tbl.IsValidEmailId = false;
                            tbl.IsActive = false;
                        }
                    }
                    res = _db.SaveChanges();
                    if (res == 1)
                    {
                        return Json(new { success = false, message = "EmailId Invalid.", resdata = 3 });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "EmailId Invalid.", resdata = "" });
                }
            }

            return Json(new { success = false, message = "EmailId Invalid.", resdata = "" });
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