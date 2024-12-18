using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Grievancemis.Models;

namespace Grievancemis.Controllers
{
    [Authorize]
    [SessionCheckAttribute]
    public class FeedbackController : Controller
    {
        private Grievance_DBEntities db = new Grievance_DBEntities();
        // GET: Feedback
        public ActionResult Feedback()
        {
         
            return View();
        }
        [HttpPost]
        public ActionResult Feedback(Feedback model)
        {
            try
            {
                int res = 0;
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrWhiteSpace(model.Feedback_Id))
                    {
                        res = 1;
                    }
                    if (res > 0)
                    {
                        using (var db = new Grievance_DBEntities())
                        {
                            Tbl_Feedback feedbackEntity = new Tbl_Feedback
                            {
                                Feedback_Id = model.Feedback_Id,
                                IsActive = true,
                                CreatedBy = User.Identity.Name,
                                CreatedOn = DateTime.Now,
                                UpdatedBy = MvcApplication.CUser.UserId,
                            };
                            db.Tbl_Feedback.Add(feedbackEntity);
                            db.SaveChanges();
                            ViewBag.SuccessMessage = "Feedback submitted successfully.";
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Feedback is not valid. Please correct the errors in the form.";
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Please correct the errors in the form.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while saving your feedback. Please try again.";
            }
            return View(model);
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