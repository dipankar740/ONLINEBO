using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web.Mvc;

namespace ONLINEBO.Models
{
    
    public class OtherDBHandler
    {
        string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();

        //public IEnumerable<BankInfoModel> bankInfo()
        //{
        //    //var bank = new List<SelectListItem>();

        //    //SqlConnection oConnection = new SqlConnection(SqlConn);
        //    //oConnection.Open();

        //    ////string query = "SELECT [ROUTING],[BANKNAME],[BRANCH],[DISTRICT] FROM T_BANKINFO";

        //    //string query = "SELECT DISTINCT [BANKNAME] FROM T_BANKINFO";
        //    //SqlCommand oCommand = new SqlCommand(query, oConnection);

        //    //DataTable oTable = new DataTable();
        //    //oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

        //    //foreach (DataRow dr in oTable.Rows)
        //    //{
        //    //    //bank.Add(new BankInfo { BANKNAME = dr["BANKNAME"].ToString(), BANKBRANCH = dr["BRANCH"].ToString(), BANKDISTRICT = dr["DISTRICT"].ToString(), BANKROUTING = dr["ROUTING"].ToString() });
        //    //    bank.Add(new SelectListItem { Text = dr["BANKNAME"].ToString(), Value = dr["BANKNAME"].ToString() });
        //    //}
        //    //return bank.ToList();
        //}


        public IEnumerable<BranchInfo> branchInfo()
        {
            var branch = new List<BranchInfo>();

            SqlConnection oConnection = new SqlConnection(SqlConn);
            oConnection.Open();

            //string query = "SELECT [ROUTING],[BANKNAME],[BRANCH],[DISTRICT] FROM T_BANKINFO";

            string query = "SELECT BRANCHNAME,[ADDRESS] FROM T_BRANCHES WHERE BRANCHNAME NOT IN ('Dhaka Ext special','CTG Ext special','WEB','SMS')";
            SqlCommand oCommand = new SqlCommand(query, oConnection);

            DataTable oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

            foreach (DataRow dr in oTable.Rows)
            {
                //bank.Add(new BankInfo { BANKNAME = dr["BANKNAME"].ToString(), BANKBRANCH = dr["BRANCH"].ToString(), BANKDISTRICT = dr["DISTRICT"].ToString(), BANKROUTING = dr["ROUTING"].ToString() });
                branch.Add(new BranchInfo { BRANCHNAME = dr["BRANCHNAME"].ToString(), BRANCHADDRESS = dr["ADDRESS"].ToString() });
            }
            return branch.ToList();
        }

        public IEnumerable<BankInfoModel> bankDistrictInfo(string bankname)
        {
            var bank = new List<BankInfoModel>();

            SqlConnection oConnection = new SqlConnection(SqlConn);
            oConnection.Open();

            //string query = "SELECT [ROUTING],[BANKNAME],[BRANCH],[DISTRICT] FROM T_BANKINFO";

            string query = "SELECT DISTINCT [DISTRICT] FROM T_BANKINFO WHERE BANKNAME='" + bankname.Trim() + "'";
            SqlCommand oCommand = new SqlCommand(query, oConnection);

            DataTable oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

            foreach (DataRow dr in oTable.Rows)
            {
                //bank.Add(new BankInfo { BANKNAME = dr["BANKNAME"].ToString(), BANKBRANCH = dr["BRANCH"].ToString(), BANKDISTRICT = dr["DISTRICT"].ToString(), BANKROUTING = dr["ROUTING"].ToString() });
                bank.Add(new BankInfoModel { BANKDISTRICT = dr["DISTRICT"].ToString() });
            }
            return bank.ToList();
        }

        public IEnumerable<BankInfoModel> bankbranchInfo(string bankName, string districtName)
        {
            var bank = new List<BankInfoModel>();

            SqlConnection oConnection = new SqlConnection(SqlConn);
            oConnection.Open();

            //string query = "SELECT [ROUTING],[BANKNAME],[BRANCH],[DISTRICT] FROM T_BANKINFO";

            string query = "SELECT [BRANCH] FROM T_BANKINFO WHERE BANKNAME='" + bankName.Trim() + "' AND DISTRICT='" + districtName.Trim() + "'";
            SqlCommand oCommand = new SqlCommand(query, oConnection);

            DataTable oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

            foreach (DataRow dr in oTable.Rows)
            {
                //bank.Add(new BankInfo { BANKNAME = dr["BANKNAME"].ToString(), BANKBRANCH = dr["BRANCH"].ToString(), BANKDISTRICT = dr["DISTRICT"].ToString(), BANKROUTING = dr["ROUTING"].ToString() });
                bank.Add(new BankInfoModel { BANKBRANCH = dr["BRANCH"].ToString() });
            }
            return bank.ToList();
        }

