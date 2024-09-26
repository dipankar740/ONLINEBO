using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

namespace ONLINEBO.Models
{
    public class OnlineBoRegDBHandle
    {
        string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();

        // **************** ADD NEW Online BO Detail *********************
        public bool AddOnlineBoReg(HomeModel bomodel)
        {
            SqlConnection oConnection = new SqlConnection(SqlConn);

            string query = "INSERT INTO T_ONLINE_BO_REG([TRACKINGNO],[FIRSTNAME],[LASTNAME],[EMAIL],[MOBILE],[PASS],[USERID],[pDATE],[IsActive],[IsApp1],[IsApp2]) VALUES(@trackingno,@FIRSTNAME,@LASTNAME,@EMAIL,@MOBILE,@PASS,@USERID,GETDATE(),1,0,0);INSERT INTO T_ONLINE_BO_AccountHolder(TRACKINGNO,fFirstName,fLastName,fMobile,fEmail) VALUES(@trackingno,@FIRSTNAME,@LASTNAME,@MOBILE,@EMAIL);INSERT INTO T_ONLINE_BO_AccHolderBANK(TRACKINGNO) VALUES(@trackingno);INSERT INTO T_ONLINE_BO_AccHolderAuthorize(TRACKINGNO) VALUES(@trackingno);INSERT INTO T_ONLINE_BO_AccHolderNominee(TRACKINGNO) VALUES(@trackingno);INSERT INTO T_ONLINE_BO_AccHolderImages(TRACKINGNO) VALUES(@trackingno)";
            //string query = "INSERT INTO T_ONLINE_BO_REG([TRACKINGNO],[FIRSTNAME],[LASTNAME],[EMAIL],[MOBILE],[PASS],[USERID],[pDATE],[IsActive]) VALUES('"
            //    + bomodel.TrackingNo + "','"
            //    + bomodel.FirstName + "','"
            //    + bomodel.LastName + "','"
            //    + bomodel.Email + "','"
            //    + bomodel.Mobile + "','"
            //    + bomodel.Password + "','"
            //    + bomodel.TrackingNo + "',GETDATE(),1)"; 

            SqlCommand oCommand = new SqlCommand(query, oConnection);
            oCommand.Parameters.AddWithValue("@trackingno", bomodel.TrackingNo);
            oCommand.Parameters.AddWithValue("@FIRSTNAME", "");
            oCommand.Parameters.AddWithValue("@LASTNAME", "");
            oCommand.Parameters.AddWithValue("@EMAIL", bomodel.Email);
            oCommand.Parameters.AddWithValue("@MOBILE", bomodel.Mobile);
            oCommand.Parameters.AddWithValue("@PASS", bomodel.Password);
            oCommand.Parameters.AddWithValue("@USERID", bomodel.TrackingNo);

            oConnection.Open();
            int i = oCommand.ExecuteNonQuery();
            oConnection.Close();

            if (i >= 1)
                return true;
            else
                return false;
        }


    }
}