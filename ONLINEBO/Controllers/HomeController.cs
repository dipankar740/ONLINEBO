using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ONLINEBO.Models;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;


namespace ONLINEBO.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index(string id=null)
        {
            if (id != null)
                Session["RefarralCode"] = id; //Request.QueryString[id].ToString();
            if (id == null)
                Session["RefarralCode"] = "0";

            return View();
        }

        [HttpPost]
        public ActionResult Index(HomeModel bomodel)
        {
            ////////////////    RefarralCode
            if (Session["RefarralCode"].ToString() != "0")
            {
                try
                {

                    string SqlConn1 = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();
                    SqlConnection oConnection1 = new SqlConnection(SqlConn1);

                    oConnection1.Open();

                    string query1 = "SELECT RefarralCode FROM T_ONLINE_BO_RefarralCode WHERE IsActive=1 AND RefarralCode=@ref";
                    SqlCommand oCommand1 = new SqlCommand(query1, oConnection1);
                    oCommand1.Parameters.AddWithValue("@ref", Session["RefarralCode"].ToString());

                    DataTable oTable1 = new DataTable();
                    oTable1.Load(oCommand1.ExecuteReader(CommandBehavior.CloseConnection));

                    foreach (DataRow dr in oTable1.Rows)
                    {
                        Session["RefarralCode"] = dr["RefarralCode"].ToString();
                        //Session["PromoCode"] = dr["PromoCode"].ToString();
                        //Session["DiscountedAmount"] = dr["DiscountedAmount"].ToString();
                    }
                    if (oTable1.Rows.Count <= 0)
                    {
                        Session["RefarralCode"] = "0";
                    }

                }
                catch (Exception ex)
                {

                    ViewBag.Message2 = ex.Message;
                }
            }

            ////////////////   END RefarralCode


            ////////////////   PromoCode
            if (!string.IsNullOrEmpty(bomodel.PromoCode))
            { 
                try
                {

                    
                    

                    string SqlConn1 = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();
                    SqlConnection oConnection1 = new SqlConnection(SqlConn1);

                    oConnection1.Open();

                    string query1 = "SELECT * FROM T_ONLINE_BO_PROMO WHERE PromoCode=@promo and (CONVERT(varchar(10),GETDATE(),101)<=Pend AND CONVERT(varchar(10),GETDATE(),101)>=Pstart AND Pstart <= Pend)";
                    SqlCommand oCommand1 = new SqlCommand(query1, oConnection1);
                    oCommand1.Parameters.AddWithValue("@promo", bomodel.PromoCode);

                    DataTable oTable1 = new DataTable();
                    oTable1.Load(oCommand1.ExecuteReader(CommandBehavior.CloseConnection));

                    foreach (DataRow dr in oTable1.Rows)
                    {

                        //Session["PromoCode"] = dr["PromoCode"].ToString();
                        //Session["DiscountedAmount"] = dr["DiscountedAmount"].ToString();
                    }
                    if (oTable1.Rows.Count <= 0)
                    {
                        ViewBag.Message2 = "Promo code is not valid!! Or Promo code is Expired!!!";
                        return View();
                    }

                }
                catch (Exception ex)
                {

                    ViewBag.Message2 = ex.Message;
                }
             }

            ////////////////   End PromoCode

            int mobileNumberCount = 0;
            try
            {
                string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();
                SqlConnection oConnection = new SqlConnection(SqlConn);

                string query = "SELECT COUNT(mobile1) NoOfMobile FROM T_Client1 Where mobile1='"+bomodel.Mobile+"' AND boStatus='Active'";

                SqlCommand oCommand = new SqlCommand(query, oConnection);
                oConnection.Open();

                DataTable oTable = new DataTable();
                oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

                foreach (DataRow dr in oTable.Rows)
                {
                    mobileNumberCount = Convert.ToInt16(dr["NoOfMobile"].ToString());   
                }
                if (mobileNumberCount >= 2)
                {
                    ViewBag.Message2 = "You Have Already Two Active BO Account With This Mobile Number.";
                    return View();
                }

            }
            catch (Exception ex)
            {
                ViewBag.Message2 = ex.Message;
                return View();
            }

            //var user="";
            try
            {
                string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();

                SqlConnection oConnection = new SqlConnection(SqlConn);


                string query = "SELECT (CASE WHEN MAX(SL) IS null THEN 0 ELSE MAX(SL) END)+1 Tmax FROM T_ONLINE_BO_REG";


                SqlCommand oCommand = new SqlCommand(query, oConnection);

                oConnection.Open();

                DataTable oTable = new DataTable();
                oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));
                
                foreach(DataRow dr in oTable.Rows)
                {
                    //bomodel.TrackingNo = (dr["Tmax"].ToString() + string.Format("{0:ddmm}", DateTime.Now.ToShortDateString().Replace("/", "")));
                    bomodel.TrackingNo = "BO"+dr["Tmax"].ToString();
                    //bomodel.TrackingNo = user;
                }

                Random generator = new Random();
                String r = generator.Next(100000, 999999).ToString("D6");
                bomodel.Password = r;

                var userid = bomodel.TrackingNo;
                var pass = bomodel.Password;


                if (ModelState.IsValid)
                {
                    OnlineBoRegDBHandle DB = new OnlineBoRegDBHandle();

                    if (DB.AddOnlineBoReg(bomodel))
                    {
                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient("159.21.2.246");
                        mail.From = new MailAddress("bo@royalcapitalbd.com","Royal Capital Online BO Opening");
                        mail.To.Add(bomodel.Email);
                        //mail.CC.Add("rafiq@royalcapitalbd.com");
                        // mail.CC.Add("invicta.corp@gmail.com");
                        //mail.CC.Add("rommel75@yahoo.com");
                        //mail.Bcc.Add("dipankar.bnk@gmail.com");
                        //mail.Bcc.Add("bparvej@yahoo.com");
                        mail.Subject = "Royal Capital-Online BO Account Opening Registration";
                        mail.Body = "Dear Concern! \n\nGood day from Royal Capital Ltd.\nThanks for signing up with our online BO opening portal.\n\n1.Access to our portal  http://bo.royalcapitalbd.com/Home/Login \n2.Put in your ID and Password in the “Sign in” area" + "\n\nUser ID: " + userid + "\nPassword: " + pass + "\n\nFor more information, call at 16379 or 09606016379\n\nThanks for being with us.\nRoyal Capital Ltd.";
                        
                        //mail.Attachments.Add(new Attachment(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\parvej.jpg"));

                        SmtpServer.Port = 25;
                        SmtpServer.Credentials = new System.Net.NetworkCredential("bo@royalcapitalbd.com", "rclbd1234%");
                        //SmtpServer.EnableSsl = true;

                        SmtpServer.Send(mail);

                        try
                        {
                            string SqlConn2 = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();
                            SqlConnection oConnection2 = new SqlConnection(SqlConn2);

                            string query2 = "UPDATE T_ONLINE_BO_REG SET PromoCode='" + bomodel.PromoCode + "',RefarralCode='" + Session["RefarralCode"].ToString() + "' Where TRACKINGNO='" + bomodel.TrackingNo + "'";

                            SqlCommand oCommand2 = new SqlCommand(query2, oConnection2);
                            oConnection2.Open();

                            oCommand2.ExecuteNonQuery();

                            oConnection2.Close();

                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message2 = ex.Message;
                            return View();
                        }

                        Session["SeccessReg"] = "Please check your email. A temporary User ID and Password sent to your email.";
                        //ViewBag.Message1 = "Please check your email. A temporary User ID and Password sent to your email.";
                        ModelState.Clear();

                        return RedirectToAction("Login");

                        ////////////////////  insert a row into T_ONLINE_BO_AccHolderImages


                        //oConnection = new SqlConnection(SqlConn);

                        //query = "INSERT INTO T_ONLINE_BO_REG([TRACKINGNO],[FIRSTNAME],[LASTNAME],[EMAIL],[MOBILE],[PASS],[USERID],[pDATE],[IsActive],[IsApp1],[IsApp2]) VALUES(@trackingno,@FIRSTNAME,@LASTNAME,@EMAIL,@MOBILE,@PASS,@USERID,GETDATE(),1,0,0);INSERT INTO T_ONLINE_BO_AccountHolder(TRACKINGNO,fFirstName,fLastName,fMobile,fEmail) VALUES(@trackingno,@FIRSTNAME,@LASTNAME,@MOBILE,@EMAIL);INSERT INTO T_ONLINE_BO_AccHolderBANK(TRACKINGNO) VALUES(@trackingno);INSERT INTO T_ONLINE_BO_AccHolderAuthorize(TRACKINGNO) VALUES(@trackingno);INSERT INTO T_ONLINE_BO_AccHolderNominee(TRACKINGNO) VALUES(@trackingno);INSERT INTO T_ONLINE_BO_AccHolderImages(TRACKINGNO) VALUES(@trackingno)";
                        //query = "INSERT INTO T_ONLINE_BO_AccHolderImages([TRACKINGNO]) VALUES('"+ bomodel.TrackingNo + "')";

                        //oCommand = new SqlCommand(query, oConnection);

                        //oConnection.Open();
                        //int i = oCommand.ExecuteNonQuery();
                        //oConnection.Close();
  


                    }
                    else
                        ViewBag.Message = "Registration Failed.Please check your Registration Details!!!";
                }

                return View();
                //return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message2 = "Registration Failed.Email Not Sent.Please check your Registration Details!!!";
                return View();
            }
        }


        public ActionResult Login()
        {
          //  ViewBag.Message1 = "Please check your email. A temporary User ID and Password sent to your email.";

            return View();
        }

       




        [HttpPost]
        public ActionResult Login(HomeModel bomodel)
        {




            int loginFlag = 0;

            string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();

            SqlConnection oConnection = new SqlConnection(SqlConn);

            string query = "SELECT TRACKINGNO,PASS,FIRSTNAME,LASTNAME, MOBILE, IsApp1,IsEkycValid FROM T_ONLINE_BO_REG WHERE TRACKINGNO=@tracking AND PASS=@pass";

            SqlCommand oCommand = new SqlCommand(query, oConnection);
            oCommand.Parameters.AddWithValue("@tracking",bomodel.TrackingNo);
            oCommand.Parameters.AddWithValue("@pass", bomodel.Password);

            oConnection.Open();

            DataTable oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader());

            foreach(DataRow dr in oTable.Rows)
            {
                if((dr["TRACKINGNO"].ToString()==Convert.ToString(bomodel.TrackingNo)) && (dr["PASS"].ToString()==bomodel.Password))
                {

                    ////checking the application status weather approved or un approved
                    if(dr["IsApp1"].ToString()=="1")
                    {
                        oConnection.Close();
                        ViewBag.LoginFailed = "Your Online BO Application is approved. Your Client Code is: "+ParGlobalClass.get_client_code_from_tracking(bomodel.TrackingNo)+". Please use our other services.";
                        return View();

                    }


                    loginFlag = 1;

                    Session["trackingno"] = bomodel.TrackingNo;
                    Session["username"] = dr["FIRSTNAME"].ToString() + ' ' + dr["LASTNAME"].ToString();
                    Session["MOBILE"] = dr["MOBILE"].ToString();

                    oConnection.Close();

                    if(Convert.ToInt16(dr["IsEkycValid"].ToString())==1)
                        return RedirectToAction("Index", "Client");

                    if (Convert.ToInt16(dr["IsEkycValid"].ToString()) == 0)
                        return RedirectToAction("EkycIndex", "Client");
                }
            }
            if (loginFlag==0)
                {

                    ////////////////////////////////// If login failed for internet user then try to login for Branch Users

                    string queryBranch = "SELECT [BRANCHCODE],[BRANCHNAME],[USERID],[USERTYPE],[UESRNAME],[password],[LOCK]FROM [dbo].[T_BRANCH_LOGIN] WHERE LOCK=1 AND USERID=@tracking";

                    oCommand = new SqlCommand(queryBranch, oConnection);
                    oCommand.Parameters.AddWithValue("@tracking", Convert.ToString(bomodel.TrackingNo));

                    oTable = new DataTable();
                    oTable.Load(oCommand.ExecuteReader());

                    foreach(DataRow dr in oTable.Rows)
                    {
                        if ((dr["USERID"].ToString() == Convert.ToString(bomodel.TrackingNo)) && (dr["password"].ToString() == bomodel.Password))
                        {
                            loginFlag = 1;

                            Session["BRANCHNAME"] = dr["BRANCHNAME"].ToString();
                            Session["USERID"] = dr["USERID"].ToString();
                            Session["USERTYPE"] = dr["USERTYPE"].ToString();
                            Session["UESRNAME"] = dr["UESRNAME"].ToString();

                            oConnection.Close();

                            if(dr["USERTYPE"].ToString()=="ADMIN")
                                return RedirectToAction("Index", "Admin");

                            if (dr["USERTYPE"].ToString() == "MANAGER")
                                return RedirectToAction("Index", "Manager");

                            if ((dr["USERTYPE"].ToString() == "BRANCH") || (dr["USERTYPE"].ToString() == "AGENT"))
                                return RedirectToAction("Index", "Branch");

                        }
                    }
                    if (loginFlag == 0)
                    {

                        oConnection.Close();
                        ViewBag.LoginFailed = "Login Failed!!! Please Check Your User ID and Password.";
                        return View();
                    }
                }

            

            return View();
            
        }

    }
}