using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ONLINEBO.Models;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using ONLINEBO.Domain;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Collections.Specialized;

namespace ONLINEBO.Controllers
{
    public class AgentController : Controller
    {

        private readonly string storeID = "royalcapitalbdlive";
        private readonly string storePassword = "5EC10D5D9D82013542";

        private readonly string totalAmount = "550";



        //
        // GET: /Agent/
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult CreateNewBO()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateNewBO(HomeModel bomodel)
        {
            if (Session["UESRNAME"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();
            SqlConnection oConnection = new SqlConnection(SqlConn);

            try
            {

                var trackingno = "";


                oConnection.Open();


                string query = "SELECT CONVERT(varchar,(CASE WHEN MAX(SL) IS null THEN 0 ELSE MAX(SL) END)+1)+REPLACE(CONVERT(varchar,GETDATE(),101),'/','') Tmax FROM T_ONLINE_BO_REG";
                SqlCommand oCommand = new SqlCommand(query, oConnection);


                DataTable oTable = new DataTable();
                oTable.Load(oCommand.ExecuteReader());

                ViewBag.CreateToken = "Failed to Create Token For Create New BO!!!";

                foreach (DataRow dr in oTable.Rows)
                {
                    trackingno = "BOA" + dr["Tmax"].ToString();
                    Session["Trackingno"] = trackingno;


                    query = "INSERT INTO T_ONLINE_BO_REG([TRACKINGNO],[FIRSTNAME],[LASTNAME],[EMAIL],[MOBILE],[PASS],[USERID],[pDATE],[IsActive],[IsApp1],[IsApp2]) VALUES(@trackingno,@FIRSTNAME,@LASTNAME,@EMAIL,@MOBILE,@PASS,@USERID,GETDATE(),1,0,0);INSERT INTO T_ONLINE_BO_AccountHolder(TRACKINGNO,fFirstName,fLastName,fMobile,fEmail) VALUES(@trackingno,@FIRSTNAME,@LASTNAME,@MOBILE,@EMAIL);INSERT INTO T_ONLINE_BO_AccHolderBANK(TRACKINGNO) VALUES(@trackingno);INSERT INTO T_ONLINE_BO_AccHolderAuthorize(TRACKINGNO) VALUES(@trackingno);INSERT INTO T_ONLINE_BO_AccHolderNominee(TRACKINGNO) VALUES(@trackingno);INSERT INTO T_ONLINE_BO_AccHolderImages(TRACKINGNO) VALUES(@trackingno)";

                    oCommand = new SqlCommand(query, oConnection);

                    oCommand.Parameters.AddWithValue("@trackingno", trackingno);
                    oCommand.Parameters.AddWithValue("@FIRSTNAME", bomodel.FirstName);
                    oCommand.Parameters.AddWithValue("@LASTNAME", bomodel.LastName);
                    oCommand.Parameters.AddWithValue("@EMAIL", bomodel.Email);
                    oCommand.Parameters.AddWithValue("@MOBILE", bomodel.Mobile);
                    oCommand.Parameters.AddWithValue("@PASS", "@@@@@@");
                    oCommand.Parameters.AddWithValue("@USERID", @Session["USERID"].ToString());

                    int res = oCommand.ExecuteNonQuery();

                    if (res > 0)
                    {
                        oConnection.Close();
                        return RedirectToAction("AccountHolder", "Agent", new { id = trackingno });
                    }
                    else
                    {
                        oConnection.Close();
                        ViewBag.CreateToken = "Failed to Create Token For Create New BO!!!";
                    }
                }
            }
            catch
            {
                oConnection.Close();
                ViewBag.CreateToken = "Failed to Create Token For Create New BO!!!";
            }

            oConnection.Close();


            return View();
        }

        public ActionResult AccountHolder()
        {
            var id = Session["Trackingno"].ToString();

            BranchDBHandler BDB = new BranchDBHandler();
            ViewData["AccHolderSelect"] = BDB.AccHolderSelect(id).ToList();

            OtherDBHandler DB1 = new OtherDBHandler();
            ViewBag.Title = DB1.GetTitle().ToList();

            return View();
        }

        [HttpPost]
        public ActionResult AccountHolder(OnlineBODetailModel bomodel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bomodel.DesireBranch = @Session["BRANCHNAME"].ToString();
                    bomodel.TRACKINGNO = @Session["Trackingno"].ToString();

                    BranchDBHandler DB = new BranchDBHandler();

                    if (DB.AddAccHolder(bomodel))
                    {
                        Session["MessageAcc1"] = "Account Holder Information Successfully Updated.";

                        BranchDBHandler BDB = new BranchDBHandler();
                        ViewData["AccHolderSelect"] = BDB.AccHolderSelect(Session["Trackingno"].ToString()).ToList();

                        OtherDBHandler DB1 = new OtherDBHandler();
                        ViewBag.Title = DB1.GetTitle().ToList();

                        ModelState.Clear();
                        return RedirectToAction("BankInfo/0", "Agent");
                    }
                    else
                        Session["MessageAcc1"] = "Account Holder Information Updated Failed!!!";
                }
                //else
                //    ViewBag.MessageNominee2 = ModelState.

                return View();
                //return RedirectToAction("BankInfo", "Client");
            }
            catch
            {
                Session["MessageAcc1"] = "DataBase Error...Account Holder Information Updated Failed!!!";
                return View();
            }
        }

        public ActionResult AccountHolderInfoRecv(string id)
        {
            //if (Session["trackingno"] != null)
            //{
            Session["Trackingno"] = id;

            //}
            //else
            return RedirectToAction("AccountHolder", "Agent");

        }

