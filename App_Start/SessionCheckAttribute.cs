using Grievancemis;
using System.Web;
using System.Web.Mvc;

public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var users = MvcApplication.CUser.UserId;
        if (HttpContext.Current.Session["CUser"] == null && (string.IsNullOrWhiteSpace(users)))
        {
            filterContext.Result = new RedirectToRouteResult(
                new System.Web.Routing.RouteValueDictionary {
                    { "controller", "Grievnce" },
                    { "action", "GrievanceCaseAdd" }
                });
        }
        //else if (HttpContext.Current.Session["PartUserId"] !=null)
        //{
        //    filterContext.Result = new RedirectToRouteResult(
        //         new System.Web.Routing.RouteValueDictionary {
        //            { "controller", "Account" },
        //            { "action", "AssessmentDone" }
        //         });
        //}

        base.OnActionExecuting(filterContext);
    }
}