        public IEnumerable<BankInfoModel> bankroutingInfo(string bankName, string districtName, string bankBranch)
        {
            var bank = new List<BankInfoModel>();

            SqlConnection oConnection = new SqlConnection(SqlConn);
            oConnection.Open();

            //string query = "SELECT [ROUTING],[BANKNAME],[BRANCH],[DISTRICT] FROM T_BANKINFO";

            string query = "SELECT [ROUTING] FROM T_BANKINFO WHERE BANKNAME='" + bankName.Trim() + "' AND DISTRICT='" + districtName.Trim() + "' AND BRANCH='" + bankBranch.Trim() + "'";
            SqlCommand oCommand = new SqlCommand(query, oConnection);

            DataTable oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

            foreach (DataRow dr in oTable.Rows)
            {
                //bank.Add(new BankInfo { BANKNAME = dr["BANKNAME"].ToString(), BANKBRANCH = dr["BRANCH"].ToString(), BANKDISTRICT = dr["DISTRICT"].ToString(), BANKROUTING = dr["ROUTING"].ToString() });
                bank.Add(new BankInfoModel { BANKROUTING = dr["ROUTING"].ToString() });
            }
            return bank.ToList();
        }


        public IEnumerable<BankInfoModel> defaultVAlue()
        {
            var bank = new List<BankInfoModel>();

            SqlConnection oConnection = new SqlConnection(SqlConn);
            oConnection.Open();

            //string query = "SELECT [ROUTING],[BANKNAME],[BRANCH],[DISTRICT] FROM T_BANKINFO";

            string query = "SELECT TOP 1 [ROUTING],[BRANCH],[DISTRICT] FROM T_BANKINFO";
            SqlCommand oCommand = new SqlCommand(query, oConnection);

            DataTable oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

            foreach (DataRow dr in oTable.Rows)
            {
                //bank.Add(new BankInfo { BANKNAME = dr["BANKNAME"].ToString(), BANKBRANCH = dr["BRANCH"].ToString(), BANKDISTRICT = dr["DISTRICT"].ToString(), BANKROUTING = dr["ROUTING"].ToString() });
                bank.Add(new BankInfoModel { BANKDISTRICT = dr["DISTRICT"].ToString(), BANKBRANCH = dr["BRANCH"].ToString(), BANKROUTING = dr["ROUTING"].ToString() });
            }
            return bank.ToList();
        }

        public IEnumerable<BankInfoModel> otherBankInfo()
        {
            var bank = new List<BankInfoModel>();

            SqlConnection oConnection = new SqlConnection(SqlConn);
            oConnection.Open();

            //string query = "SELECT [ROUTING],[BANKNAME],[BRANCH],[DISTRICT] FROM T_BANKINFO";

            string query = "SELECT [ROUTING],[BRANCH],[DISTRICT] FROM T_BANKINFO";
            SqlCommand oCommand = new SqlCommand(query, oConnection);

            DataTable oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

            foreach (DataRow dr in oTable.Rows)
            {
                //bank.Add(new BankInfo { BANKNAME = dr["BANKNAME"].ToString(), BANKBRANCH = dr["BRANCH"].ToString(), BANKDISTRICT = dr["DISTRICT"].ToString(), BANKROUTING = dr["ROUTING"].ToString() });
                bank.Add(new BankInfoModel { BANKDISTRICT = dr["DISTRICT"].ToString(), BANKBRANCH = dr["BRANCH"].ToString(), BANKROUTING = dr["ROUTING"].ToString() });
            }
            return bank.ToList();
        }

        public byte[] ConvertImageToByteArray(System.Drawing.Image imageToConvert, System.Drawing.Imaging.ImageFormat formatOfImage)
        {
            byte[] Ret;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    imageToConvert.Save(ms, formatOfImage);
                    Ret = ms.ToArray();
                }
            }
            catch (Exception) { throw; }
            return Ret;
        }


        public IEnumerable<BankInfoModel> GetTitle()
        {
            var branch = new List<BankInfoModel>();


                //bank.Add(new BankInfo { BANKNAME = dr["BANKNAME"].ToString(), BANKBRANCH = dr["BRANCH"].ToString(), BANKDISTRICT = dr["DISTRICT"].ToString(), BANKROUTING = dr["ROUTING"].ToString() });
            branch.Add(new BankInfoModel { TitleValue="Mr.", TitleText="Mr." });
            branch.Add(new BankInfoModel { TitleValue = "Ms.", TitleText = "Ms." });
            branch.Add(new BankInfoModel { TitleValue = "Mrs.", TitleText = "Mrs." });
            branch.Add(new BankInfoModel { TitleValue = "Dr.", TitleText = "Dr." });

            return branch.ToList();
        }
    }


    public enum Gender
    {
        Male,
        Female
    }

    public enum Residency
    {
        Resident,
        NonResident
    }
    public enum Title
    {
        Mr,
        Mrs,
        Ms,
        Dr
    }


}