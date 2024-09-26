using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ONLINEBO.Models
{
    public class AdminDBHandler
    {

        string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();


        public IEnumerable<UserModel> GetUserList()
        {
            var userList = new List<UserModel>();

            SqlConnection oConnection = new SqlConnection(SqlConn);
            oConnection.Open();

            string query = "SELECT ROW_NUMBER() OVER(ORDER BY BRANCHNAME) SL, [BRANCHCODE],[BRANCHNAME],[USERID],[USERTYPE],[UESRNAME],[BRANCH_PREFIX],[AGENT_PREFIX],[password],[LOCK] FROM [T_BRANCH_LOGIN]";
            SqlCommand oCommand = new SqlCommand(query, oConnection);

            DataTable oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

            foreach (DataRow dr in oTable.Rows)
            {
                //bank.Add(new BankInfo { BANKNAME = dr["BANKNAME"].ToString(), BANKBRANCH = dr["BRANCH"].ToString(), BANKDISTRICT = dr["DISTRICT"].ToString(), BANKROUTING = dr["ROUTING"].ToString() });
                userList.Add(new UserModel { SL = dr["SL"].ToString(), BRANCHCODE = dr["BRANCHCODE"].ToString(), BRANCHNAME = dr["BRANCHNAME"].ToString(), USERID = dr["USERID"].ToString(), USERTYPE = dr["USERTYPE"].ToString(), UESRNAME = dr["UESRNAME"].ToString(), BRANCH_PREFIX = dr["BRANCH_PREFIX"].ToString(), AGENT_PREFIX = dr["AGENT_PREFIX"].ToString(), password = dr["password"].ToString(), LOCK = dr["LOCK"].ToString() });
            }
            return userList.ToList();
        }

        public IEnumerable<UserModel> GetUserDetails(string userID)
        {
            var userList = new List<UserModel>();

            SqlConnection oConnection = new SqlConnection(SqlConn);
            oConnection.Open();

            string query = "SELECT [BRANCHCODE],[BRANCHNAME],[USERID],[USERTYPE],[UESRNAME],[BRANCH_PREFIX],[AGENT_PREFIX],[password],[LOCK] FROM [T_BRANCH_LOGIN] WHERE USERID=@userid";
            SqlCommand oCommand = new SqlCommand(query, oConnection);
            oCommand.Parameters.AddWithValue("@userid",userID);


            DataTable oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

            foreach (DataRow dr in oTable.Rows)
            {
                //bank.Add(new BankInfo { BANKNAME = dr["BANKNAME"].ToString(), BANKBRANCH = dr["BRANCH"].ToString(), BANKDISTRICT = dr["DISTRICT"].ToString(), BANKROUTING = dr["ROUTING"].ToString() });
                userList.Add(new UserModel { BRANCHCODE = dr["BRANCHCODE"].ToString(), BRANCHNAME = dr["BRANCHNAME"].ToString(), USERID = dr["USERID"].ToString(), USERTYPE = dr["USERTYPE"].ToString(), UESRNAME = dr["UESRNAME"].ToString(), BRANCH_PREFIX = dr["BRANCH_PREFIX"].ToString(), AGENT_PREFIX = dr["AGENT_PREFIX"].ToString(), password = dr["password"].ToString(), LOCK = dr["LOCK"].ToString() });
            }
            return userList.ToList();
        }


        public bool EditUser(UserModel bomodel)
        {
            int i = 0;
            SqlConnection oConnection = new SqlConnection(SqlConn);

            string query = "UPDATE T_BRANCH_LOGIN SET [BRANCHNAME]=@BRANCHNAME,[USERTYPE]=@USERTYPE,[UESRNAME]=@UESRNAME,[BRANCH_PREFIX]=@BRANCH_PREFIX,[AGENT_PREFIX]=@AGENT_PREFIX,[password]=@password,[LOCK]=@LOCK WHERE [USERID]=@USERID";

            SqlCommand oCommand = new SqlCommand(query, oConnection);

            try
            {
                //oCommand.Parameters.AddWithValue("@TRACKINGNO", 1);
                //oCommand.Parameters.AddWithValue("@BANKNAME", bomodel.BANKNAME);
                //oCommand.Parameters.AddWithValue("@BANKBRANCH", bomodel.BANKBRANCH);
                //oCommand.Parameters.AddWithValue("@DISTRICT", bomodel.BANKDISTRICT);
                //oCommand.Parameters.AddWithValue("@ROUTING", bomodel.BANKROUTING);
                //oCommand.Parameters.AddWithValue("@AC", bomodel.BANKAC);

                oCommand.Parameters.AddWithValue("@USERID", bomodel.USERID.ToString());
                oCommand.Parameters.AddWithValue("@BRANCHNAME", bomodel.BRANCHNAME.ToString());
                oCommand.Parameters.AddWithValue("@USERTYPE", bomodel.USERTYPE);
                oCommand.Parameters.AddWithValue("@UESRNAME", bomodel.UESRNAME);
                oCommand.Parameters.AddWithValue("@BRANCH_PREFIX", bomodel.BRANCH_PREFIX);
                oCommand.Parameters.AddWithValue("@AGENT_PREFIX", bomodel.AGENT_PREFIX);
                oCommand.Parameters.AddWithValue("@password", bomodel.password);
                oCommand.Parameters.AddWithValue("@LOCK", bomodel.LOCK.ToString());



                oConnection.Open();
                i = oCommand.ExecuteNonQuery();
                oConnection.Close();

            }
            catch (Exception ex)
            {
                return false;
            }
            if (i >= 1)
                return true;
            else
                return false;

        }


        public IEnumerable<UserModel> UserTypeInfo()
        {
            var branch = new List<UserModel>();

            SqlConnection oConnection = new SqlConnection(SqlConn);
            oConnection.Open();

            //string query = "SELECT [ROUTING],[BANKNAME],[BRANCH],[DISTRICT] FROM T_BANKINFO";

            string query = "SELECT [USERTYPE] FROM [T_BRANCH_USERTYPE]";
            SqlCommand oCommand = new SqlCommand(query, oConnection);

            DataTable oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

            foreach (DataRow dr in oTable.Rows)
            {
                //bank.Add(new BankInfo { BANKNAME = dr["BANKNAME"].ToString(), BANKBRANCH = dr["BRANCH"].ToString(), BANKDISTRICT = dr["DISTRICT"].ToString(), BANKROUTING = dr["ROUTING"].ToString() });
                branch.Add(new UserModel { USERTYPE = dr["USERTYPE"].ToString() });
            }
            return branch.ToList();
        }

        public IEnumerable<BranchInfo> branchInfo()
        {
            var branch = new List<BranchInfo>();

            SqlConnection oConnection = new SqlConnection(SqlConn);
            oConnection.Open();

            //string query = "SELECT [ROUTING],[BANKNAME],[BRANCH],[DISTRICT] FROM T_BANKINFO";

            string query = "SELECT DISTINCT BRANCHNAME FROM T_BRANCH_LOGIN WHERE BRANCHNAME NOT IN ('Dhaka Ext special','CTG Ext special','WEB','SMS')";
            SqlCommand oCommand = new SqlCommand(query, oConnection);

            DataTable oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

            foreach (DataRow dr in oTable.Rows)
            {
                //bank.Add(new BankInfo { BANKNAME = dr["BANKNAME"].ToString(), BANKBRANCH = dr["BRANCH"].ToString(), BANKDISTRICT = dr["DISTRICT"].ToString(), BANKROUTING = dr["ROUTING"].ToString() });
                branch.Add(new BranchInfo { BRANCHNAME = dr["BRANCHNAME"].ToString() });
            }
            return branch.ToList();
        }

        public IEnumerable<BranchInfo> branchDetail(string branchName)
        {
            var branch = new List<BranchInfo>();

            SqlConnection oConnection = new SqlConnection(SqlConn);
            oConnection.Open();

            //string query = "SELECT [ROUTING],[BANKNAME],[BRANCH],[DISTRICT] FROM T_BANKINFO";

            string query = "SELECT [BRANCHCODE],[BRANCH_PREFIX] FROM T_BRANCH_LOGIN WHERE BRANCHNAME NOT IN ('Dhaka Ext special','CTG Ext special','WEB','SMS') AND BRANCHNAME=@branchname";
            SqlCommand oCommand = new SqlCommand(query, oConnection);
            oCommand.Parameters.AddWithValue("@branchname", branchName);

            DataTable oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

            foreach (DataRow dr in oTable.Rows)
            {
                //bank.Add(new BankInfo { BANKNAME = dr["BANKNAME"].ToString(), BANKBRANCH = dr["BRANCH"].ToString(), BANKDISTRICT = dr["DISTRICT"].ToString(), BANKROUTING = dr["ROUTING"].ToString() });
                branch.Add(new BranchInfo { BRANCHNAME = dr["BRANCHNAME"].ToString() });
            }
            return branch.ToList();
        }


    }




}