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
        private Grievance_DBEntities db = new Grievance_DBEntities();

        public static DataTable GetGrievancefilterList(string stateFilter, string typeFilter)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("USP_GetGrievancefilterList", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StateName", stateFilter);
                cmd.Parameters.AddWithValue("@GrievanceType", typeFilter);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

    }
}
