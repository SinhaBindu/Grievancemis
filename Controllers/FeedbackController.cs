﻿using System;
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
                // Validate the model state
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Please correct the errors in the form." });
                }

                // Check if feedback is empty
                if (string.IsNullOrWhiteSpace(model.Grievance_Feedback))
                {
                    return Json(new { success = false, message = "Feedback cannot be empty." });
                }

                // Create and save feedback
                var feedbackEntity = new Tbl_Feedback
                {
                    Grievance_Feedback = model.Grievance_Feedback,
                    IsActive = true,
                    CreatedBy = MvcApplication.CUser.Name,
                    CreatedOn = DateTime.Now,
                    UpdatedBy = MvcApplication.CUser.UserId,
                };

                db.Tbl_Feedback.Add(feedbackEntity);
                db.SaveChanges();

                return Json(new { success = true, message = "Feedback submitted successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception (if logging is implemented)
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
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