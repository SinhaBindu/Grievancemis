using SubSonic.Schema;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grievancemis.Manager;
using Grievancemis.Models;
using System.Web;
using Grievancemis.Helpers;


namespace Grievancemis.Manager
{
    public class SP_Model
    {
        public static DataTable GetGrievanceList(FilterModel filtermodel)
        {
            StoredProcedure sp = new StoredProcedure("USP_GetGrievancefilterList");
            sp.Command.AddParameter("@StateId", filtermodel.StateId, DbType.String);
            sp.Command.AddParameter("@TypeGId", filtermodel.TypeGId, DbType.String);
            //sp.Command.AddParameter("@GrievanceId", filtermodel.GrievanceId, DbType.String);
            DataTable dt = sp.ExecuteDataSet().Tables[0];
            return dt;
        }
        public static DataTable GetGrievanceDoc(FilterModel filtermodel)
        {
            StoredProcedure sp = new StoredProcedure("Usp_GrievanceImage");
            sp.Command.AddParameter("@GrievanceId", filtermodel.GrievanceId, DbType.String);
            DataTable dt = sp.ExecuteDataSet().Tables[0];
            return dt;
        }
        public static DataTable GetUserGList(FilterModel filtermodel)
        {
            StoredProcedure sp = new StoredProcedure("USP_UserGrievanceList");
            sp.Command.AddParameter("@StateId", filtermodel.StateId, DbType.String);
            sp.Command.AddParameter("@TypeGId", filtermodel.TypeGId, DbType.String);
            sp.Command.AddParameter("@EmailID", MvcApplication.CUser.EmailId, DbType.String);
            DataTable dt = sp.ExecuteDataSet().Tables[0];
            return dt;
        }
        //public static DataTable GetGrievanceDocuments(string grievanceId)
        //{
        //    StoredProcedure sp = new StoredProcedure("USP_GetGrievanceDocuments");
        //    sp.Command.AddParameter("@GrievanceId", grievanceId, DbType.String);
        //    DataTable dt = sp.ExecuteDataSet().Tables[0];
        //    return dt;
        //}
        public static DataTable SPGetUserlist(int? RoleId)
        {
            StoredProcedure sp = new StoredProcedure("SPGetUserlist");
            sp.Command.AddParameter("@RoleId", RoleId, DbType.Int32);
            sp.Command.AddParameter("@User", HttpContext.Current.User.Identity.Name, DbType.String);
            DataTable dt = sp.ExecuteDataSet().Tables[0];
            return dt;
        }

        public static DataTable GetRevartList(FilterModel filtermodel)
        {
            StoredProcedure sp = new StoredProcedure("USP_RevertCDataList");
            sp.Command.AddParameter("@StateId", filtermodel.StateId, DbType.String);
            sp.Command.AddParameter("@TypeGId", filtermodel.TypeGId, DbType.String);
            sp.Command.AddParameter("@RevertTypeId", filtermodel.RevertTypeId, DbType.Int32);
            sp.Command.AddParameter("@GId", filtermodel.GrievanceId, DbType.String);
            DataTable dt = sp.ExecuteDataSet().Tables[0];
            return dt;
        }
        public static DataTable GetTeamMailID()
        {
            StoredProcedure sp = new StoredProcedure("GetEmailsByRoleId");
            sp.Command.AddParameter("@RoleId", 0, DbType.Int32);
            DataTable dt = sp.ExecuteDataSet().Tables[0];
            return dt;
        }

        public static DataTable GetRevartMail(string GVID,string RevertId)
        {
            StoredProcedure sp = new StoredProcedure("Usp_RevartMailSend");
            sp.Command.AddParameter("@Id", GVID, DbType.String);
            sp.Command.AddParameter("@RevertId", RevertId, DbType.String);
            DataTable dt = sp.ExecuteDataSet().Tables[0];
            return dt;
        }
        public static DataSet GetLoginCheckMail(string EmailId)
        {
            StoredProcedure sp = new StoredProcedure("SP_LoginCheck");
            sp.Command.AddParameter("@EmailId", EmailId, DbType.String);
            DataSet ds = sp.ExecuteDataSet();
            return ds;
        }
        public static DataTable GetOTPCheckLoginMail(string EmailId, string OTP)
        {
            StoredProcedure sp = new StoredProcedure("Usp_CheckhourOtp");
            sp.Command.AddParameter("@EmailId", EmailId, DbType.String);
            sp.Command.AddParameter("@OTP", OTP, DbType.String);
            DataTable dt = sp.ExecuteDataSet().Tables[0];
            return dt;
        }
        public static DataTable SP_AspnetUser(string EmailId)
        {
            StoredProcedure sp = new StoredProcedure("SP_AspnetUser");
            sp.Command.AddParameter("@EmailId", EmailId, DbType.String);
            DataTable dt = sp.ExecuteDataSet().Tables[0];
            return dt;
        }
        public static DataTable SP_AspnetUserCaseFirstTimeCheck(string EmailId,string AspnetUserId)
        {
            StoredProcedure sp = new StoredProcedure("SP_AspnetUserCaseFirstTimeCheck");
            sp.Command.AddParameter("@EmailId", EmailId, DbType.String);
            sp.Command.AddParameter("@AspnetUserId", AspnetUserId, DbType.String);
            DataTable dt = sp.ExecuteDataSet().Tables[0];
            return dt;
        }
        public static DataTable Usp_GetCaseIDwithguid(string guid)
        {
            StoredProcedure sp = new StoredProcedure("Usp_GetCaseIDwithguid");
            sp.Command.AddParameter("@guid", guid, DbType.String);
            DataTable dt = sp.ExecuteDataSet().Tables[0];
            return dt;
        }
        public static DataTable Usp_GetCIdRevart(string guid)
        {
            StoredProcedure sp = new StoredProcedure("Usp_GetCIdRevart");
            sp.Command.AddParameter("@guid", guid, DbType.String);
            DataTable dt = sp.ExecuteDataSet().Tables[0];
            return dt;
        }
        public static DataTable GetTeamRevertDetailsByCID(string id)
        {
            StoredProcedure sp = new StoredProcedure("GetTeamRevertDetailsByCID");
            sp.Command.AddParameter("@CaseId", id, DbType.String);
            DataTable dt = sp.ExecuteDataSet().Tables[0];
            return dt;
        }
        
        public static DataSet GetDashboard()
        {
            StoredProcedure sp = new StoredProcedure("Usp_Dashboard");
            sp.Command.AddParameter("@RoleId", MvcApplication.CUser.RoleId, DbType.String);
            sp.Command.AddParameter("@UserId", MvcApplication.CUser.UserId, DbType.String);
            DataSet ds = sp.ExecuteDataSet();
            return ds;

        }
        public static DataTable GetSPCheckGrievanceAlready(string EmailId,string RegDate)
        {
            StoredProcedure sp = new StoredProcedure("SP_CheckGrievanceAlready");
            sp.Command.AddParameter("@EmailId", EmailId, DbType.String);
            sp.Command.AddParameter("@RegDate", RegDate, DbType.String);
            DataTable dt = sp.ExecuteDataSet().Tables[0];
            return dt;

        }
        public static DataTable GetSPCheckRevertAlready()
        {
            StoredProcedure sp = new StoredProcedure("SP_CheckRevertAlready");
            sp.Command.AddParameter("@UserId", MvcApplication.CUser.UserId, DbType.String);
            sp.Command.AddParameter("@Date", DateTime.Now.Date.ToDateTimeyyyyMMdd(), DbType.String);
            DataTable dt = sp.ExecuteDataSet().Tables[0];
            return dt;

        }
    }
}