        public ActionResult BankInfo(int id)
        {
            //if (Session["trackingno"] != null)
            //{

            if (id == 1)
            {
                BranchDBHandler DB = new BranchDBHandler();
                ViewBag.bankname = DB.defaultVAlue().ToList();
                ViewBag.districtName = DB.defaultVAlue().ToList();
                ViewBag.branchName = DB.defaultVAlue().ToList();
                ViewBag.routingNUmber = DB.defaultVAlue().ToList();
                //ViewBag.otherBankInfo = DB.otherBankInfo().ToList();


                return View();
            }

            string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();

            int IsbankExsists = 0;
            if (Session["trackingno"] != null)
            {
                var bankinfo = new List<BankInfoModel>();

                SqlConnection oConnection = new SqlConnection(SqlConn);
                oConnection.Open();
                string query = "SELECT [BANKNAME],[BANKBRANCH],[DISTRICT],[ROUTING],[AC] FROM T_ONLINE_BO_AccHolderBANK WHERE [TRACKINGNO]='" + @Session["trackingno"].ToString() + "'";

                SqlCommand oCommand = new SqlCommand(query, oConnection);

                DataTable oTable = new DataTable();
                oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));
                foreach (DataRow dr in oTable.Rows)
                {
                    if (dr["BANKNAME"].ToString() != "")
                        IsbankExsists = 1;

                }

                if (IsbankExsists == 1)
                {
                    return RedirectToAction("BankInfoEdit");
                }
                else
                {
                    IsbankExsists = 0;

                    BranchDBHandler DB = new BranchDBHandler();
                    ViewBag.bankname = DB.defaultVAlue().ToList();
                    ViewBag.districtName = DB.defaultVAlue().ToList();
                    ViewBag.branchName = DB.defaultVAlue().ToList();
                    ViewBag.routingNUmber = DB.defaultVAlue().ToList();

                    return View();
                }

            }

            //BranchDBHandler DB = new BranchDBHandler();
            //ViewBag.bankname = DB.defaultVAlue().ToList();
            //ViewBag.districtName = DB.defaultVAlue().ToList();
            //ViewBag.branchName = DB.defaultVAlue().ToList();
            //ViewBag.routingNUmber = DB.defaultVAlue().ToList();
            ////ViewBag.otherBankInfo = DB.otherBankInfo().ToList();
            //IsbankExsists = 0;

