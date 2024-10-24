using Grievancemis.Models;
using SubSonic.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Grievancemis
{
    public class MvcApplication : System.Web.HttpApplication
    {
        Grievance_DBEntities db=new Grievance_DBEntities();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        public static UserViewModel CUser
        {
            get
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    var User = new UserViewModel();
                    if (HttpContext.Current.Session["CUser"] != null)
                    {
                        return (UserViewModel)HttpContext.Current.Session["CUser"];
                    }
                    else
                    {
                        StoredProcedure sp = new StoredProcedure("SP_LoginCheck");
                        sp.Command.AddParameter("@EmailId",HttpContext.Current.Session["EmailId"], DbType.String);
                        DataSet ds = sp.ExecuteDataSet();
                        DataTable dt = new DataTable();
                        DataTable dtu = new DataTable();
                        if (ds.Tables.Count>0)
                        {
                            dt = ds.Tables[0];
                            //dtu = ds.Tables[1]; 
                        }
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                User.UserId = dr["Id"].ToString();
                                User.Name = dr["Name"].ToString();
                                User.EmailId = dr["Email"].ToString();
                                User.UserName = dr["UserName"].ToString();
                                User.Phone = dr["PhoneNumber"].ToString();
                                User.LockoutEnabled = dr["LockoutEnabled"].ToString();
                                User.RoleId = dr["RoleId"].ToString();
                                User.Role = dr["RoleName"].ToString();
                            }
                        }
                        //if (dtu.Rows.Count > 0)
                        //{
                        //    foreach (DataRow dr in dtu.Rows)
                        //    {
                        //        User.UserId = dr["Id"].ToString();
                        //        User.Name = dr["Name"].ToString();
                        //        User.EmailId = dr["Email"].ToString();
                        //        User.Phone = dr["PhoneNo"].ToString();
                        //        User.LockoutEnabled = dr["IsActive"].ToString();
                        //        User.RoleId = dr["RoleId"].ToString();
                        //        User.Role = dr["RoleName"].ToString();
                        //    }
                        //}

                        HttpContext.Current.Session["CUser"] = User;
                        return CUser;
                    }
                    //if (HttpContext.Current.Session["User"] != null)
                    //{
                    //    return (UserViewModel)HttpContext.Current.Session["User"];
                    //}
                    //else
                    //{
                    //    var u = dbe.AspNetUsers.Single(x => x.UserName == HttpContext.Current.User.Identity.Name);
                    //    var dis = (from l in dbe.Dist_Mast
                    //               join un in dbe.AspNetUsers on l.ID equals un.DistrictId
                    //               where ((u.DistrictId != 0) || u.DistrictId == 0 && un.LockoutEnabled == true)
                    //               select l);

                    //    var role = GetUserRole();
                    //    var forAll = new List<string>() { "All", "Admin" };

                    //    var user = new UserViewModel
                    //    {
                    //        Id = u.Id,
                    //        Name = u.Name,
                    //        Email = u.Email,
                    //        DistrictId = u.DistrictId.Value,
                    //        District = string.Join(", ", dis.Select(x => x.DistName)),
                    //        PhoneNumber = u.PhoneNumber,
                    //        Role = u.AspNetRoles.First()?.Name,
                    //    };
                    //    HttpContext.Current.Session["User"] = user;
                    //    return user;
                    //}
                }
                else
                {
                    RouteCollection routes = new RouteCollection();
                    routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

                    routes.MapRoute(
                        name: "Default",
                        url: "{controller}/{action}/{id}",
                        defaults: new { controller = "Grievnce", action = "GrievanceCaseAdd", id = UrlParameter.Optional }
                    );
                    //HttpContext.Current.Response.RedirectToRoute("~/Account/Login", false);

                    //HttpContext.Current.Response.Redirect("~/Account/Login", false);
                    //RewritePath
                    return null;
                }
            }
        }
    }
}
