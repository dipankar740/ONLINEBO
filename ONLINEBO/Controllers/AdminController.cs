using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ONLINEBO.Models;
using ONLINEBO.Controllers;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using ONLINEBO.Domain;
using System.Text.RegularExpressions;

namespace ONLINEBO.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserList()
        {
            AdminDBHandler ADB=new AdminDBHandler();
            ViewData["userList"] = ADB.GetUserList().ToList();

            return View();
        }

        [HttpPost]
        public ActionResult UserList(string id)
        {
            return View();
        }

        public ActionResult ViewUser(UserModel bomodel)
        {
            AdminDBHandler ADB = new AdminDBHandler();
            ViewData["UserDetails"] = ADB.GetUserDetails(bomodel.USERID).ToList();

            return View();
        }

        public ActionResult EditUser(UserModel bomodel)
        {
            AdminDBHandler ADB = new AdminDBHandler();
            ViewData["UserDetails"] = ADB.GetUserDetails(bomodel.USERID).ToList();

            return View();
        }

        public ActionResult CreateUser()
        {
            string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();
            AdminDBHandler ABD=new AdminDBHandler();

            SelectList branch = new SelectList(ABD.branchInfo().ToList(), "BRANCHNAME", "BRANCHNAME");
            SelectList userType = new SelectList(ABD.UserTypeInfo().ToList(), "USERTYPE", "USERTYPE");

            ViewBag.BRANCHES = branch;
            ViewBag.UserType = userType;

            //var userList = new List<UserModel>();

            //SqlConnection oConnection = new SqlConnection(SqlConn);
            //oConnection.Open();

            //string query = "SELECT  [BRANCHCODE],[BRANCHNAME],[USERID],[USERTYPE],[UESRNAME],[BRANCH_PREFIX],[AGENT_PREFIX],[password],[LOCK] FROM [T_BRANCH_LOGIN]";
            //SqlCommand oCommand = new SqlCommand(query, oConnection);

            //DataTable oTable = new DataTable();
            //oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

            //foreach (DataRow dr in oTable.Rows)
            //{
            //    //bank.Add(new BankInfo { BANKNAME = dr["BANKNAME"].ToString(), BANKBRANCH = dr["BRANCH"].ToString(), BANKDISTRICT = dr["DISTRICT"].ToString(), BANKROUTING = dr["ROUTING"].ToString() });
            //    userList.Add(new UserModel { SL = dr["SL"].ToString(), BRANCHCODE = dr["BRANCHCODE"].ToString(), BRANCHNAME = dr["BRANCHNAME"].ToString(), USERID = dr["USERID"].ToString(), USERTYPE = dr["USERTYPE"].ToString(), UESRNAME = dr["UESRNAME"].ToString(), BRANCH_PREFIX = dr["BRANCH_PREFIX"].ToString(), AGENT_PREFIX = dr["AGENT_PREFIX"].ToString(), password = dr["password"].ToString(), LOCK = dr["LOCK"].ToString() });
            //}

            return View();
        }

        [HttpPost]
        public JsonResult GetBranchDetails(string branchName)
        {
            AdminDBHandler DB = new AdminDBHandler();
            //ViewBag.districtName = DB.bankDistrictInfo(bankName).ToList();
            //OnlineBODetailModel district =new OnlineBODetailModel();
            //district.bankdistrictlist = DB.bankDistrictInfo(bankName).ToList();
            return Json(DB.branchDetail(branchName).ToList());
        }

        [HttpPost]
        public ActionResult CreateUser(UserModel bomodel)
        {
            return View();
        }

	}
}