            return View();
            //}
            //else
            //    return RedirectToAction("index", "Home");
        }

        [HttpPost]
        public ActionResult BankInfo(BankInfoModel bomodel)
        {


            try
            {
                if (ModelState.IsValid)
                {
                    BranchDBHandler DB = new BranchDBHandler();
                    bomodel.TrackingNo = Session["trackingno"].ToString();

                    if (DB.AddBankDetail(bomodel))
                    {
                        Session["MessageBank"] = "Your Bank Information Successfully Inserted.";
                        ModelState.Clear();
                    }
                    else
                        Session["MessageBank"] = "Your Bank Information Insertion Failed!!!";
                }
                //else
                //    ViewBag.MessageNominee2 = ModelState.

                //return View();
                return RedirectToAction("UploadImages", "Agent");
            }
            catch
            {
                Session["MessageBank"] = "DataBase Error...Your Bank Information Insertion Failed!!!";
                return View();
            }
        }

        public ActionResult BankInfoEdit()
        {
            string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();

            if (Session["trackingno"] != null)
            {
                var bankinfo = new List<BankInfoModel>();

                SqlConnection oConnection = new SqlConnection(SqlConn);
                oConnection.Open();
                string query = "SELECT [BANKNAME],[BANKBRANCH],[DISTRICT],[ROUTING],[AC] FROM T_ONLINE_BO_AccHolderBANK WHERE [TRACKINGNO]='" + @Session["trackingno"].ToString() + "'";

                SqlCommand oCommand = new SqlCommand(query, oConnection);

                DataTable oTable = new DataTable();
                oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));
                foreach (DataRow dr in oTable.Rows)
                {
                    bankinfo.Add(new BankInfoModel { BANKNAME = dr["BANKNAME"].ToString(), BANKBRANCH = dr["BANKBRANCH"].ToString(), BANKDISTRICT = dr["DISTRICT"].ToString(), BANKROUTING = dr["ROUTING"].ToString(), BANKAC = dr["AC"].ToString() });
                }

                ViewData["bankinfo1"] = bankinfo.ToList();
            }

            return View();
        }

        [HttpPost]
        public ActionResult BankInfoEdit(BankInfoModel bomodel)
        {
            return View();
        }


        [HttpPost]
        public JsonResult GetBankName()
        {
            BranchDBHandler DB = new BranchDBHandler();
            //ViewBag.districtName = DB.bankDistrictInfo(bankName).ToList();
            //OnlineBODetailModel district =new OnlineBODetailModel();
            //district.bankdistrictlist = DB.bankDistrictInfo(bankName).ToList();
            return Json(DB.bankNameInfo().ToList());
        }

        [HttpPost]
        public JsonResult GetBankDistrict(string bankName)
        {
            BranchDBHandler DB = new BranchDBHandler();
            //ViewBag.districtName = DB.bankDistrictInfo(bankName).ToList();
            //OnlineBODetailModel district =new OnlineBODetailModel();
            //district.bankdistrictlist = DB.bankDistrictInfo(bankName).ToList();
            return Json(DB.bankDistrictInfo(bankName).ToList());
        }

        [HttpPost]
        public JsonResult GetBankBranch(string bankName, string districtName)
        {
            BranchDBHandler DB = new BranchDBHandler();
            //ViewBag.branchName = DB.bankbranchInfo(bankName, districtName).ToList();
            //OnlineBODetailModel district =new OnlineBODetailModel();
            //district.bankdistrictlist = DB.bankDistrictInfo(bankName).ToList();
            return Json(DB.bankbranchInfo(bankName, districtName).ToList());
        }
        [HttpPost]
        public JsonResult GetBankRouting(string bankName, string districtName, string bankBranch)
        {

            BranchDBHandler DB = new BranchDBHandler();
            //ViewBag.routingNUmber = DB.bankroutingInfo(bankName, districtName, bankBranch);
            //OnlineBODetailModel district =new OnlineBODetailModel();
            //district.bankdistrictlist = DB.bankroutingInfo(bankName, districtName, bankBranch).ToList();
            return Json(DB.bankroutingInfo(bankName, districtName, bankBranch).ToList());
        }

        public ActionResult AuthorizeInfo()
        {

            BranchDBHandler BDB = new BranchDBHandler();
            ViewData["authoSelect"] = BDB.AccAuInfo(@Session["Trackingno"].ToString()).ToList();

            return View();
        }
        [HttpPost]
        public ActionResult AuthorizeInfo(OnlineBODetailModel bomodel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    BranchDBHandler DB = new BranchDBHandler();
                    bomodel.TRACKINGNO = @Session["trackingno"].ToString();

                    if (DB.AddAuthorize(bomodel))
                    {
                        ViewBag.MessageAutho1 = "Data Successfully Inserted.";
                        ModelState.Clear();

                        return RedirectToAction("NomineeInfo", "Agent");
                    }
                    else
                        ViewBag.MessageAutho1 = "Insertion Failed!!!";
                }
                //else
                //    ViewBag.MessageNominee2 = ModelState.

                return View();
                //return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.MessageAutho1 = "DataBase Error...Insertion Failed!!!";
                return View();
            }
        }

        public ActionResult NomineeInfo()
        {

            BranchDBHandler BDB = new BranchDBHandler();
            //ViewData["NomineeSelect"] = BDB.AccNoInfo(Session["Trackingno"].ToString()).ToList();
            ViewData["NomineeSelect"] = BDB.AccNoInfo(@Session["trackingno"].ToString()).ToList();

            //OtherDBHandler DB1 = new OtherDBHandler();
            //ViewBag.Title = DB1.GetTitle().ToList();

            return View();

        }
        [HttpPost]
        public ActionResult NomineeInfo(OnlineBODetailModel bomodel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bomodel.TRACKINGNO = @Session["trackingno"].ToString();

                    BranchDBHandler DB = new BranchDBHandler();

                    if (DB.AddNominee(bomodel))
                    {
                        //BranchDBHandler BDB = new BranchDBHandler();
                        //ViewData["NomineeSelect"] = BDB.AccNoInfo(bomodel.TRACKINGNO.ToString()).ToList();

                        ViewBag.MessageNominee1 = "Data Successfully Inserted.";
                        ModelState.Clear();


                    }
                    else
                        ViewBag.MessageNominee2 = "Insertion Failed!!!";
                }
                //else
                //    ViewBag.MessageNominee2 = ModelState.

                return RedirectToAction("UploadImages", "Agent");
                //return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.MessageNominee2 = "DataBase Error...Insertion Failed!!!";
                return View();
            }
        }


        public ActionResult IncompleteBOAccount()
        {
            string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();

            var incompleteList = new List<OnlineBODetailModel>();
            SqlConnection oConnection = new SqlConnection(SqlConn);
            oConnection.Open();
            string query = "SELECT [TRACKINGNO],[FIRSTNAME],[LASTNAME],[EMAIL],[MOBILE] FROM T_ONLINE_BO_REG WHERE USERID='" + @Session["USERID"].ToString() + "' AND IsApp1=0";

            SqlCommand oCommand = new SqlCommand(query, oConnection);

            DataTable oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

            foreach (DataRow dr in oTable.Rows)
            {
                incompleteList.Add(new OnlineBODetailModel { TRACKINGNO = dr["TRACKINGNO"].ToString(), fFirstName = dr["FIRSTNAME"].ToString(), fLastName = dr["LASTNAME"].ToString(), fEmail = dr["EMAIL"].ToString(), fMobile = dr["MOBILE"].ToString() });
            }

            ViewData["incompleteList"] = incompleteList.ToList();

            return View();
        }

        [HttpPost]
        public ActionResult IncompleteBOAccount(string id)
        {

            return View();
        }


        public ActionResult completeBOAccount()
        {
            string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();

            var incompleteList = new List<OnlineBODetailModel>();
            SqlConnection oConnection = new SqlConnection(SqlConn);
            oConnection.Open();
            string query = "SELECT [TRACKINGNO],[FIRSTNAME],[LASTNAME],[EMAIL],[MOBILE] FROM T_ONLINE_BO_REG WHERE USERID='" + @Session["USERID"].ToString() + "' AND IsApp1=1";

            SqlCommand oCommand = new SqlCommand(query, oConnection);

            DataTable oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

            foreach (DataRow dr in oTable.Rows)
            {
                incompleteList.Add(new OnlineBODetailModel { TRACKINGNO = dr["TRACKINGNO"].ToString(), fFirstName = dr["FIRSTNAME"].ToString(), fLastName = dr["LASTNAME"].ToString(), fEmail = dr["EMAIL"].ToString(), fMobile = dr["MOBILE"].ToString() });
            }

            ViewData["completeList"] = incompleteList.ToList();

            return View();
        }

        [HttpPost]
        public ActionResult completeBOAccount(string id)
        {

            return View();
        }


        public ActionResult DownloadBO(string id)
        {
            string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();

            SqlConnection oConnection = new SqlConnection(SqlConn);

            DataSetBO DS = new DataSetBO();
            SqlDataAdapter DA = new SqlDataAdapter();

            ///// Account Holder
            string query = "SELECT [TRACKINGNO],[RCODE],[BO],[ToC],[fFirstName],[fLastName],[fOccupation],[fDoB],[fTitle],[fFName],[fMName],[fAddress],[fCity],[fPostCode],[fDivision],[fCountry],[fMobile],[fTel],[fFax],[fEmail],[fNationality],[fNID],[fTIN],[fSex],[fResidency],[jTitle],[jFirstName],[jLastName],[jOccupation],[jDoB],[jFName],[jMName],[jAddress],[jCity],[jPostCode],[jDivision],[jCountry],[jMobile],[jTel],[jFax],[jEmail],[jNID],[DesireBranch],[IsDirector],[DirectorShare],[pDate],[fIsComplete] FROM [dbo].[T_ONLINE_BO_AccountHolder] WHERE TRACKINGNO='" + id + "'";
            DA = new SqlDataAdapter(query, oConnection);
            DA.Fill(DS, "ACHOLDER");

            /////  Bank information 
            query = "SELECT [TRACKINGNO],[BANKNAME],[BANKBRANCH],[DISTRICT],[ROUTING],[AC],[pDate],[bIsComplete] FROM [dbo].[T_ONLINE_BO_AccHolderBANK] WHERE TRACKINGNO='" + id + "'";
            DA = new SqlDataAdapter(query, oConnection);
            DA.Fill(DS, "ACBANK");

            //// Account Holder Authorize info
            query = "SELECT [TRACKINGNO],[aTitle],[aFirstName],[aLastName],[aOccupation],[aDoB],[aFName],[aMName],[aAddress],[aCity],[aPostCode],[aDivision],[aCountry],[aMobile],[aTel],[aFax],[aEmail],[aNID],[pDate] FROM [dbo].[T_ONLINE_BO_AccHolderAuthorize] WHERE TRACKINGNO='" + id + "'";
            DA = new SqlDataAdapter(query, oConnection);
            DA.Fill(DS, "ACAUT");

            ///////////  Account Holder Nominee
            query = "SELECT [TRACKINGNO],[n1Title],[n1FirstName],[n1LastName],[n1RelWithACHolder],[n1Percentage],[n1IsResident],[n1DoB],[n1NID],[n1Address],[n1City],[n1PostCode],[n1Division],[n1Country],[n1Mobile],[n1Tel],[n1Fax],[n1Email],[n2Title],[n2FirstName],[n2LastName],[n2RelWithACHolder],[n2Percentage],[n2IsResident],[n2DoB],[n2NID],[n2Address],[n2City],[n2PostCode],[n2Division],[n2Country],[n2Mobile],[n2Tel],[n2Fax],[n2Email],[g1Title],[g1FirstName],[g1LastName],[g1RelWithNominee],[g1DoBMinor],[g1MaturityDoM],[g1IsResident],[g1DoB],[g1NID],[g1Address],[g1City],[g1PostCode],[g1Division],[g1Country],[g1Mobile],[g1Tel],[g1Fax],[g1Email],[g2Title],[g2FirstName],[g2LastName],[g2RelWithNominee],[g2DoBMinor],[g2MaturityDoM],[g2IsResident],[g2DoB],[g2NID],[g2Address],[g2City],[g2PostCode],[g2Division],[g2Country],[g2Mobile],[g2Tel],[g2Fax],[g2Email],[pDate],[nIsComplete] FROM [dbo].[T_ONLINE_BO_AccHolderNominee] WHERE TRACKINGNO='" + id + "'";
            DA = new SqlDataAdapter(query, oConnection);
            DA.Fill(DS, "ACNO");

            ///// Account Holder Image  ////
            query = "SELECT [TRACKINGNO],[fImage],[fNIDFont],[fNIDBack],[fSig],[jImage],[jNIDFont],[jNIDBack],[jSig],[aImage],[aNIDFont],[aNIDBack],[aSig],[n1Image],[n1NIDFont],[n1NIDBack],[n1Sig],[n2Image],[n2NIDFont],[n2NIDBack],[n2Sig],[g1Image],[g1NIDFont],[g1NIDBack],[g1Sig],[g2Image],[g2NIDFont],[g2NIDBack],[g2Sig],[pDate],[iIsComplete] FROM [dbo].[T_ONLINE_BO_AccHolderImages] WHERE TRACKINGNO='" + id + "'";
            DA = new SqlDataAdapter(query, oConnection);
            DA.Fill(DS, "ACIMG");


            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Report"), "cr_bo_book.rpt"));
            rd.SetDataSource(DS);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            //rd.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
            //rd.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(5, 5, 5, 5));
            //rd.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);


            return File(stream, "application/pdf", id + ".pdf");
        }



        public string Get_Latest_RCODE_BRANCH(string branch_name)
        {
            string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();
            string RCODE = "";

            try
            {

                SqlConnection oConnection = new SqlConnection(SqlConn);
                oConnection.Open();

                string queryMAX = "SELECT RCODE FROM T_NEW_CLIENT WHERE BRANCHCODE IN (SELECT TOP 1 [BRANCHCODE] FROM [T_BRANCH_LOGIN] WHERE [BRANCHNAME]='" + branch_name + "') AND EntryTime=(SELECT MAX(EntryTime) FROM T_NEW_CLIENT WHERE BRANCHCODE IN (SELECT TOP 1 [BRANCHCODE] FROM [T_BRANCH_LOGIN] WHERE [BRANCHNAME]='" + branch_name + "'))";

                if (branch_name.Contains("Corporate"))
                {
                    queryMAX = "SELECT RCODE FROM T_NEW_CLIENT WHERE BRANCHCODE IN (1,2) AND EntryTime=(SELECT MAX(EntryTime) FROM T_NEW_CLIENT WHERE BRANCHCODE IN (1,2))";
                }

                SqlCommand oCommandMAX = new SqlCommand(queryMAX, oConnection);

                DataTable oTableMAX = new DataTable();
                oTableMAX.Load(oCommandMAX.ExecuteReader(CommandBehavior.CloseConnection));

                bool new_branch = true;

                foreach (DataRow drMAX in oTableMAX.Rows)
                {
                    //flagRcode = 0;

                    if (!DBNull.Value.Equals(drMAX["RCODE"]))
                    {
                        string prfx = Regex.Replace(drMAX["RCODE"].ToString(), "[^A-Za-z]", "");
                        int sufx = Convert.ToInt32(Regex.Replace(drMAX["RCODE"].ToString(), "[A-Za-z]", "")) + 1;
                        RCODE = prfx + sufx.ToString();
                        //flagRcode = 1;
                        new_branch = false;
                    }
                }

                if (new_branch)
                {
                    oConnection.Open();
                    queryMAX = "SELECT BRANCH_PREFEX as perfex FROM T_BRANCHES WHERE IN (SELECT [BRANCHCODE] FROM [T_BRANCHES] WHERE [BRANCHNAME]='" + branch_name + "')";
                    oCommandMAX = new SqlCommand(queryMAX, oConnection);

                    oTableMAX = new DataTable();
                    oTableMAX.Load(oCommandMAX.ExecuteReader(CommandBehavior.CloseConnection));

                    foreach (DataRow drMAX in oTableMAX.Rows)
                    {

                        if (branch_name != null)
                        {
                            string prfx = drMAX["perfex"].ToString();
                            int sufx = 1;
                            RCODE = prfx + sufx.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return "";
            }


            return RCODE;


        }

        public string Get_Latest_RCODE_AGENT(string userID)
        {
            string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();
            string RCODE = "";

            try
            {

                SqlConnection oConnection = new SqlConnection(SqlConn);
                oConnection.Open();

                string queryMAX = "SELECT RCODE FROM T_NEW_CLIENT WHERE USER=@userid  AND EntryTime=(SELECT MAX(EntryTime) FROM T_NEW_CLIENT WHERE USER=@userid))";

                SqlCommand oCommandMAX = new SqlCommand(queryMAX, oConnection);
                oCommandMAX.Parameters.AddWithValue("@userid", userID);

                DataTable oTableMAX = new DataTable();
                oTableMAX.Load(oCommandMAX.ExecuteReader(CommandBehavior.CloseConnection));

                bool new_branch = true;

                foreach (DataRow drMAX in oTableMAX.Rows)
                {
                    //flagRcode = 0;

                    if (!DBNull.Value.Equals(drMAX["RCODE"]))
                    {
                        string prfx = Regex.Replace(drMAX["RCODE"].ToString(), "[^A-Za-z]", "");
                        int sufx = Convert.ToInt16(Regex.Replace(drMAX["RCODE"].ToString(), "[A-Za-z]", "")) + 1;
                        RCODE = prfx + sufx.ToString();
                        //flagRcode = 1;
                        new_branch = false;
                    }
                }

                if (new_branch)
                {
                    oConnection.Open();
                    queryMAX = "SELECT (BRANCH_PREFEX+AGENT_PREFIX) as perfex FROM T_BRANCH_LOGIN WHERE USERNAME=@username)";
                    oCommandMAX = new SqlCommand(queryMAX, oConnection);
                    oCommandMAX.Parameters.AddWithValue("@username", userID);

                    oTableMAX = new DataTable();
                    oTableMAX.Load(oCommandMAX.ExecuteReader(CommandBehavior.CloseConnection));

                    foreach (DataRow drMAX in oTableMAX.Rows)
                    {

                        if (userID != null)
                        {
                            string prfx = drMAX["perfex"].ToString();
                            int sufx = 1;
                            RCODE = prfx + sufx.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return "";
            }


            return RCODE;


        }


        public ActionResult Payment()
        {
            if (Session["Trackingno"] != null)
            {


                string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();
                SqlConnection oConnection = new SqlConnection(SqlConn);
                oConnection.Open();
                string qry = "SELECT * FROM [T_ONLINE_PAYMENT_RECEIVER] WHERE [tracking_no]='" + Session["trackingno"].ToString() + "'";
                SqlCommand oCommand = new SqlCommand(qry, oConnection);
                DataTable otable = new DataTable();
                otable.Load(oCommand.ExecuteReader());
                oConnection.Close();
                ViewBag.paid = "unpaid";
                foreach (DataRow dr in otable.Rows)
                {
                    ViewBag.paid = "paid";
                }

                return View();
            }
            else
                return RedirectToAction("index", "Branch");

            return View();
        }


             

        public ActionResult UploadImages()
        {
            if (Session["trackingno"] != null)
            {
                ViewData["image"] = GetImages().ToList(); ///////////////  Get All Stored Images

                string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();

                var incompleteList = new List<OnlineBODetailModel>();
                SqlConnection oConnection = new SqlConnection(SqlConn);
                oConnection.Open();
                string query = "SELECT TOP 1 fFirstName,jFirstName,aFirstName,n1FirstName,n2FirstName,g1FirstName,g2FirstName FROM T_ONLINE_BO_AccountHolder a,T_ONLINE_BO_AccHolderAuthorize b,T_ONLINE_BO_AccHolderNominee c where a.TRACKINGNO=@trackingno AND a.TRACKINGNO=@trackingno AND c.TRACKINGNO=@trackingno";

                SqlCommand oCommand = new SqlCommand(query, oConnection);
                oCommand.Parameters.AddWithValue("@trackingno", @Session["Trackingno"].ToString());

                DataTable oTable = new DataTable();
                oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

                foreach (DataRow dr in oTable.Rows)
                {
                    incompleteList.Add(new OnlineBODetailModel { fFirstName = dr["fFirstName"].ToString(), jFastName = dr["jFirstName"].ToString(), aFirstName = dr["aFirstName"].ToString(), n1FirstName = dr["n1FirstName"].ToString(), n2FirstName = dr["n2FirstName"].ToString(), g1FirstName = dr["g1FirstName"].ToString(), g2FirstName = dr["g2FirstName"].ToString() });
                }

                ViewData["printmr"] = incompleteList.ToList();

                return View();
            }
            else
                return RedirectToAction("index", "Home");
        }

        [HttpPost]
        public ActionResult UploadImages(BankInfoModel img, string id)
        {

            if (img.File.ContentLength == 0)
            {
                ViewData.Add(id, "Image Not Selected.");
                return View();
            }
            if (!(img.File.ContentType == "image/jpeg"))
            {
                ViewData.Add(id, "File type allowed : jpeg.");
                return View();
            }

            OtherDBHandler DB = new OtherDBHandler();
            SqlConnection oConnection = null;

            string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();
            //var path = img.File.p;
            System.Drawing.Image imag = System.Drawing.Image.FromStream(img.File.InputStream);

            try
            {
                try
                {
                    oConnection = new SqlConnection(SqlConn);

                    oConnection.Open();

                    SqlCommand oCommand = new SqlCommand("UPDATE [T_ONLINE_BO_AccHolderImages] SET " + id + "= @Pic,pDate=GETDATE(),iIsComplete=1 WHERE TRACKINGNO='" + (@Session["trackingno"].ToString()) + "'", oConnection);
                    //oCommand.Parameters.Add("Pic", SqlDbType.Image, 0).Value =ConvertImageToByteArray(imag, System.Drawing.Imaging.ImageFormat.Jpeg);
                    oCommand.Parameters.AddWithValue("Pic", DB.ConvertImageToByteArray(imag, System.Drawing.Imaging.ImageFormat.Jpeg));
                    int queryResult = oCommand.ExecuteNonQuery();
                    if (queryResult > 0)
                    {
                        ViewData.Add(id, "Successfully Uploaded.");


                        ViewData["image"] = GetImages().ToList(); ///////////////  Get All Stored Images

                        SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();

                        var incompleteList = new List<OnlineBODetailModel>();
                        oConnection = new SqlConnection(SqlConn);
                        oConnection.Open();
                        string query = "SELECT TOP 1 fFirstName,jFirstName,aFirstName,n1FirstName,n2FirstName,g1FirstName,g2FirstName FROM T_ONLINE_BO_AccountHolder a,T_ONLINE_BO_AccHolderAuthorize b,T_ONLINE_BO_AccHolderNominee c where a.TRACKINGNO=@trackingno AND a.TRACKINGNO=@trackingno AND c.TRACKINGNO=@trackingno";

                        oCommand = new SqlCommand(query, oConnection);
                        oCommand.Parameters.AddWithValue("@trackingno", @Session["Trackingno"].ToString());

                        DataTable oTable = new DataTable();
                        oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

                        foreach (DataRow dr in oTable.Rows)
                        {
                            incompleteList.Add(new OnlineBODetailModel { fFirstName = dr["fFirstName"].ToString(), jFastName = dr["jFirstName"].ToString(), aFirstName = dr["aFirstName"].ToString(), n1FirstName = dr["n1FirstName"].ToString(), n2FirstName = dr["n2FirstName"].ToString(), g1FirstName = dr["g1FirstName"].ToString(), g2FirstName = dr["g2FirstName"].ToString() });
                        }

                        ViewData["printmr"] = incompleteList.ToList();

                        return View("UploadImages");
                    }
                }
                catch (Exception ex)
                {
                    ViewData.Add(id, "Error: " + ex.Message);
                    ViewBag.ImgUpSuccess = "Error: " + ex.Message;
                }
            }
            finally
            {
                if (oConnection != null)
                    oConnection.Close();
            }

            return View("UploadImages");
        }

        private List<OnlineBODetailModel> GetImages()
        {
            string query = "SELECT * FROM T_ONLINE_BO_AccHolderImages WHERE TRACKINGNO='" + (@Session["trackingno"].ToString()) + "'";
            List<OnlineBODetailModel> images = new List<OnlineBODetailModel>();
            string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();
            using (SqlConnection con = new SqlConnection(SqlConn))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            if (!Convert.IsDBNull(sdr["def"]))
                            {
                                //                                                                                                                ],[],[],[],[],[],[],[],[],[],[],[],[],[],                                                                                                                                  [],[],[],[],                                                                                                                                                                                                                                                                                                         [],[],[],[],[],[],[],[],[pDate],[iIsComplete]
                                images.Add(new OnlineBODetailModel
                                {
                                    fImage = (sdr["fImage"] != DBNull.Value) ? (byte[])sdr["fImage"] : (byte[])sdr["def"],
                                    fNIDFont = (sdr["fNIDFont"] != DBNull.Value) ? (byte[])sdr["fNIDFont"] : (byte[])sdr["def"],
                                    fNIDBack = (sdr["fNIDBack"] != DBNull.Value) ? (byte[])sdr["fNIDBack"] : (byte[])sdr["def"],
                                    fSig = (sdr["fSig"] != DBNull.Value) ? (byte[])sdr["fSig"] : (byte[])sdr["def"],

                                    jImage = (sdr["jImage"] != DBNull.Value) ? (byte[])sdr["jImage"] : (byte[])sdr["def"],
                                    jNIDFont = (sdr["jNIDFont"] != DBNull.Value) ? (byte[])sdr["jNIDFont"] : (byte[])sdr["def"],
                                    jNIDBack = (sdr["jNIDBack"] != DBNull.Value) ? (byte[])sdr["jNIDBack"] : (byte[])sdr["def"],
                                    jSig = (sdr["jSig"] != DBNull.Value) ? (byte[])sdr["jSig"] : (byte[])sdr["def"],

                                    aImage = (sdr["aImage"] != DBNull.Value) ? (byte[])sdr["aImage"] : (byte[])sdr["def"],
                                    aNIDFont = (sdr["aNIDFont"] != DBNull.Value) ? (byte[])sdr["aNIDFont"] : (byte[])sdr["def"],
                                    aNIDBack = (sdr["aNIDBack"] != DBNull.Value) ? (byte[])sdr["aNIDBack"] : (byte[])sdr["def"],
                                    aSig = (sdr["aSig"] != DBNull.Value) ? (byte[])sdr["aSig"] : (byte[])sdr["def"],

                                    n1Image = (sdr["n1Image"] != DBNull.Value) ? (byte[])sdr["n1Image"] : (byte[])sdr["def"],
                                    n1NIDFont = (sdr["n1NIDFont"] != DBNull.Value) ? (byte[])sdr["n1NIDFont"] : (byte[])sdr["def"],
                                    n1NIDBack = (sdr["n1NIDBack"] != DBNull.Value) ? (byte[])sdr["n1NIDBack"] : (byte[])sdr["def"],
                                    n1Sig = (sdr["n1Sig"] != DBNull.Value) ? (byte[])sdr["n1Sig"] : (byte[])sdr["def"],


                                    n2Image = (sdr["n2Image"] != DBNull.Value) ? (byte[])sdr["n2Image"] : (byte[])sdr["def"],
                                    n2NIDFont = (sdr["n2NIDFont"] != DBNull.Value) ? (byte[])sdr["n2NIDFont"] : (byte[])sdr["def"],
                                    n2NIDBack = (sdr["n2NIDBack"] != DBNull.Value) ? (byte[])sdr["n2NIDBack"] : (byte[])sdr["def"],
                                    n2Sig = (sdr["n2Sig"] != DBNull.Value) ? (byte[])sdr["n2Sig"] : (byte[])sdr["def"],


                                    g1Image = (sdr["g1Image"] != DBNull.Value) ? (byte[])sdr["g1Image"] : (byte[])sdr["def"],
                                    g1NIDFont = (sdr["g1NIDFont"] != DBNull.Value) ? (byte[])sdr["g1NIDFont"] : (byte[])sdr["def"],
                                    g1NIDBack = (sdr["g1NIDBack"] != DBNull.Value) ? (byte[])sdr["g1NIDBack"] : (byte[])sdr["def"],
                                    g1Sig = (sdr["g1Sig"] != DBNull.Value) ? (byte[])sdr["g1Sig"] : (byte[])sdr["def"],

                                    g2Image = (sdr["g2Image"] != DBNull.Value) ? (byte[])sdr["g2Image"] : (byte[])sdr["def"],
                                    g2NIDFont = (sdr["g2NIDFont"] != DBNull.Value) ? (byte[])sdr["g2NIDFont"] : (byte[])sdr["def"],
                                    g2NIDBack = (sdr["g2NIDBack"] != DBNull.Value) ? (byte[])sdr["g2NIDBack"] : (byte[])sdr["def"],
                                    g2Sig = (sdr["g2Sig"] != DBNull.Value) ? (byte[])sdr["g2Sig"] : (byte[])sdr["def"]
                                });

                            }
                        }
                    }
                    con.Close();
                }

                return images;
            }
        }

        public ActionResult BOSaleReport()
        {

            string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();


            var bankinfo = new List<OnlineBODetailModel>();

            SqlConnection oConnection = new SqlConnection(SqlConn);
            oConnection.Open();
            string query = "SELECT TOP 10 b.TRACKINGNO,a.NAME,a.AMOUNT,a.[DATE],b.USERID FROM T_BO_CHARGE a,T_ONLINE_BO_REG b WHERE a.MR_NO=b.TRACKINGNO AND b.USERID='" + @Session["USERID"].ToString() + "' ORDER BY a.[DATE] DESC ";

            SqlCommand oCommand = new SqlCommand(query, oConnection);

            DataTable oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));
            foreach (DataRow dr in oTable.Rows)
            {
                bankinfo.Add(new OnlineBODetailModel { TRACKINGNO = dr["TRACKINGNO"].ToString(), aFirstName = dr["NAME"].ToString(), amount = dr["AMOUNT"].ToString(), DATE = dr["DATE"].ToString() });
            }

            ViewData["bosale"] = bankinfo.ToList();


            return View();
        }

        [HttpPost]
        public ActionResult BOSaleReport(OnlineBODetailModel bomodel)
        {
            string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();


            var bankinfo = new List<OnlineBODetailModel>();

            SqlConnection oConnection = new SqlConnection(SqlConn);
            oConnection.Open();
            string query = "SELECT b.TRACKINGNO,a.NAME,a.AMOUNT,a.[DATE],b.USERID FROM T_BO_CHARGE a,T_ONLINE_BO_REG b WHERE a.MR_NO=b.TRACKINGNO AND b.USERID='" + @Session["USERID"].ToString() + "' AND a.[DATE] BETWEEN '" + bomodel.fDoB + "' AND '" + bomodel.jDoB + "' ";

            SqlCommand oCommand = new SqlCommand(query, oConnection);

            DataTable oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));
            foreach (DataRow dr in oTable.Rows)
            {
                bankinfo.Add(new OnlineBODetailModel { TRACKINGNO = dr["TRACKINGNO"].ToString(), aFirstName = dr["NAME"].ToString(), amount = dr["AMOUNT"].ToString(), DATE = dr["DATE"].ToString() });
            }

            ViewData["bosale"] = bankinfo.ToList();

            return View();
        }



        public ActionResult CDBLReport()
        {

            string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();


            var bankinfo = new List<OnlineBODetailModel>();

            SqlConnection oConnection = new SqlConnection(SqlConn);
            oConnection.Open();
            string query = "SELECT TOP 10 b.TRACKINGNO,a.NAME,SUM(a.AMOUNT) AMOUNT,a.[DATE],b.USERID FROM T_CDBL_CHARGE a,T_ONLINE_BO_REG b WHERE a.MR_NO=b.TRACKINGNO AND b.USERID='" + @Session["USERID"].ToString() + "' GROUP BY b.TRACKINGNO,a.NAME,a.[DATE],b.USERID ORDER BY a.[DATE] DESC ";

            SqlCommand oCommand = new SqlCommand(query, oConnection);

            DataTable oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));
            foreach (DataRow dr in oTable.Rows)
            {
                bankinfo.Add(new OnlineBODetailModel { TRACKINGNO = dr["TRACKINGNO"].ToString(), aFirstName = dr["NAME"].ToString(), amount = dr["AMOUNT"].ToString(), DATE = dr["DATE"].ToString() });
            }

            ViewData["bosale"] = bankinfo.ToList();


            return View();
        }

        [HttpPost]
        public ActionResult CDBLReport(OnlineBODetailModel bomodel)
        {
            string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();


            var bankinfo = new List<OnlineBODetailModel>();

            SqlConnection oConnection = new SqlConnection(SqlConn);
            oConnection.Open();

            string query = "SELECT TOP 10 b.TRACKINGNO,a.NAME,SUM(a.AMOUNT) AMOUNT,a.[DATE],b.USERID FROM T_CDBL_CHARGE a,T_ONLINE_BO_REG b WHERE a.MR_NO=b.TRACKINGNO AND b.USERID='" + @Session["USERID"].ToString() + "' AND a.[DATE] BETWEEN '" + bomodel.fDoB + "' AND '" + bomodel.jDoB + "' GROUP BY b.TRACKINGNO,a.NAME,a.[DATE],b.USERID ORDER BY a.[DATE] DESC ";

            SqlCommand oCommand = new SqlCommand(query, oConnection);

            DataTable oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));
            foreach (DataRow dr in oTable.Rows)
            {
                bankinfo.Add(new OnlineBODetailModel { TRACKINGNO = dr["TRACKINGNO"].ToString(), aFirstName = dr["NAME"].ToString(), amount = dr["AMOUNT"].ToString(), DATE = dr["DATE"].ToString() });
            }

            ViewData["bosale"] = bankinfo.ToList();

            return View();
        }


        public ActionResult Search()
        {
            string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();

            var incompleteList = new List<OnlineBODetailModel>();
            SqlConnection oConnection = new SqlConnection(SqlConn);
            oConnection.Open();
            string query = "SELECT [TRACKINGNO],[FIRSTNAME],[LASTNAME],[EMAIL],[MOBILE] FROM T_ONLINE_BO_REG WHERE USERID='' AND IsApp1=1";

            SqlCommand oCommand = new SqlCommand(query, oConnection);

            DataTable oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

            foreach (DataRow dr in oTable.Rows)
            {
                incompleteList.Add(new OnlineBODetailModel { TRACKINGNO = dr["TRACKINGNO"].ToString(), fFirstName = dr["FIRSTNAME"].ToString(), fLastName = dr["LASTNAME"].ToString(), fEmail = dr["EMAIL"].ToString(), fMobile = dr["MOBILE"].ToString() });
            }

            ViewData["search"] = incompleteList.ToList();

            return View();
        }

        [HttpPost]
        public ActionResult Search(OnlineBODetailModel bomdel)
        {
            string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();

            var incompleteList = new List<OnlineBODetailModel>();
            SqlConnection oConnection = new SqlConnection(SqlConn);
            oConnection.Open();
            string query = "SELECT [TRACKINGNO],[FIRSTNAME],[LASTNAME],[EMAIL],[MOBILE] FROM T_ONLINE_BO_REG WHERE TRACKINGNO LIKE '%" + bomdel.aFirstName + "%'";

            SqlCommand oCommand = new SqlCommand(query, oConnection);
            //oCommand.Parameters.AddWithValue("@trackingno",bomdel.aFirstName);

            DataTable oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

            foreach (DataRow dr in oTable.Rows)
            {
                incompleteList.Add(new OnlineBODetailModel { TRACKINGNO = dr["TRACKINGNO"].ToString(), fFirstName = dr["FIRSTNAME"].ToString(), fLastName = dr["LASTNAME"].ToString(), fEmail = dr["EMAIL"].ToString(), fMobile = dr["MOBILE"].ToString() });
            }

            ViewData["search"] = incompleteList.ToList();
            return View();
        }



        /////integration of SSL Commerz payment gateway

        public JsonResult mycall()
        {
            //string baseUrl = @"{Request.Scheme}://{Request.Host}{Request.PathBase}/";
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";

            NameValueCollection PostData = new NameValueCollection
            {
                { "total_amount", totalAmount },
                { "currency", "BDT"},
                { "tran_id", GenerateUniqueId() },
                { "success_url", baseUrl + "Agent/PaymentGatewayCallback" },
                { "fail_url", baseUrl + "Agent/PaymentGatewayCallback" },
                { "cancel_url", baseUrl + "Agent/PaymentGatewayCallback" },
                { "cus_name", "parvej" },
                { "cus_email", "john.doe@mail.co" },
                { "cus_add1", "Address Line On" },
                { "cus_city", "Dhaka" },
                { "cus_postcode", "1219" },
                { "cus_country", "Bangladesh" },
                { "cus_phone", "1234" },
                { "shipping_method", "NO" },
                { "product_name", "UD" },
                { "product_category", "Service" },
                { "product_profile", "general" },
                { "value_a", Session["Trackingno"].ToString() },
                { "value_b", "BOA" },
                { "value_c", get_ip()+ ", Webpage" },
                { "value_d", "123" }
            };

            var sslcz = new SSLCommerz(storeID, storePassword);

            var response = sslcz.InitiateTransaction(PostData);
            var model = new { status = "success", data = response.GatewayPageURL, logo = response.storeLogo };
            var json_val = Json(model, JsonRequestBehavior.AllowGet);
            //Response.Write(json_val.ToString());
            return Json(model, JsonRequestBehavior.AllowGet);

            //return  json_encode(['status' => 'success', 'data' => $sslcz['GatewayPageURL'], 'logo' => $sslcz['storeLogo'] ]);



        }



        private string GenerateUniqueId()
        {
            long i = 1;

            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= (b + 1);
            }

            return string.Format("{0:x}", i - DateTime.Now.Ticks).ToUpper();
        }


        [HttpPost]
        public ActionResult PaymentGatewayCallback(ONLINEBO.SSLCommerz.SSLCommerzValidatorResponse response)
        {
            if (!string.IsNullOrEmpty(response.status) && response.status == "VALID")
            {
                //WebApplication1.SSLCommerz sslcz = new WebApplication1.SSLCommerz(storeID, storePassword, true);

                //if (WebApplication1.SSLCommerz.OrderValidate(response.tran_id, totalAmount, "BDT", Request))
                //{
                //    return View("Success", GetProperties(response));
                //}

                string qry = " INSERT INTO [T_ONLINE_PAYMENT_RECEIVER] ([tracking_no] ,[payment_id] ,[trxID] ,[amount] ,[create_date] ,[status] ,[source])  VALUES (@tracking_no, @payment_id, @trxID, @amount, @create_date, @status,@source)";
                SqlConnection oConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["RCLWEB"].ToString());
                SqlCommand command = new SqlCommand(qry, oConnection);
                command.Parameters.Add("@tracking_no", response.value_a);
                command.Parameters.Add("@payment_id", response.bank_tran_id);
                command.Parameters.Add("@trxID", response.tran_id);
                command.Parameters.Add("@amount", response.store_amount);
                command.Parameters.Add("@create_date", response.tran_date);
                command.Parameters.Add("@status", "success");
                command.Parameters.Add("@source", "SSLCOMMERZ");

                oConnection.Open();
                command.ExecuteNonQuery();
                oConnection.Close();

                return View("Success_SSL", GetProperties(response));
            }

            if (!string.IsNullOrEmpty(response.status) && response.status == "FAILED")
            {
                return View("Fail", GetProperties(response));
            }

            if (!string.IsNullOrEmpty(response.status) && response.status == "CANCELLED")
            {
                return View("Cancel", GetProperties(response));
            }

            return View("Error", GetProperties(response));
        }

        private static Dictionary<string, string> GetProperties(object obj)
        {
            var props = new Dictionary<string, string>();
            if (obj == null)
                return props;

            var type = obj.GetType();
            foreach (var prop in type.GetProperties())
            {
                var val = prop.GetValue(obj, new object[] { });

                if (val != null)
                {
                    props.Add(prop.Name, val.ToString());
                }
            }

            return props;
        }

        private string get_ip()
        {



            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
                //ip = System.Web.HttpContext.Current.Request.UserHostAddress;
            }



            return ip;
        }


        ///end of SSL Commerz payment gateway



        public ActionResult agent_ledger()
        {
            return View();
        }

        public ActionResult fund_withdraw_request()
        {
            return View();
        }

        public ActionResult bo_revenue()
        {
            return View();
        }

        public ActionResult share_revenue()
        {
            return View();
        }




	}///end of the classs
}