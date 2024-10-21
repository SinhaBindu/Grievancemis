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


namespace Grievancemis.Manager
{
    public class SP_Model
    {
        public static DataTable GetGrievanceList(FilterModel filtermodel)
        {
            StoredProcedure sp = new StoredProcedure("USP_GetGrievancefilterList");
            sp.Command.AddParameter("@StateId", filtermodel.StateId, DbType.String);
            sp.Command.AddParameter("@TypeGId", filtermodel.TypeGId, DbType.String);
            DataTable dt = sp.ExecuteDataSet().Tables[0];
            return dt;
        }
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

        public static DataTable GetRevartMail(string GVID)
        {
            StoredProcedure sp = new StoredProcedure("Usp_RevartMailSend");
            sp.Command.AddParameter("@Id", GVID, DbType.String);
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

    }
}
