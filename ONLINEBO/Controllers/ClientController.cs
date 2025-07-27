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
using Windows.UI.Xaml.Media.Imaging;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using CrystalDecisions.Shared;

namespace ONLINEBO.Controllers
{
    public class ClientController : Controller
    {
        //
        // GET: /Client/


        //private readonly string storeID = "royal5ea12c33f3985";
        //private readonly string storePassword = "royal5ea12c33f3985@ssl";

        private readonly string storeID = "royalcapitalbdlive";
        private readonly string storePassword = "5EC10D5D9D82013542";
        
        string totalAmount = "450";

        public ActionResult Index()
        {
            if (Session["trackingno"] != null)
            {
                OnlineBODetailModel bo = new OnlineBODetailModel();
                bo.TRACKINGNO = @Session["trackingno"].ToString();

                BankInfoModel bo1 = new BankInfoModel();
                bo1.TrackingNo = @Session["trackingno"].ToString();


                OnlineBoDetailDBHandle DB = new OnlineBoDetailDBHandle();

                ViewData["Acc"] = DB.AccHolderSelect(bo.TRACKINGNO.ToString()).ToList();

                ViewData["AccBank"] = DB.AccBankInfo(bo1.TrackingNo.ToString()).ToList();

                ViewData["AccAu"] = DB.AccAuInfo(bo.TRACKINGNO.ToString()).ToList();

                ViewData["AccNo"] = DB.AccNoInfo(bo.TRACKINGNO.ToString()).ToList();

                ViewData["image"] = GetImages().ToList();

                return View();
            }
            else
                return RedirectToAction("index","Home");
        }

        public ActionResult Logout()
        {
            if (Session["trackingno"] != null)
            {
                Session["trackingno"] = null;

                return RedirectToAction("index", "Home");
            }
            if (Session["UESRNAME"] != null)
            {
                    Session["UESRNAME"]= null;
                    Session["USERID"]= null;
                    Session["USERTYPE"]= null;
                    Session["BRANCHNAME"] = null;

                return RedirectToAction("index", "Home");
            }
            else
                return View();

        }


