using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ONLINEBO
{
    public class ParGlobalClass
    {

        public static string get_client_code_from_tracking(string tracking_no)
        {
            string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();

            SqlConnection oConnection = new SqlConnection(SqlConn);

            string query = "SELECT RCODE FROM T_ONLINE_BO_AccountHolder WHERE TRACKINGNO=@tracking";

            SqlCommand oCommand = new SqlCommand(query, oConnection);
            oCommand.Parameters.AddWithValue("@tracking", tracking_no);
            //oCommand.Parameters.AddWithValue("@pass", bomodel.Password);

            oConnection.Open();

            DataTable oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader());

            foreach (DataRow dr in oTable.Rows)
            {
                return dr["RCODE"].ToString();
            }

            return null;
        }

        public  static void LastUpdateDate(string trackingno)
        {
            string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();
            SqlConnection oConnection = new SqlConnection(SqlConn);
            oConnection.Open();

            SqlCommand oCommand = new SqlCommand("UPDATE [T_ONLINE_BO_REG] SET LastUpdate=GETDATE() WHERE TRACKINGNO='" + trackingno + "'", oConnection);
            int queryResult = oCommand.ExecuteNonQuery();
            oConnection.Close();
        }


    }///end of the class
}