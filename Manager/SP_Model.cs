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


namespace Grievancemis.Manager
{
    public class SP_Model
    {
        public static DataTable GetGrievanceList(string stateFilter, string typeFilter)
        {
            StoredProcedure sp = new StoredProcedure("USP_GetGrievancefilterList");
            sp.Command.AddParameter("@StateName", stateFilter, DbType.String);
            sp.Command.AddParameter("@GrievanceType", typeFilter, DbType.String);
            DataTable dt = sp.ExecuteDataSet().Tables[0];
            return dt;
        }
        
    }
}