        private List<OnlineBODetailModel> GetImagesForEkyc()
        {
            string query = "SELECT * FROM T_ONLINE_BO_AccHolderImages WHERE TRACKINGNO='" + @Session["trackingno"].ToString() + "'";
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
                                });

                            }
                        }
                    }
                    con.Close();
                }

                return images;
            }
        }


        private List<OnlineBODetailModel> GetImages()
        {
            string query = "SELECT * FROM T_ONLINE_BO_AccHolderImages WHERE TRACKINGNO='" + @Session["trackingno"].ToString() + "'";
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

        //public ActionResult AccHolder()
        //{
        //    OnlineBoDetailDBHandle DB = new OnlineBoDetailDBHandle();
        //    ViewData["Acc"] = DB.AccInfo().ToList();

        //    ViewData["AccBank"] = DB.AccBankInfo().ToList();

        //    ViewData["AccAu"] = DB.AccAuInfo().ToList();

        //    ViewData["AccNo"] = DB.AccAuInfo().ToList();

        //    return View();
        //}



        public ActionResult AccountHolder()
        {
            if (Session["trackingno"] != null)
            {
                OtherDBHandler DB = new OtherDBHandler();

                SelectList branch = new SelectList(DB.branchInfo().ToList(), "BRANCHNAME", "BRANCHNAME");

                ViewBag.BRANCHES = branch;

                var id = Session["Trackingno"].ToString();

                OnlineBoDetailDBHandle BDB = new OnlineBoDetailDBHandle();
                ViewData["AccHolderSelect"] = BDB.AccHolderSelect(id).ToList();

                OtherDBHandler DB1 = new OtherDBHandler();
                ViewBag.Title = DB1.GetTitle().ToList();

                return View();
            }
            else
                return RedirectToAction("index", "Home");
        }
        [HttpPost]
        public ActionResult AccountHolder(OnlineBODetailModel bomodel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    OnlineBoDetailDBHandle DB = new OnlineBoDetailDBHandle();
                    bomodel.TRACKINGNO = @Session["Trackingno"].ToString();

                    if (DB.AddAccHolder(bomodel))
                    {
                        Session["MessageAcc1"] = "Account Holder Information Successfully Updated.";

                        OnlineBoDetailDBHandle BDB = new OnlineBoDetailDBHandle();
                        ViewData["AccHolderSelect"] = BDB.AccHolderSelect(Session["Trackingno"].ToString()).ToList();

                        OtherDBHandler DB1 = new OtherDBHandler();
                        ViewBag.Title = DB1.GetTitle().ToList();

                        ModelState.Clear();

                        ParGlobalClass.LastUpdateDate(@Session["Trackingno"].ToString());

                        return RedirectToAction("BankInfo/0", "Client");
                    }
                    else
                    {
                        ViewBag.MessageAcc= "Account Holder Information Insertion Failed!!!";
                        return View();
                    }
                      //  Session["MessageAcc1"] = "Account Holder Information Insertion Failed!!!";

                }
                //else
                //    ViewBag.MessageNominee2 = ModelState.

                //return View();
                // Session["MessageAcc1"] = "Account Holder Information Insertion Failed!!!";
                ViewBag.MessageAcc = "Account Holder Information Insertion Failed!!!";
                return View();
                //return RedirectToAction("BankInfo/0","Client");
            }
            catch
            {
              //  Session["MessageAcc1"] = "DataBase Error...Account Holder Information Insertion Failed!!!";
                ViewBag.MessageAcc= "DataBase Error...Account Holder Information Insertion Failed!!!";
                return View();
            }
        }
        public ActionResult AuthorizeInfo()
        {
            OnlineBoDetailDBHandle BDB = new OnlineBoDetailDBHandle();
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
                    OnlineBoDetailDBHandle DB = new OnlineBoDetailDBHandle();
                    bomodel.TRACKINGNO = @Session["trackingno"].ToString();

                    if (DB.AddAuthorize(bomodel))
                    {
                        Session["MessageAuth"] = "Authorize Info Successfully Inserted.";
                        ModelState.Clear();
                        ParGlobalClass.LastUpdateDate(@Session["Trackingno"].ToString());
                        return RedirectToAction("NomineeInfo", "Client");
                    }
                    else
                    {
                        ViewBag.MessageAutho1 = "Insertion Failed!!!";
                        return View();
                    }
                       
                }
                //else
                //    ViewBag.MessageNominee2 = ModelState.
                ViewBag.MessageAutho1 = "Insertion Failed!!!";
                return View();
                //return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.MessageAutho1 = "DataBase Error...Insertion Failed!!!";
                return View();
            }
        }



        public ActionResult BankInfo(int id)
        {
            if (Session["trackingno"] != null)
            {
                if (id == 1)
                {
                    OnlineBoDetailDBHandle DB = new OnlineBoDetailDBHandle();
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
                        return RedirectToAction("BankInfoEdit", "Client");
                    }
                    else
                    {
                        IsbankExsists = 0;

                        OnlineBoDetailDBHandle DB = new OnlineBoDetailDBHandle();
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
            else
                return RedirectToAction("index", "Home");
        }

        [HttpPost]
        public ActionResult BankInfo(BankInfoModel bomodel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    OnlineBoDetailDBHandle DB = new OnlineBoDetailDBHandle();
                    bomodel.TrackingNo = Session["trackingno"].ToString();

                    if (DB.AddBankDetail(bomodel))
                    {
                        Session["MessageBank"] = "Your Bank Information Successfully Added.";
                        ModelState.Clear();
                        ParGlobalClass.LastUpdateDate(@Session["Trackingno"].ToString());
                        return RedirectToAction("AuthorizeInfo", "Client");
                    }
                    else
                    {
                       // Session["MessageBank"] = "Your Bank Information Insertion Failed!!!";
                        ViewBag.MessageBank= "Your Bank Information Insertion Failed!!!";
                        return View();
                    }
                       
                }
                //else
                //    ViewBag.MessageNominee2 = ModelState.
                ViewBag.MessageBank = "Your Bank Information Insertion Failed!!!";
                return View();
                //return RedirectToAction("AuthorizeInfo", "Client");
            }
            catch
            {
               // Session["MessageBank"] = "DataBase Error...Your Bank Information Insertion Failed!!!";
                ViewBag.MessageBank = "DataBase Error...Your Bank Information Insertion Failed!!!";
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

                ParGlobalClass.LastUpdateDate(@Session["Trackingno"].ToString());
            }

            return View();
        }

        [HttpPost]
        public JsonResult GetBankName()
        {
            OnlineBoDetailDBHandle DB = new OnlineBoDetailDBHandle();
            //ViewBag.districtName = DB.bankDistrictInfo(bankName).ToList();
            //OnlineBODetailModel district =new OnlineBODetailModel();
            //district.bankdistrictlist = DB.bankDistrictInfo(bankName).ToList();
            return Json(DB.bankNameInfo().ToList());
        }

        [HttpPost]
        public JsonResult GetBankDistrict(string bankName)
        {
            OnlineBoDetailDBHandle DB = new OnlineBoDetailDBHandle();
            //ViewBag.districtName = DB.bankDistrictInfo(bankName).ToList();
            //OnlineBODetailModel district =new OnlineBODetailModel();
            //district.bankdistrictlist = DB.bankDistrictInfo(bankName).ToList();
            return Json(DB.bankDistrictInfo(bankName).ToList());
        }

        [HttpPost]
        public JsonResult GetBankBranch(string bankName, string districtName)
        {
            OnlineBoDetailDBHandle DB = new OnlineBoDetailDBHandle();
            //ViewBag.branchName = DB.bankbranchInfo(bankName, districtName).ToList();
            //OnlineBODetailModel district =new OnlineBODetailModel();
            //district.bankdistrictlist = DB.bankDistrictInfo(bankName).ToList();
            return Json(DB.bankbranchInfo(bankName, districtName).ToList());
        }
        [HttpPost]
        public JsonResult GetBankRouting(string bankName, string districtName, string bankBranch)
        {

            OnlineBoDetailDBHandle DB = new OnlineBoDetailDBHandle();
            //ViewBag.routingNUmber = DB.bankroutingInfo(bankName, districtName, bankBranch);
            //OnlineBODetailModel district =new OnlineBODetailModel();
            //district.bankdistrictlist = DB.bankroutingInfo(bankName, districtName, bankBranch).ToList();
            return Json(DB.bankroutingInfo(bankName, districtName, bankBranch).ToList());
        }

        //[HttpPost]
        //public ActionResult GetBankDistrict(string bankName)
        //{
        //    //OtherDBHandler DB = new OtherDBHandler();
        //    //OnlineBODetailModel district = new OnlineBODetailModel();
        //    //district.bankdistrictlist = DB.bankDistrictInfo(bankName).ToList();
        //    //return View(district);

        //    OtherDBHandler DB = new OtherDBHandler();
        //    ViewBag.districtName = DB.bankDistrictInfo(bankName).ToList();
        //    return View();

        //    //OtherDBHandler DB = new OtherDBHandler();
        //    ////OnlineBODetailModel district =new OnlineBODetailModel();
        //    ////district.bankdistrictlist = DB.bankDistrictInfo(bankName).ToList();
        //    //return Json(DB.bankDistrictInfo(bankName).ToList(), JsonRequestBehavior.AllowGet);
        //}
        public ActionResult NomineeInfo()
        {
            if (Session["trackingno"] != null)
            {
                OnlineBoDetailDBHandle BDB = new OnlineBoDetailDBHandle();
                //ViewData["NomineeSelect"] = BDB.AccNoInfo(Session["Trackingno"].ToString()).ToList();
                ViewData["NomineeSelect"] = BDB.AccNoInfo(@Session["trackingno"].ToString()).ToList();

                return View();
            }
            else
                return RedirectToAction("index", "Home");
        }
        [HttpPost]
        public ActionResult NomineeInfo(OnlineBODetailModel bomodel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bomodel.TRACKINGNO = @Session["trackingno"].ToString();

                    OnlineBoDetailDBHandle DB = new OnlineBoDetailDBHandle();

                    if (DB.AddNominee(bomodel))
                    {
                        //BranchDBHandler BDB = new BranchDBHandler();
                        //ViewData["NomineeSelect"] = BDB.AccNoInfo(bomodel.TRACKINGNO.ToString()).ToList();

                        //ViewBag.MessageNominee1 = "Data Successfully Inserted.";
                        ModelState.Clear();
                        ParGlobalClass.LastUpdateDate(@Session["Trackingno"].ToString());

                        Session["MessageNominee"] = "Nominee Info successfully Inserted.";
                        return RedirectToAction("UploadImages", "Client");
                    }
                    else
                    {
                        ViewBag.MessageNominee2 = "Insertion Failed!!!";

                        return View();
                    }
                }
                //else
                //    ViewBag.MessageNominee2 = ModelState.

                ViewBag.MessageNominee2 = "Insertion Failed!!!";
                return View();
                //return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.MessageNominee2 = "DataBase Error...Insertion Failed!!!";
                return View();
            }
        }

        public ActionResult PaymentOption()
        {
            if (Session["trackingno"] != null)
            {

                //try
                //{
                //    string tag = Request.QueryString["tag"];
                //    string tracking_no = Request.QueryString["tracking_no"];
                //    string payment_id = Request.QueryString["payment_id"];
                //    string amount = Request.QueryString["amount"];
                //    string trxID = Request.QueryString["trxID"];
                //    if (tag == "success" )
                //    {
                //        string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();
                //        SqlConnection oConnection = new SqlConnection(SqlConn);
                //        oConnection.Open();
                //        string qry = "INSERT INTO T_ONLINE_PAYMENT_RECEIVER ([tracking_no] ,[payment_id] ,[trxID] ,[amount],[create_date] ,[status]) VALUES ('" + tracking_no + "', '" + payment_id + "', '" + trxID + "', '" + amount + "', GETDATE(), '" + tag + "')";
                //        SqlCommand oCommand = new SqlCommand(qry, oConnection);
                //        oCommand.ExecuteNonQuery();
                //        oConnection.Close();
                //    }
                //    else
                //    {
                //        string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();
                //        SqlConnection oConnection = new SqlConnection(SqlConn);
                //        oConnection.Open();
                //        string qry = "INSERT INTO T_ONLINE_PAYMENT_RECEIVER ([create_date] ,[status]) VALUES ( GETDATE(), '" + tag + "')";
                //        SqlCommand oCommand = new SqlCommand(qry, oConnection);
                //        oCommand.ExecuteNonQuery();
                //        oConnection.Close();

                //    }
                //}
                //catch(Exception ex)
                //{

                //}





                
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


                SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();
                oConnection = new SqlConnection(SqlConn);
                oConnection.Open();
                qry = "SELECT DiscountedAmount FROM T_ONLINE_BO_REG a,T_ONLINE_BO_PROMO b WHERE a.PromoCode=b.PromoCode AND [TRACKINGNO]='" + Session["trackingno"].ToString() + "'";
                oCommand = new SqlCommand(qry, oConnection);
                otable = new DataTable();
                otable.Load(oCommand.ExecuteReader());
                oConnection.Close();
                Session["PaybleAmount"]= "450";
                ViewBag.amount = "450";
                foreach (DataRow dr in otable.Rows)
                {
                    Session["PaybleAmount"] = Convert.ToString(450 - Convert.ToInt16(dr["DiscountedAmount"].ToString()));
                    ViewBag.amount = Convert.ToString(450 - Convert.ToInt16(dr["DiscountedAmount"].ToString()));
                }

                return View();
            }
            else
                return RedirectToAction("index", "Home");
        }

        public  ActionResult EkycIndex()
        {
            if (Session["trackingno"] != null)
            {
                ViewData["imageForEKYC"] = GetImagesForEkyc().ToList().FirstOrDefault(); ///////////////  Get All Stored Images
               
            }
            else
            {
                return RedirectToAction("index", "Home");
            }


            //TempData["message"] = "Your OTP Doesn't Match, Please try again !!";
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> EkycIndex(EkycModel img, string id)
        {
            if (img.mobile_otp != Session["OTP"].ToString())
            {
                TempData["message"] = "Your OTP Doesn't Match, Please try again !!";
                return RedirectToAction("EkycIndex", "Client");
            }       
            if (img.File == null && img.person_photo == null)
            {
                TempData["message"] = "Please Upload or Capture Your Image";
                return RedirectToAction("EkycIndex", "Client");
            }
            if (img.File != null && !(img.File.ContentType == "image/jpeg"))
            {
                TempData["message"] = "Please Upload or Capture Your Image/JPEG";
                return RedirectToAction("EkycIndex", "Client");
            }
            if (img.person_photo != null)
            {
                OtherDBHandler DB = new OtherDBHandler();
                SqlConnection oConnection = null;
                Session["OTP"] = string.Empty;

                string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();
                //var path = img.File.p;
                //System.Drawing.Image imag = System.Drawing.Image.FromStream(img.File.InputStream);

                try
                {
                    try
                    {
                        oConnection = new SqlConnection(SqlConn);

                        oConnection.Open();

                        SqlCommand oCommand = new SqlCommand("UPDATE [T_ONLINE_BO_AccHolderImages] SET " + id + "= @Pic,pDate=GETDATE(),iIsComplete=1 WHERE TRACKINGNO='" + @Session["trackingno"].ToString() + "'", oConnection);
                        //oCommand.Parameters.Add("Pic", SqlDbType.Image, 0).Value =ConvertImageToByteArray(imag, System.Drawing.Imaging.ImageFormat.Jpeg);
                        //oCommand.Parameters.AddWithValue("Pic", DB.ConvertImageToByteArray(imag, System.Drawing.Imaging.ImageFormat.Jpeg));
                        byte[] data2 = Convert.FromBase64String(img.person_photo);
                        oCommand.Parameters.AddWithValue("@Pic", data2);
                        int queryResult = oCommand.ExecuteNonQuery();

                        if (queryResult > 0)
                        {
                            //string image = Convert.ToBase64String(GetImagesForEkyc().ToList().FirstOrDefault().fImage, 0, GetImagesForEkyc().ToList().FirstOrDefault().fImage.Length);
                            string date = (DateTime.ParseExact(img.person_dob, "dd/MM/yyyy", null)).ToString("yyyy-MM-dd");// Convert.ToDateTime(img.person_dob).ToString("yyyy-MM-dd");
                            DataFaceModel dfm = await GetDataByImg(img.person_photo, date, img.national_id);

                            if (dfm.voter != null)
                            {
                                if (dfm.voter.nationalId != null)
                                {

                                    string query = "INSERT INTO [dbo].[T_Porichoy_NID_Data] ([fatherEn],[motherEn],[spouseEn],[permanentAddressEn],[presentAddressEn],[name],[nameEn],[father],[mother],[gender],[profession],[spouse],[dob],[permanentAddress],[presentAddress],[nationalId],[oldNationalId],[photo],[matched],[percentage]) VALUES(@fatherEn,@motherEn,@spouseEn,@permanentAddressEn,@presentAddressEn,@name,@nameEn,@father,@mother,@gender,@profession,@spouse,@dob,@permanentAddress,@presentAddress,@nationalId,@oldNationalId,@photo,@matched,@percentage)";

                                    SqlCommand oCommandEkyc = new SqlCommand(query, oConnection);
                                    oCommandEkyc.Parameters.AddWithValue("@fatherEn", dfm.voter.fatherEn ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@motherEn", dfm.voter.motherEn ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@spouseEn", dfm.voter.spouseEn ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@permanentAddressEn", dfm.voter.permanentAddressEn ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@presentAddressEn", dfm.voter.presentAddressEn ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@name", dfm.voter.name ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@nameEn", dfm.voter.nameEn ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@father", dfm.voter.father ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@mother", dfm.voter.mother ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@gender", dfm.voter.gender ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@profession", dfm.voter.profession ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@spouse", dfm.voter.spouse ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@dob", dfm.voter.dob ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@permanentAddress", dfm.voter.permanentAddress ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@presentAddress", dfm.voter.presentAddress ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@nationalId", dfm.voter.nationalId ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@oldNationalId", dfm.voter.oldNationalId ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@photo", dfm.voter.photo ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@matched", dfm.voter.faceMatchResult.matched);
                                    oCommandEkyc.Parameters.AddWithValue("@percentage", dfm.voter.faceMatchResult.percentage);

                                    int i = oCommandEkyc.ExecuteNonQuery();


                                    if (i >= 1)
                                    {
                                        if (dfm.voter.faceMatchResult.matched == true && dfm.voter.faceMatchResult.percentage >= 50)
                                        {
                                            string data = dfm.voter.nameEn.Trim();
                                            string lastName = string.Empty;
                                            string firstName = string.Empty;
                                            string[] name = data.Split(' ');

                                            if (name.Length > 1)
                                            {
                                                lastName = name[name.Length - 1];
                                                if (lastName == string.Empty)
                                                    lastName = ".";
                                                firstName = data.Replace(lastName, "");
                                            }
                                            else
                                            {
                                                lastName = ".";
                                                firstName = data;
                                            }


                                            //////////////  Update Registration Table 
                                            oCommand = new SqlCommand("UPDATE [T_ONLINE_BO_REG] SET FIRSTNAME= '" + firstName + "',LASTNAME='" + lastName + "',IsEkycValid=" + 1 + ",pDate=GETDATE() WHERE TRACKINGNO='" + @Session["trackingno"].ToString() + "'", oConnection);
                                            oCommand.ExecuteNonQuery();
                                            /////////////// End Upadte Registration Table /

                                            //////////////  Update T_ONLINE_BO_AccountHolder Table 
                                            oCommand = new SqlCommand("UPDATE [T_ONLINE_BO_AccountHolder] SET fFirstName= '" + firstName + "',fLastName='" + lastName + "',fDoB='" + dfm.voter.dob + "',fFName='" + dfm.voter.fatherEn + "',fMName='" + dfm.voter.motherEn + "',fAddress='" + dfm.voter.presentAddressEn.Replace("'","''") + "',fNID='" + dfm.voter.nationalId + "' WHERE TRACKINGNO='" + @Session["trackingno"].ToString() + "'", oConnection);
                                            oCommand.ExecuteNonQuery();
                                            /////////////// End Upadte T_ONLINE_BO_AccountHolder Table /

                                            return RedirectToAction("AccountHolder", "Client");


                                        }
                                        else
                                        {
                                            TempData["message"] = "Your face doesn't match with NID server. Please try again later!!!";
                                            return RedirectToAction("EkycIndex", "Client");
                                        }
                                    }
                                    else
                                    {
                                        TempData["message"] = "No Data Found From NID Server !!! Plese Try Again Later";
                                        return RedirectToAction("EkycIndex", "Client");
                                    }
                                }
                                else
                                {
                                    TempData["message"] = "No Data Found From NID Server !!! Plese Try Again Later";
                                    return RedirectToAction("EkycIndex", "Client");
                                }
                            }
                            else
                            {
                                TempData["message"] = "No Data Found From NID Server !!! Plese Try Again Later";
                                return RedirectToAction("EkycIndex", "Client");
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        //ViewData.Add(id, "Error: " + ex.Message);
                        // ViewBag.ImgUpSuccess = "Error: " + ex.Message;
                        TempData["message"] = "Error: " + ex.Message;
                        return RedirectToAction("EkycIndex", "Client");
                    }
                }
                finally
                {
                    if (oConnection != null)
                        oConnection.Close();
                }
            }
            else
            {
                OtherDBHandler DB = new OtherDBHandler();
                SqlConnection oConnection = null;
                Session["OTP"] = string.Empty;

                string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();
                //var path = img.File.p;
                System.Drawing.Image imag = System.Drawing.Image.FromStream(img.File.InputStream);

                try
                {
                    try
                    {
                        oConnection = new SqlConnection(SqlConn);

                        oConnection.Open();

                        SqlCommand oCommand = new SqlCommand("UPDATE [T_ONLINE_BO_AccHolderImages] SET " + id + "= @Pic,pDate=GETDATE(),iIsComplete=1 WHERE TRACKINGNO='" + @Session["trackingno"].ToString() + "'", oConnection);
                        //oCommand.Parameters.Add("Pic", SqlDbType.Image, 0).Value =ConvertImageToByteArray(imag, System.Drawing.Imaging.ImageFormat.Jpeg);
                        oCommand.Parameters.AddWithValue("Pic", DB.ConvertImageToByteArray(imag, System.Drawing.Imaging.ImageFormat.Jpeg));
                        int queryResult = oCommand.ExecuteNonQuery();

                        if (queryResult > 0)
                        {


                            string image = Convert.ToBase64String(GetImagesForEkyc().ToList().FirstOrDefault().fImage, 0, GetImagesForEkyc().ToList().FirstOrDefault().fImage.Length);
                            string date = DateTime.ParseExact(img.person_dob, "dd/MM/yyyy", null).ToString("yyyy-MM-dd");// Convert.ToDateTime(img.person_dob).ToString("yyyy-MM-dd");
                            DataFaceModel dfm = await GetDataByImg(image, date, img.national_id);

                            if (dfm.voter !=null)
                            {
                                if (dfm.voter.nationalId != null)
                                {

                                    string query = "INSERT INTO [dbo].[T_Porichoy_NID_Data] ([fatherEn],[motherEn],[spouseEn],[permanentAddressEn],[presentAddressEn],[name],[nameEn],[father],[mother],[gender],[profession],[spouse],[dob],[permanentAddress],[presentAddress],[nationalId],[oldNationalId],[photo],[matched],[percentage]) VALUES(@fatherEn,@motherEn,@spouseEn,@permanentAddressEn,@presentAddressEn,@name,@nameEn,@father,@mother,@gender,@profession,@spouse,@dob,@permanentAddress,@presentAddress,@nationalId,@oldNationalId,@photo,@matched,@percentage)";

                                    SqlCommand oCommandEkyc = new SqlCommand(query, oConnection);
                                    oCommandEkyc.Parameters.AddWithValue("@fatherEn", dfm.voter.fatherEn ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@motherEn", dfm.voter.motherEn ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@spouseEn", dfm.voter.spouseEn ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@permanentAddressEn", dfm.voter.permanentAddressEn ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@presentAddressEn", dfm.voter.presentAddressEn ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@name", dfm.voter.name ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@nameEn", dfm.voter.nameEn ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@father", dfm.voter.father ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@mother", dfm.voter.mother ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@gender", dfm.voter.gender ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@profession", dfm.voter.profession ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@spouse", dfm.voter.spouse ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@dob", dfm.voter.dob ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@permanentAddress", dfm.voter.permanentAddress ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@presentAddress", dfm.voter.presentAddress ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@nationalId", dfm.voter.nationalId ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@oldNationalId", dfm.voter.oldNationalId ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@photo", dfm.voter.photo ?? (object)DBNull.Value);
                                    oCommandEkyc.Parameters.AddWithValue("@matched", dfm.voter.faceMatchResult.matched);
                                    oCommandEkyc.Parameters.AddWithValue("@percentage", dfm.voter.faceMatchResult.percentage);

                                    int i = oCommandEkyc.ExecuteNonQuery();


                                    if (i >= 1)
                                    {
                                        if (dfm.voter.faceMatchResult.matched == true && dfm.voter.faceMatchResult.percentage >= 50)
                                        {
                                            string data = dfm.voter.nameEn.Trim();
                                            string lastName = string.Empty;
                                            string firstName = string.Empty;
                                            string[] name = data.Split(' ');

                                            if (name.Length > 1)
                                            {
                                                lastName = name[name.Length - 1];
                                                if (lastName == string.Empty)
                                                    lastName = ".";
                                                firstName = data.Replace(lastName, "");
                                            }
                                            else
                                            {
                                                lastName = ".";
                                                firstName = data;
                                            }


                                            //////////////  Update Registration Table 
                                            oCommand = new SqlCommand("UPDATE [T_ONLINE_BO_REG] SET FIRSTNAME= '" + firstName + "',LASTNAME='" + lastName + "',IsEkycValid=" + 1 + ",pDate=GETDATE() WHERE TRACKINGNO='" + @Session["trackingno"].ToString() + "'", oConnection);
                                            oCommand.ExecuteNonQuery();
                                            /////////////// End Upadte Registration Table /

                                            //////////////  Update T_ONLINE_BO_AccountHolder Table 
                                            oCommand = new SqlCommand("UPDATE [T_ONLINE_BO_AccountHolder] SET fFirstName= '" + firstName + "',fLastName='" + lastName + "',fDoB='" + dfm.voter.dob + "',fFName='" + dfm.voter.fatherEn + "',fMName='" + dfm.voter.motherEn + "',fAddress='" + dfm.voter.presentAddressEn.Replace("'","") + "',fNID='" + dfm.voter.nationalId + "' WHERE TRACKINGNO='" + @Session["trackingno"].ToString() + "'", oConnection);
                                            oCommand.ExecuteNonQuery();
                                            /////////////// End Upadte T_ONLINE_BO_AccountHolder Table /

                                            return RedirectToAction("AccountHolder", "Client");


                                        }
                                        else
                                        {
                                            TempData["message"] = "Your face doesn't match with NID server. Please try again later!!!";
                                            return RedirectToAction("EkycIndex", "Client");
                                        }
                                    }
                                    else
                                    {
                                        TempData["message"] = "Somthing Went Wrong!!!. Please try again later!!!";
                                        return RedirectToAction("EkycIndex", "Client");
                                    }
                                }
                                else
                                {
                                    TempData["message"] = "No Data Found From NID Server !!! Plese Try Again Later";
                                    return RedirectToAction("EkycIndex", "Client");
                                }
                            }
                            else
                            {
                                TempData["message"] = "No Data Found From NID Server !!! Plese Try Again Later";
                                return RedirectToAction("EkycIndex", "Client");
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        //ViewData.Add(id, "Error: " + ex.Message);
                        //ViewBag.ImgUpSuccess = "Error: " + ex.Message;
                        TempData["message"] = "Error: " + ex.Message;
                        return RedirectToAction("EkycIndex", "Client");
                    }
                }
                finally
                {
                    if (oConnection != null)
                        oConnection.Close();
                }
            }
            

            ViewData["imageForEKYC"] = GetImagesForEkyc().ToList().FirstOrDefault(); ///////////////  Get All Stored Images
            return View();
        }

        public ActionResult NidFontPageFastHolder()
        {
            ViewData["imageForEKYC"] = GetImagesForEkyc().ToList().FirstOrDefault(); ///////////////  Get All Stored Images

            return View();
        }

        public async Task<DataFaceModel> GetDataByImg(string image, string date, string nid)
        {
            DataFaceModel dtm = new DataFaceModel();


            APICallWithFaceModel callingData = new APICallWithFaceModel();
            callingData.national_id = nid;
            callingData.team_tx_id = String.Empty;
            callingData.person_dob = date;
            callingData.english_output = true;
            callingData.person_photo = image;
            

            StringContent content = new StringContent(JsonConvert.SerializeObject(callingData), Encoding.UTF8, "application/json");

            var apiUrl = "https://api.porichoybd.com/api/v0/kyc/nid-person-values-image-match";
            var apiKey = "7b2637c9-9614-41b7-a8a4-339e2989b2dc";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-api-key", apiKey);
            //ServicePointManager.Expect100Continue = true;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;


            var response = await client.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                dtm = JsonConvert.DeserializeObject<DataFaceModel>(Convert.ToString(responseBody));

            }
            else
            {
                //Console.WriteLine($"Request failed with status code {response.StatusCode}");
            }

            return dtm;
        }

        [HttpPost]
        public ActionResult NidFontPageFastHolder(EkycModel img, string id)
        {

            //if (img.File == null)
            //{
            //    ViewData["userimg"]="Image Not Selected.";
            //    return View();
            //}
            //if (!(img.File.ContentType == "image/jpeg"))
            //{              
            //    ViewBag.userimg("File type allowed : jpeg.");
            //    return View();
            //}

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

                    SqlCommand oCommand = new SqlCommand("UPDATE [T_ONLINE_BO_AccHolderImages] SET " + id + "= @Pic,pDate=GETDATE(),iIsComplete=1 WHERE TRACKINGNO='" + @Session["trackingno"].ToString() + "'", oConnection);
                    //oCommand.Parameters.Add("Pic", SqlDbType.Image, 0).Value =ConvertImageToByteArray(imag, System.Drawing.Imaging.ImageFormat.Jpeg);
                    oCommand.Parameters.AddWithValue("Pic", DB.ConvertImageToByteArray(imag, System.Drawing.Imaging.ImageFormat.Jpeg));
                    int queryResult = oCommand.ExecuteNonQuery();
                    if (queryResult > 0)
                        ViewData.Add(id, "Successfully Uploaded.");

                }
                catch (Exception ex)
                {
                    //ViewData.Add(id, "Error: " + ex.Message);
                    ViewBag.ImgUpSuccess = "Error: " + ex.Message;
                }
            }
            finally
            {
                if (oConnection != null)
                    oConnection.Close();
            }

            ViewData["imageForEKYC"] = GetImagesForEkyc().ToList().FirstOrDefault(); ///////////////  Get All Stored Images
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
                string query = "SELECT TOP 1 fFirstName,jFirstName,aFirstName,n1FirstName,n2FirstName,g1FirstName,g2FirstName FROM T_ONLINE_BO_AccountHolder a,T_ONLINE_BO_AccHolderAuthorize b,T_ONLINE_BO_AccHolderNominee c where a.TRACKINGNO=@trackingno AND b.TRACKINGNO=@trackingno AND c.TRACKINGNO=@trackingno";

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

                    SqlCommand oCommand = new SqlCommand("UPDATE [T_ONLINE_BO_AccHolderImages] SET " + id + "= @Pic,pDate=GETDATE(),iIsComplete=1 WHERE TRACKINGNO='" + @Session["trackingno"].ToString() + "'", oConnection);
                    //oCommand.Parameters.Add("Pic", SqlDbType.Image, 0).Value =ConvertImageToByteArray(imag, System.Drawing.Imaging.ImageFormat.Jpeg);
                    oCommand.Parameters.AddWithValue("Pic", DB.ConvertImageToByteArray(imag, System.Drawing.Imaging.ImageFormat.Jpeg));
                    int queryResult = oCommand.ExecuteNonQuery();
                    if (queryResult > 0)
                        ViewData.Add(id,"Successfully Uploaded.");

                    ParGlobalClass.LastUpdateDate(@Session["Trackingno"].ToString());

                    /////////////////////////////  select and pass default value //////////////////////

                    ViewData["image"] = GetImages().ToList(); ///////////////  Get All Stored Images

                    SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();

                    var incompleteList = new List<OnlineBODetailModel>();
                    oConnection = new SqlConnection(SqlConn);
                    oConnection.Open();
                    string query = "SELECT TOP 1 fFirstName,jFirstName,aFirstName,n1FirstName,n2FirstName,g1FirstName,g2FirstName FROM T_ONLINE_BO_AccountHolder a,T_ONLINE_BO_AccHolderAuthorize b,T_ONLINE_BO_AccHolderNominee c where a.TRACKINGNO=@trackingno AND b.TRACKINGNO=@trackingno AND c.TRACKINGNO=@trackingno";

                    oCommand = new SqlCommand(query, oConnection);
                    oCommand.Parameters.AddWithValue("@trackingno", @Session["Trackingno"].ToString());

                    DataTable oTable = new DataTable();
                    oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

                    foreach (DataRow dr in oTable.Rows)
                    {
                        incompleteList.Add(new OnlineBODetailModel { fFirstName = dr["fFirstName"].ToString(), jFastName = dr["jFirstName"].ToString(), aFirstName = dr["aFirstName"].ToString(), n1FirstName = dr["n1FirstName"].ToString(), n2FirstName = dr["n2FirstName"].ToString(), g1FirstName = dr["g1FirstName"].ToString(), g2FirstName = dr["g2FirstName"].ToString() });
                    }

                    ViewData["printmr"] = incompleteList.ToList();
                    ////////////////////////////////////////////////////////////////////////////


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

            return View();
        }


        public ActionResult PaymentStatus()
        {
            if (Session["trackingno"] != null)
            {

                string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();
                SqlConnection oConnection = new SqlConnection(SqlConn);
                oConnection.Open();
                string qry = "SELECT * FROM [T_ONLINE_PAYMENT_RECEIVER] WHERE [tracking_no]='" + Session["trackingno"] .ToString()+ "'";
                SqlCommand oCommand = new SqlCommand(qry, oConnection);
                DataTable otable = new DataTable();
                otable.Load(oCommand.ExecuteReader());
                oConnection.Close();

                foreach(DataRow dr in otable.Rows)
                {
                    ViewData["payment_id"] = dr["payment_id"].ToString();
                    ViewData["trxID"] = dr["trxID"].ToString();
                    ViewData["amount"] = dr["amount"].ToString();
                    ViewData["create_date"] = dr["create_date"].ToString();
                    ViewData["status"] = dr["status"].ToString();
                    ViewData["isApproved"] = dr["isApproved"].ToString();
                }
                


                return View();
            }
            else
            {
                return RedirectToAction("index", "Home");
            }
        }



        [HttpPost]
        public ActionResult PaymentStatus(string id)
        {
            try
            {
                //string tag = Request.Form["tag"];
                string status = Request.Form["status"];
                //string tracking_no = Request.Form["tracking_no"];
                string tracking_no = Request.Form["value_a"];
                string payment_id = Request.Form["bank_tran_id"];
                string amount = Request.Form["amount"];
                string trxID = Request.Form["tran_id"];
                string card_type = Request.Form["card_type"];
                string tran_date = Request.Form["tran_date"];

                string cus_name = Request.Form["cus_name"];


                Session["username"]= "";
                Session["trackingno"]= tracking_no;

                
                if (amount == "" || String.IsNullOrEmpty(amount))
                {
                    amount = "0";
                }

                ViewData["pay_id"] = payment_id;

                if (status == "VALID")
                {
                    string SqlConn1 = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();
                    SqlConnection oConnection1 = new SqlConnection(SqlConn1);
                    oConnection1.Open();
                    string qry1 = "UPDATE T_ONLINE_BO_REG SET IsAgree=1 WHERE [TRACKINGNO]='"+tracking_no + "'";
                    SqlCommand oCommand1 = new SqlCommand(qry1, oConnection1);
                    oCommand1.ExecuteNonQuery();
                    oConnection1.Close();

                    string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();
                    SqlConnection oConnection = new SqlConnection(SqlConn);
                    oConnection.Open();
                    //string qry = "INSERT INTO T_ONLINE_PAYMENT_RECEIVER ([tracking_no] ,[payment_id] ,[trxID] ,[amount],[create_date] ,[status], [source]) VALUES ('" + tracking_no + "', '" + payment_id + "', '" + trxID + "', " + amount + ", GETDATE(), '" + tag + "')";
                    string qry = " INSERT INTO [T_ONLINE_PAYMENT_RECEIVER] ([tracking_no] ,[payment_id] ,[trxID] ,[amount] ,[create_date] ,[status] ,[source])  VALUES (@tracking_no, @payment_id, @trxID, @amount, @create_date, @status,@source)";
                    SqlCommand oCommand = new SqlCommand(qry, oConnection);
                    oCommand.Parameters.Add("@tracking_no", tracking_no);
                    oCommand.Parameters.Add("@payment_id", payment_id);
                    oCommand.Parameters.Add("@trxID", trxID);
                    oCommand.Parameters.Add("@amount", amount);
                    oCommand.Parameters.Add("@create_date", tran_date);
                    oCommand.Parameters.Add("@status", "success");
                    oCommand.Parameters.Add("@source", card_type);
                    oCommand.ExecuteNonQuery();
                    oConnection.Close();
                    ParGlobalClass.LastUpdateDate(tracking_no);
                    
                    return RedirectToAction("PaymentStatus", "Client");

                    //return View("Success_SSL");
                }
                else
                {
                    string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();
                    SqlConnection oConnection = new SqlConnection(SqlConn);
                    oConnection.Open();
                    string qry = "INSERT INTO T_ONLINE_PAYMENT_RECEIVER ([create_date] ,[status]) VALUES ( GETDATE(), '" + status + "')";
                    SqlCommand oCommand = new SqlCommand(qry, oConnection);
                    oCommand.ExecuteNonQuery();
                    oConnection.Close();
                    return RedirectToAction("index", "Home");
                }
            }
            catch (Exception ex)
            {

            }




            return View();
        }


        /////integration of SSL Commerz payment gateway

        public JsonResult mycall()
        {
            //string baseUrl = @"{Request.Scheme}://{Request.Host}{Request.PathBase}/";
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";

            NameValueCollection PostData = new NameValueCollection
            {
                { "total_amount", Session["PaybleAmount"].ToString()},
                { "currency", "BDT"},
                { "tran_id", GenerateUniqueId() },
                { "success_url", baseUrl + "Client/PaymentStatus" },
                { "fail_url", baseUrl + "Client/PaymentGatewayCallback" },
                { "cancel_url", baseUrl + "Client/PaymentGatewayCallback" },
                { "cus_name", Session["username"].ToString() },
                { "cus_email", "bo@royalcapitalbd.com" },
                { "cus_add1", "Address Line On" },
                { "cus_city", "Dhaka" },
                { "cus_postcode", "1219" },
                { "cus_country", "Bangladesh" },
                { "cus_phone", Session["MOBILE"].ToString() },
                { "shipping_method", "NO" },
                { "product_name", "ONLINE BO" },
                { "product_category", "Service" },
                { "product_profile", "general" },
                { "value_a", Session["trackingno"].ToString() },
                { "value_b", "BO" },
                { "value_c", get_ip()+ ", Webpage" },
                { "value_d", Session["MOBILE"].ToString() }
            };

            var sslcz = new SSLCommerz(storeID, storePassword);

            var response = sslcz.InitiateTransaction(PostData);
            var model = new { status = "success", data = response.GatewayPageURL, logo = response.storeLogo };
            var json_val = Json(model, JsonRequestBehavior.AllowGet);
            //Response.Write(json_val.ToString());

            Session["PaybleAmount"] = null;
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

        [HttpPost]
        public async Task<ActionResult> SendOTP(bool isInetrnationalNo)
        {
            Session["OTP"]=string.Empty;
            try
            {
                string send_whatsUp_sms = "";
                string snd_sms_data = "";

				string mobile = "";
                string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();

                SqlConnection oConnection = new SqlConnection(SqlConn);
                oConnection.Open();
                string query = "SELECT TOP 1 MOBILE FROM T_ONLINE_BO_REG WHERE [TRACKINGNO]='" + @Session["trackingno"].ToString() + "'";
                SqlCommand oCommand = new SqlCommand(query, oConnection);

                DataTable oTable = new DataTable();
                oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

                foreach (DataRow dr in oTable.Rows)
                {
                    mobile = dr["MOBILE"].ToString();
                }

                if (mobile != null)
                {
                    if (isInetrnationalNo)
                    {
					     send_whatsUp_sms = await send_WhatsUp_SMS_automatically(mobile);

						if (send_whatsUp_sms != "failed")
						{
							Session["OTP"] = send_whatsUp_sms.ToString();
							return Json(new { success = true, msg = "OTP Succesfully sent your WhatsApp application" });
							
						}
						else
						{	
							return Json(new { success = false, msg = "OTP Sent Failed to your WhatsApp application!!! Plese Try Again Later" });							
						}

					}
                    else
                    {
						snd_sms_data = send_SMS_automatically(mobile);

						if (snd_sms_data != "failed")
						{
							Session["OTP"] = snd_sms_data.ToString();
							//  TempData["messageSuccess"] = "OTP Succesfully sent your mobile.";
							return Json(new { success = true, msg = "OTP Succesfully sent your mobile" });
							//return RedirectToAction("EkycIndex", "Client");
						}

						else
						{
							//  TempData["message"] = "OTP Sent Failed !!! Plese Try Again Later";
							return Json(new { success = false, msg = "OTP Sent Failed !!! Plese Try Again Later" });
							//return RedirectToAction("EkycIndex", "Client");
						}
					}
                        

                }
                else
                {
                   // TempData["message"] = "No Mobile Number Found";
                    return Json(new { success = false, msg = "No Mobile Number Found" });
                    //return RedirectToAction("EkycIndex", "Client");
                }

                
            }
            catch (Exception ex)
            {

                TempData["message"] = "Error: " + ex.Message;
                return RedirectToAction("EkycIndex", "Client");
            }
        }


		public string send_SMS_automatically(string mobileNo)
        {
            Random ran_num = new Random();
            string verification_code = ran_num.Next(100000, 999999).ToString();
            string response = "failed";

            string url = "https://sms.royalcapitalbd.com/api/sendSMS?apikey=AMEYO02G7ZZX&mobile=" +
                mobileNo + "&text=" + "Your Mobile Verification Code For Online BO Opening : " + verification_code + " \nFor query: 09606555333";

            try
            {
                using (var wb = new WebClient())
                {
                    response = wb.DownloadString(url);
                    if (response.Contains("SUCCESS")) 
                    {
                        response = verification_code;
                    }
                }
            }
            catch (Exception ex)
            {
                response = "failed";
            }
            return response;
        }

		public async Task<string> send_WhatsUp_SMS_automatically(string mobileNo)
		{
			Random ran_num = new Random();
			string verification_code = ran_num.Next(100000, 999999).ToString();
			string responseContent = "failed";

			string apiUrl = "https://backend.aisensy.com/campaign/t1/api/v2";

			// Define the request body
			var requestBody = new
			{
				apiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjY0YmUyNjVkY2IwYTM0MGJkMzNiYTg1YyIsIm5hbWUiOiJSb3lhbCBDYXBpdGFsIEx0ZC4iLCJhcHBOYW1lIjoiQWlTZW5zeSIsImNsaWVudElkIjoiNjRiZTI2NWRjYjBhMzQwYmQzM2JhODU2IiwiYWN0aXZlUGxhbiI6IkJBU0lDX01PTlRITFkiLCJpYXQiOjE2OTAxODMyNjF9.zSIlck9dwspcMH2dEG1_tOAWNbXcVi24SC-GUU2u-Qc",
				campaignName = "bo_registration_authentication",
				destination = mobileNo,
				userName = "Royal Capital",
				templateParams = new string[] { verification_code },
				buttons = new[]
				{
			new
			{
				type = "button",
				sub_type = "url",
				index = "0",
				parameters = new[]
				{
					new
					{
						type = "text",
						text = verification_code
					}
				}
			}
		}
			};
			System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

			string jsonRequestBody = JsonConvert.SerializeObject(requestBody);

			using (HttpClient client = new HttpClient())
			{
				try
				{
					// Send the POST request with the JSON body, setting Content-Type via StringContent
					var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");
					HttpResponseMessage response = await client.PostAsync(apiUrl, content);

					// Check if the request was successful
					if (response.IsSuccessStatusCode)
					{
						responseContent = await response.Content.ReadAsStringAsync();
                        responseContent = verification_code;

						Console.WriteLine("Response: " + responseContent);
					}
					else
					{
						Console.WriteLine("Error: " + response.StatusCode);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Request failed: " + ex.Message);
				}
			}

			return responseContent;
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

    }////end of the main class
}


