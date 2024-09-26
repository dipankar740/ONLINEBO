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
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using System.Collections.Specialized;

namespace ONLINEBO.Controllers
{
    public class BranchController : Controller
    {

        private readonly string storeID = "royalcapitalbdlive";
        private readonly string storePassword = "5EC10D5D9D82013542";

        string totalAmount = "450";


        // GET: /Branch/
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
                return RedirectToAction("Index","Home");
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

                //ViewBag.CreateToken = "Failed to Create Token For Create New BO!!!";

                foreach (DataRow dr in oTable.Rows)
                {
                    trackingno = "BO" + dr["Tmax"].ToString();
                    Session["Trackingno"] = trackingno;


                    query = "INSERT INTO T_ONLINE_BO_REG([TRACKINGNO],[EMAIL],[MOBILE],[PASS],[USERID],[pDATE],[IsActive],[IsApp1],[IsApp2]) VALUES(@trackingno,@EMAIL,@MOBILE,@PASS,@USERID,GETDATE(),1,0,0);" +
                        "INSERT INTO T_ONLINE_BO_AccountHolder(TRACKINGNO,fMobile,fEmail) VALUES(@trackingno,@MOBILE,@EMAIL);" +
                        "INSERT INTO T_ONLINE_BO_AccHolderBANK(TRACKINGNO) VALUES(@trackingno);INSERT INTO T_ONLINE_BO_AccHolderAuthorize(TRACKINGNO) VALUES(@trackingno);" +
                        "INSERT INTO T_ONLINE_BO_AccHolderNominee(TRACKINGNO) VALUES(@trackingno);" +
                        "INSERT INTO T_ONLINE_BO_AccHolderImages(TRACKINGNO) VALUES(@trackingno)";

                    oCommand = new SqlCommand(query, oConnection);

                    oCommand.Parameters.AddWithValue("@trackingno", trackingno);
                    oCommand.Parameters.AddWithValue("@EMAIL", bomodel.Email);
                    oCommand.Parameters.AddWithValue("@MOBILE", bomodel.Mobile);
                    oCommand.Parameters.AddWithValue("@PASS", "@@@@@@");
                    oCommand.Parameters.AddWithValue("@USERID", @Session["USERID"].ToString());

                    int res = oCommand.ExecuteNonQuery();

                    if (res > 0)
                    {
                        oConnection.Close();
                        //return RedirectToAction("AccountHolder", "Branch", new { id = trackingno });
                        return RedirectToAction("EkycBranchIndex", "Branch");
                    }
                    else
                    {
                        oConnection.Close();
                        TempData["message"] = "Failed to Create Token For Create New BO!!!";
                    }
                }
            }
            catch (Exception ex)
            {
                oConnection.Close();
                TempData["message"] = "Failed to Create Token For Create New BO!!!";

            }

            oConnection.Close();

            
            return View();
        }

        public ActionResult EkycBranchIndex()
        {
            if (Session["trackingno"] != null)
            {
                ViewData["imageForEKYC"] = GetImagesForEkyc().ToList().FirstOrDefault(); ///////////////  Get All Stored Images

            }
            else
            {
                return RedirectToAction("index", "Home");
            }



            return View();
        }

        [HttpPost]
        public async Task<ActionResult> EkycBranchIndex(EkycModel img, string id)
        {
            if (img.mobile_otp != @Session["OTP"].ToString())
            {
                TempData["message"] = "Your OTP Doesn't Match, Please try again !!";
                return RedirectToAction("EkycBranchIndex", "Branch");
            }
            if (img.File == null && img.person_photo == null)
            {
                TempData["message"] = "Please Upload or Capture Your Image";
                return RedirectToAction("EkycBranchIndex", "Branch");
            }
            if (img.File != null && !(img.File.ContentType == "image/jpeg"))
            {
                TempData["message"] = "Please Upload or Capture Your Image/JPEG";
                return RedirectToAction("EkycBranchIndex", "Branch");
            }
            if (img.person_photo != null)
            {
                OtherDBHandler DB = new OtherDBHandler();
                SqlConnection oConnection = null;
                Session["OTP"] = null;

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
                                            oCommand = new SqlCommand("UPDATE [T_ONLINE_BO_AccountHolder] SET fFirstName= '" + firstName + "',fLastName='" + lastName + "',fDoB='" + dfm.voter.dob + "',fFName='" + dfm.voter.fatherEn + "',fMName='" + dfm.voter.motherEn + "',fAddress='" + dfm.voter.presentAddressEn + "',fNID='" + dfm.voter.nationalId + "' WHERE TRACKINGNO='" + @Session["trackingno"].ToString() + "'", oConnection);
                                            oCommand.ExecuteNonQuery();
                                            /////////////// End Upadte T_ONLINE_BO_AccountHolder Table /

                                            return RedirectToAction("AccountHolder", "Branch", new { id = Session["trackingno"].ToString() });
                                            // return RedirectToAction("Index", "Client");


                                        }
                                        else
                                        {
                                            TempData["message"] = "Your face doesn't match with NID server. Please try again later!!!";
                                            return RedirectToAction("EkycBranchIndex", "Branch");
                                        }
                                    }
                                    else
                                    {
                                        TempData["message"] = "No Data Found From NID Server !!! Plese Try Again Later";
                                        return RedirectToAction("EkycBranchIndex", "Branch");
                                    }
                                }
                                else
                                {
                                    TempData["message"] = "No Data Found From NID Server !!! Plese Try Again Later";
                                    return RedirectToAction("EkycBranchIndex", "Branch");
                                }
                            }
                            else
                            {
                                TempData["message"] = "No Data Found From NID Server !!! Plese Try Again Later";
                                return RedirectToAction("EkycBranchIndex", "Branch");
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
                Session["OTP"] = null;

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
                                            oCommand = new SqlCommand("UPDATE [T_ONLINE_BO_AccountHolder] SET fFirstName= '" + firstName + "',fLastName='" + lastName + "',fDoB='" + dfm.voter.dob + "',fFName='" + dfm.voter.fatherEn + "',fMName='" + dfm.voter.motherEn + "',fAddress='" + dfm.voter.presentAddressEn + "',fNID='" + dfm.voter.nationalId + "' WHERE TRACKINGNO='" + @Session["trackingno"].ToString() + "'", oConnection);
                                            oCommand.ExecuteNonQuery();
                                            /////////////// End Upadte T_ONLINE_BO_AccountHolder Table /

                                            //return RedirectToAction("Index", "Client");
                                            return RedirectToAction("AccountHolder", "Branch", new { id = Session["trackingno"].ToString() });


                                        }
                                        else
                                        {
                                            TempData["message"] = "Your face doesn't match with NID server. Please try again later!!!";
                                            return RedirectToAction("EkycBranchIndex", "Branch");
                                        }
                                    }
                                    else
                                    {
                                        TempData["message"] = "Somthing Went Wrong!!!. Please try again later!!!";
                                        return RedirectToAction("EkycBranchIndex", "Branch");
                                    }
                                }
                                else
                                {
                                    TempData["message"] = "No Data Found From NID Server !!! Plese Try Again Later";
                                    return RedirectToAction("EkycBranchIndex", "Branch");
                                }
                            }
                            else
                            {
                                TempData["message"] = "No Data Found From NID Server !!! Plese Try Again Later";
                                return RedirectToAction("EkycBranchIndex", "Branch");
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        //ViewData.Add(id, "Error: " + ex.Message);
                        //ViewBag.ImgUpSuccess = "Error: " + ex.Message;
                        TempData["message"] = "Error: " + ex.Message;
                        return RedirectToAction("EkycBranchIndex", "Branch");
                    }
                }
                finally
                {
                    if (oConnection != null)
                        oConnection.Close();
                }
            }


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

            var apiUrl = "https://porichoy.azurewebsites.net/api/v0/kyc/nid-person-values-image-match";
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

        public ActionResult AccountHolder()
        {
            if (Session["Trackingno"] == null)
            {
                return RedirectToAction("Index", "Branch");
            }
            var id = Session["Trackingno"].ToString();

            BranchDBHandler BDB = new BranchDBHandler();
            ViewData["AccHolderSelect"] = BDB.AccHolderSelect(id).ToList();

            OtherDBHandler DB1 = new OtherDBHandler();
            ViewBag.Title = DB1.GetTitle().ToList();
           // ViewBag.MessageAcc = "Account Holder Information Updated Failed!!!";
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
                        return RedirectToAction("BankInfo/0", "Branch");
                    }
                    else
                    {
                        ViewBag.MessageAcc = "Account Holder Information Updated Failed!!!";
                       // Session["MessageAcc1"] = "Account Holder Information Updated Failed!!!";
                        return View();
                    }
                        
                }
                //else
                //    ViewBag.MessageNominee2 = ModelState.
                ViewBag.MessageAcc = "Account Holder Information Updated Failed!!!";
                return View();
                //return RedirectToAction("BankInfo", "Client");
            }
            catch
            {
               // Session["MessageAcc1"] = "DataBase Error...Account Holder Information Updated Failed!!!";
                ViewBag.MessageAcc = "DataBase Error...Account Holder Information Updated Failed!!!";
                return View();
            }
        }

        public ActionResult AccountHolderInfoRecv(string id)
        {
            try
            {
                Session["Trackingno"] = id;
                string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();

                SqlConnection oConnection = new SqlConnection(SqlConn);

                string query = "SELECT TRACKINGNO,PASS,FIRSTNAME,LASTNAME, MOBILE, IsApp1,IsEkycValid FROM T_ONLINE_BO_REG WHERE TRACKINGNO=@tracking";

                SqlCommand oCommand = new SqlCommand(query, oConnection);
                oCommand.Parameters.AddWithValue("@tracking", id);

                oConnection.Open();

                DataTable oTable = new DataTable();
                oTable.Load(oCommand.ExecuteReader());

                foreach (DataRow dr in oTable.Rows)
                {

                    if (Convert.ToInt16(dr["IsEkycValid"].ToString()) == 1)
                        return RedirectToAction("AccountHolder", "Branch");

                    if (Convert.ToInt16(dr["IsEkycValid"].ToString()) == 0)
                        return RedirectToAction("EkycBranchIndex", "Branch");
                }

                oConnection.Close();
            }
            catch (Exception)
            {

                throw;
            }
            return View();

        }

        public ActionResult BankInfo(int id)
        {
            if (Session["Trackingno"] == null)
            {
                return RedirectToAction("Index", "Branch");
            }
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
                        if (dr["BANKNAME"].ToString()!="")
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
                        return RedirectToAction("AuthorizeInfo", "Branch");
                    }
                    else
                    {
                        ViewBag.MessageBank = "Your Bank Information Insertion Failed!!!";
                        // Session["MessageBank"] = "Your Bank Information Insertion Failed!!!";
                        return View();
                    }
                        
                }
                //else
                //    ViewBag.MessageNominee2 = ModelState.
                ViewBag.MessageBank = "Your Bank Information Insertion Failed!!!";
                return View();
                //return RedirectToAction("AuthorizeInfo", "Branch");
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
            if (Session["Trackingno"] == null)
            {
                return RedirectToAction("Index", "Branch");
            }
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

                        Session["MessageAuth"] = "Authorize Info successfully Inserted.";
                        return RedirectToAction("NomineeInfo", "Branch");
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
            if (Session["Trackingno"] == null)
            {
                return RedirectToAction("Index", "Branch");
            }
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
                        Session["MessageNominee"] = "Nominee Info successfully Inserted.";
                        return RedirectToAction("UploadImages", "Branch");

                    }
                    else
                    {
                        ViewBag.MessageNominee2 = "Insertion Failed!!!";
                        return View();
                    }
                }
                //else
                //    ViewBag.MessageNominee2 = ModelState.

               // return RedirectToAction("UploadImages", "Branch");
                return View();
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
            SqlDataAdapter DA=new SqlDataAdapter();
            
            ///// Account Holder
            string query = "SELECT [TRACKINGNO],[RCODE],[BO],[ToC],[fFirstName],[fLastName],[fOccupation],[fDoB],[fTitle],[fFName],[fMName],[fAddress],[fCity],[fPostCode],[fDivision],[fCountry],[fMobile],[fTel],[fFax],[fEmail],[fNationality],[fNID],[fTIN],[fSex],[fResidency],[jTitle],[jFirstName],[jLastName],[jOccupation],[jDoB],[jFName],[jMName],[jAddress],[jCity],[jPostCode],[jDivision],[jCountry],[jMobile],[jTel],[jFax],[jEmail],[jNID],[DesireBranch],[IsDirector],[DirectorShare],[pDate],[fIsComplete] FROM [dbo].[T_ONLINE_BO_AccountHolder] WHERE TRACKINGNO='" + id + "'";
            DA=new SqlDataAdapter(query,oConnection);
            DA.Fill(DS,"ACHOLDER");

            /////  Bank information 
            query = "SELECT [TRACKINGNO],[BANKNAME],[BANKBRANCH],[DISTRICT],[ROUTING],[AC],[pDate],[bIsComplete] FROM [dbo].[T_ONLINE_BO_AccHolderBANK] WHERE TRACKINGNO='"+id+"'";
            DA=new SqlDataAdapter(query,oConnection);
            DA.Fill(DS,"ACBANK");

            //// Account Holder Authorize info
            query = "SELECT [TRACKINGNO],[aTitle],[aFirstName],[aLastName],[aOccupation],[aDoB],[aFName],[aMName],[aAddress],[aCity],[aPostCode],[aDivision],[aCountry],[aMobile],[aTel],[aFax],[aEmail],[aNID],[pDate] FROM [dbo].[T_ONLINE_BO_AccHolderAuthorize] WHERE TRACKINGNO='"+id+"'";
            DA=new SqlDataAdapter(query,oConnection);
            DA.Fill(DS,"ACAUT");

            ///////////  Account Holder Nominee
            query = "SELECT [TRACKINGNO],[n1Title],[n1FirstName],[n1LastName],[n1RelWithACHolder],[n1Percentage],[n1IsResident],[n1DoB],[n1NID],[n1Address],[n1City],[n1PostCode],[n1Division],[n1Country],[n1Mobile],[n1Tel],[n1Fax],[n1Email],[n2Title],[n2FirstName],[n2LastName],[n2RelWithACHolder],[n2Percentage],[n2IsResident],[n2DoB],[n2NID],[n2Address],[n2City],[n2PostCode],[n2Division],[n2Country],[n2Mobile],[n2Tel],[n2Fax],[n2Email],[g1Title],[g1FirstName],[g1LastName],[g1RelWithNominee],[g1DoBMinor],[g1MaturityDoM],[g1IsResident],[g1DoB],[g1NID],[g1Address],[g1City],[g1PostCode],[g1Division],[g1Country],[g1Mobile],[g1Tel],[g1Fax],[g1Email],[g2Title],[g2FirstName],[g2LastName],[g2RelWithNominee],[g2DoBMinor],[g2MaturityDoM],[g2IsResident],[g2DoB],[g2NID],[g2Address],[g2City],[g2PostCode],[g2Division],[g2Country],[g2Mobile],[g2Tel],[g2Fax],[g2Email],[pDate],[nIsComplete] FROM [dbo].[T_ONLINE_BO_AccHolderNominee] WHERE TRACKINGNO='"+id+"'";
            DA=new SqlDataAdapter(query,oConnection);
            DA.Fill(DS,"ACNO");

            ///// Account Holder Image  ////
            query = "SELECT [TRACKINGNO],[fImage],[fNIDFont],[fNIDBack],[fSig],[jImage],[jNIDFont],[jNIDBack],[jSig],[aImage],[aNIDFont],[aNIDBack],[aSig],[n1Image],[n1NIDFont],[n1NIDBack],[n1Sig],[n2Image],[n2NIDFont],[n2NIDBack],[n2Sig],[g1Image],[g1NIDFont],[g1NIDBack],[g1Sig],[g2Image],[g2NIDFont],[g2NIDBack],[g2Sig],[pDate],[iIsComplete] FROM [dbo].[T_ONLINE_BO_AccHolderImages] WHERE TRACKINGNO='"+id+"'";
            DA=new SqlDataAdapter(query,oConnection);
            DA.Fill(DS,"ACIMG");


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

        //public ActionResult Payment()
        //{
        //    string year1="", year2="";

        //    var yearFrom = new List<SelectListItem>();
        //    var yearTo = new List<SelectListItem>();

        //    if(DateTime.Now.Month<=6)
        //    {
        //        year1 = (DateTime.Now.Year - 1).ToString(); 
        //        year2 = (DateTime.Now.Year).ToString();
        //    }
        //    if (DateTime.Now.Month > 6)
        //    {
        //        year1 = (DateTime.Now.Year).ToString();
        //        year2 = (DateTime.Now.Year+1).ToString();
        //    }

        //    yearFrom.Add(new SelectListItem { Text = Convert.ToInt16(year1).ToString(), Value = Convert.ToInt16(year1).ToString() });

        //    yearTo.Add(new SelectListItem { Text = Convert.ToInt16(year2).ToString(), Value = Convert.ToInt16(year2).ToString() });
        //    yearTo.Add(new SelectListItem { Text = (Convert.ToInt16(year2) + 1).ToString(), Value = (Convert.ToInt16(year2) + 1).ToString() });
        //    yearTo.Add(new SelectListItem { Text = (Convert.ToInt16(year2) + 2).ToString(), Value = (Convert.ToInt16(year2) + 2).ToString() });


        //    ViewBag.yearFrom = yearFrom;
        //    ViewBag.yearTo = yearTo;


        //    ////////////////////////////////////  GET BO and CDBL Charge ///////////

        //    string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();

        //    SqlConnection oConnection = new SqlConnection(SqlConn);
        //    oConnection.Open();

        //    string query = "SELECT [BOCHARGE],[CDBLCHARGE] FROM T_BOANDCDBLCHARGE WHERE BRANCHCODE=(SELECT TOP 1 BRANCHCODE FROM T_BRANCH_LOGIN WHERE BRANCHNAME=@branchname)";

        //    SqlCommand oCommand = new SqlCommand(query, oConnection);
        //    oCommand.Parameters.AddWithValue("@branchname", Session["BRANCHNAME"].ToString());

        //    DataTable oTable = new DataTable();
        //    oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

        //    foreach (DataRow dr in oTable.Rows)
        //    {
        //        ViewBag.BOCHARGE = dr["BOCHARGE"].ToString();
        //        ViewBag.CDBLCHARGE = dr["CDBLCHARGE"].ToString();
        //    }

        //    //////////////////  Check if Previous Rcode found for this tracking no 


        //    ViewBag.CLIENTCODE = "Client Code Not Yet Created...";

        //    SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();

        //    oConnection = new SqlConnection(SqlConn);
        //    oConnection.Open();

        //    query = "SELECT * FROM [T_ONLINE_BO_AccountHolderPayment] WHERE TRACKINGNO=@trackingno";

        //    oCommand = new SqlCommand(query, oConnection);
        //    oCommand.Parameters.AddWithValue("@trackingno", @Session["Trackingno"].ToString());

        //    oTable = new DataTable();
        //    oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

        //    foreach (DataRow dr in oTable.Rows)
        //    {
        //        ViewBag.CLIENTCODE = "Already Paid...";
        //        //ViewBag.RCODE = dr["RCODE"].ToString();
        //    }
        //    ////////////////// End of Check if Previous Rcode found for this tracking no 

            
        //    ////////////////////////////////////  END OF GET BO and CDBL Charge ///////////


        //    ////////////////////////////////////  GET Full Name of First Applicant ///////////


        //    oConnection = new SqlConnection(SqlConn);
        //    oConnection.Open();

        //    query = "SELECT (fFirstName + ' '+ fLastName) fullname FROM T_ONLINE_BO_AccountHolder WHERE TRACKINGNO=@trackingno";

        //    oCommand = new SqlCommand(query, oConnection);
        //    oCommand.Parameters.AddWithValue("@trackingno", Session["Trackingno"].ToString());

        //    oTable = new DataTable();
        //    oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

        //    foreach (DataRow dr in oTable.Rows)
        //    {
        //        ViewBag.FULLNAME = dr["fullname"].ToString();
        //    }

        //    ////////////////////////////////////  END OF Full Name of First Applicant ///////////


        //    return View();
        //}


        public string Get_Latest_RCODE_BRANCH(string branch_name)
        {
            string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();
            string RCODE = "";

            try
            {

                SqlConnection oConnection = new SqlConnection(SqlConn);
                oConnection.Open();

                string queryMAX = "SELECT RCODE FROM T_NEW_CLIENT WHERE BRANCHCODE IN (SELECT TOP 1 [BRANCHCODE] FROM [T_BRANCH_LOGIN] WHERE [BRANCHNAME]='" + branch_name + "') AND EntryTime=(SELECT MAX(EntryTime) FROM T_NEW_CLIENT WHERE BRANCHCODE IN (SELECT TOP 1 [BRANCHCODE] FROM [T_BRANCH_LOGIN] WHERE [BRANCHNAME]='" + branch_name + "'))";

                if (branch_name == "Corporate Office")
                {
                    queryMAX = "SELECT RCODE FROM T_NEW_CLIENT WHERE BRANCHCODE IN (1) AND EntryTime=(SELECT MAX(EntryTime) FROM T_NEW_CLIENT WHERE BRANCHCODE IN (1))";
                }
                if (branch_name == "Corporate Extention Office")
                {
                    queryMAX = "SELECT RCODE FROM T_NEW_CLIENT WHERE BRANCHCODE IN (33) AND EntryTime=(SELECT MAX(EntryTime) FROM T_NEW_CLIENT WHERE BRANCHCODE IN (33))";
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
                    oCommandMAX.Parameters.AddWithValue("@username",userID);

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

        //public ActionResult Payment()
        //{
        //    if (Session["trackingno"] != null)
        //    {
        //        string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();
        //        SqlConnection oConnection = new SqlConnection(SqlConn);
        //        oConnection.Open();
        //        string qry = "SELECT * FROM [T_ONLINE_PAYMENT_RECEIVER] WHERE [tracking_no]='" + Session["trackingno"].ToString() + "'";
        //        SqlCommand oCommand = new SqlCommand(qry, oConnection);
        //        DataTable otable = new DataTable();
        //        otable.Load(oCommand.ExecuteReader());
        //        oConnection.Close();
        //        ViewBag.paid = "unpaid";
        //        foreach (DataRow dr in otable.Rows)
        //        {
        //            ViewBag.paid = "paid";
        //        }

        //        return View();
        //    }
        //    else
        //        return RedirectToAction("index", "Branch");
        //}

        //[HttpPost]
        //public ActionResult Payment(OnlineBODetailModel bomodel)
        //{
        //    string RCODE = "";

        //    string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();

        //    SqlConnection oConnection = new SqlConnection(SqlConn);


        //    if (@Session["USERTYPE"].ToString() == "BRANCH")
        //    {
        //        RCODE = Get_Latest_RCODE_BRANCH(@Session["BRANCHNAME"].ToString());
        //    }
        //    if (@Session["USERTYPE"].ToString() == "AGENT")
        //    {
        //        RCODE = Get_Latest_RCODE_AGENT(@Session["USERID"].ToString());
        //    }



        //    if (RCODE != "")
        //    {
        //        oConnection.Open();
        //    /////////////// insert into bo charge //////////  (CONVERT(VARCHAR,(SELECT MAX(SL_NO)+1 FROM T_BO_CHARGE))+CONVERT(VARCHAR,(SELECT TOP 1 BRANCHCODE FROM T_BRANCH_LOGIN WHERE BRANCHNAME=@branchname))+(SELECT REPLACE(CONVERT(VARCHAR(10),GETDATE(),110),'-','')))
        //    string query = "INSERT INTO T_BO_CHARGE ([SL_NO],[MR_NO],[NAME],[QTY],[AMOUNT],[BRANCHCODE],[DATE],[NOTE]) VALUES((SELECT MAX(SL_NO)+1 FROM T_BO_CHARGE) , @MR_NO,@fullName,1,@amount,(SELECT TOP 1 BRANCHCODE FROM T_BRANCH_LOGIN WHERE BRANCHNAME=@branchname) ,(SELECT CONVERT(VARCHAR,GETDATE(),101)) ,''   )";

        //    SqlCommand oCommand = new SqlCommand(query, oConnection);
        //    oCommand.Parameters.AddWithValue("@branchname", @Session["BRANCHNAME"].ToString());
        //    oCommand.Parameters.AddWithValue("@MR_NO", bomodel.TRACKINGNO.ToString());
        //    oCommand.Parameters.AddWithValue("@fullName", bomodel.fullName);
        //    oCommand.Parameters.AddWithValue("@amount", bomodel.BOCHARGE);

        //    int res = oCommand.ExecuteNonQuery();
        //    /////////////// end of insert into bo charge //////////

        //    ////////////////  CREATE and Insert CLIENT CODE ///////////////////////
        //    //if (res > 0)
        //    //{



        //    //}

        //    int cdblpay = 0;

        //        //oConnection = new SqlConnection(SqlConn);
        //        //if()

        //        query = "INSERT INTO T_NEW_CLIENT(RCODE,NAME,BRANCHCODE,[USER],EntryTime) VALUES(@rcode,@fullname,(SELECT TOP 1 [BRANCHCODE] FROM T_BRANCH_LOGIN WHERE USERID=@userid),@userid,getDate())";
        //        oCommand = new SqlCommand(query, oConnection);
        //        oCommand.Parameters.AddWithValue("@rcode", RCODE);
        //        oCommand.Parameters.AddWithValue("@fullname", bomodel.fullName);
        //        oCommand.Parameters.AddWithValue("@userid", @Session["USERID"].ToString());

        //        if (oCommand.ExecuteNonQuery() > 0)
        //        {
                    
        //            int year1 ;
        //            int year2 ;

        //            int tyear = bomodel.DateTo - bomodel.DateFrom;

        //            for (int i = 0; i < tyear; i++)
        //            {
        //                year1 = bomodel.DateFrom+i;
        //                year2 = year1+1;
        //                /////////////// insert into CDBL charge ////////// (CONVERT(VARCHAR,(SELECT MAX(ROW_ID)+1 FROM T_CDBL_CHARGE))+CONVERT(VARCHAR,(SELECT TOP 1 BRANCHCODE FROM T_BRANCH_LOGIN WHERE BRANCHNAME=@branchname))+(SELECT REPLACE(CONVERT(VARCHAR(10),GETDATE(),110),'-','')))
        //                query = "INSERT INTO T_CDBL_CHARGE([ROW_ID],[MR_NO],[RCODE],[YEAR],[FISCAL],[AMOUNT],[BRANCHCODE],[DATE],[NOTE],[fis],[tamnt],[NAME]) VALUES ((SELECT MAX(ROW_ID)+1 FROM T_CDBL_CHARGE), @MR_NO,@rcode,@year2,('July '+CONVERT(VARCHAR,@year1) +' - '+'June '+CONVERT(VARCHAR,@year2)) ,@amount,(SELECT TOP 1 BRANCHCODE FROM T_BRANCH_LOGIN WHERE BRANCHNAME=@branchname) ,(SELECT CONVERT(VARCHAR,GETDATE(),101)) ,'Cash Received' ,('July-'+CONVERT(VARCHAR,@from) +'To '+'June-'+CONVERT(VARCHAR,@to)) ,@tamnt,@fullName   )";

        //                oCommand = new SqlCommand(query, oConnection);
        //                oCommand.Parameters.AddWithValue("@branchname", @Session["BRANCHNAME"].ToString());
        //                oCommand.Parameters.AddWithValue("@MR_NO", bomodel.TRACKINGNO.ToString());
        //                oCommand.Parameters.AddWithValue("@fullName", bomodel.fullName);
        //                oCommand.Parameters.AddWithValue("@rcode", RCODE);
        //                oCommand.Parameters.AddWithValue("@year1", year1);
        //                oCommand.Parameters.AddWithValue("@year2", year2);
        //                oCommand.Parameters.AddWithValue("@amount", bomodel.CDBLCHARGE);
        //                oCommand.Parameters.AddWithValue("@tamnt", bomodel.TOTALCHARGE);
        //                oCommand.Parameters.AddWithValue("@from", bomodel.DateFrom);
        //                oCommand.Parameters.AddWithValue("@to", bomodel.DateTo);

        //                cdblpay=oCommand.ExecuteNonQuery();
        //            }

        //            /////////////// end of insert into CDBL charge //////////

        //            if (cdblpay > 0)   /// if payment Successfull then update rcode in account Holder table 
        //            {
        //                query = "UPDATE [T_ONLINE_BO_AccountHolder] SET RCODE=@rcode WHERE TRACKINGNO=@trackingno";
        //                oCommand = new SqlCommand(query, oConnection);
        //                oCommand.Parameters.AddWithValue("@trackingno", bomodel.TRACKINGNO.ToString());
        //                oCommand.Parameters.AddWithValue("@rcode", RCODE);
        //            }
        //            if (oCommand.ExecuteNonQuery() > 0)   /// if payment Successfull then update rcode in account Holder table 
        //            {
                                              
        //                query = "INSERT INTO [T_ONLINE_BO_AccountHolderPayment](TRACKINGNO,PAYMENTTYPE,AMOUNT,pDate) VALUES(@trackingno,@payType,@amount,GETDATE())";
        //                oCommand = new SqlCommand(query, oConnection);
        //                oCommand.Parameters.AddWithValue("@trackingno",  bomodel.TRACKINGNO.ToString());
        //                oCommand.Parameters.AddWithValue("@payType", "CASH");
        //                oCommand.Parameters.AddWithValue("@amount", bomodel.TOTALCHARGE);


        //                @ViewBag.CLIENTCODE = RCODE;
        //                oCommand.ExecuteNonQuery();

        //                //////////////////////  update user ////////////
        //                query = "UPDATE [T_ONLINE_BO_REG] SET USERID=@user WHERE TRACKINGNO=@trackingno";
        //                oCommand = new SqlCommand(query, oConnection);
        //                oCommand.Parameters.AddWithValue("@trackingno", bomodel.TRACKINGNO.ToString());
        //                oCommand.Parameters.AddWithValue("@user", @Session["USERID"].ToString());

        //                oCommand.ExecuteNonQuery();


        //                oConnection.Close();

        //                //return RedirectToAction("Payment");
        //            }

        //        }

        //        oConnection.Close();
        //    }

        //    @ViewBag.CLIENTCODE = "Unable to create Client Code!!!";
        //    //////////////// End of CREATE and Insert CLIENT CODE ///////////////////////



        //    return RedirectToAction("PrintMR", new { id = bomodel.TRACKINGNO.ToString() });
        //}


        public ActionResult PrintMR(string id)
        {
            string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();

            var incompleteList = new List<OnlineBODetailModel>();
            SqlConnection oConnection = new SqlConnection(SqlConn);
            oConnection.Open();
            string query = "SELECT TOP 1 [MR_NO],[RCODE],[DATE],[fis],[tamnt],[NAME] FROM [T_CDBL_CHARGE] where MR_NO=@MR_NO";

            SqlCommand oCommand = new SqlCommand(query, oConnection);
            oCommand.Parameters.AddWithValue("@MR_NO",id);

            DataTable oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

            foreach (DataRow dr in oTable.Rows)
            {
                incompleteList.Add(new OnlineBODetailModel { MR_NO = dr["MR_NO"].ToString(), RCODE = dr["RCODE"].ToString(), DATE = dr["DATE"].ToString(), fis = dr["fis"].ToString(), tamnt = dr["tamnt"].ToString(), NAME = dr["NAME"].ToString() });
            }

            ViewData["printmr"] = incompleteList.ToList();


            //////////////////////////////////////////////

            string year1 = "", year2 = "";

            var yearFrom = new List<SelectListItem>();
            var yearTo = new List<SelectListItem>();

            if (DateTime.Now.Month <= 6)
            {
                year1 = (DateTime.Now.Year - 1).ToString();
                year2 = (DateTime.Now.Year).ToString();
            }
            if (DateTime.Now.Month > 6)
            {
                year1 = (DateTime.Now.Year).ToString();
                year2 = (DateTime.Now.Year + 1).ToString();
            }

            yearFrom.Add(new SelectListItem { Text = Convert.ToInt16(year1).ToString(), Value = Convert.ToInt16(year1).ToString() });

            yearTo.Add(new SelectListItem { Text = Convert.ToInt16(year2).ToString(), Value = Convert.ToInt16(year2).ToString() });
            yearTo.Add(new SelectListItem { Text = (Convert.ToInt16(year2) + 1).ToString(), Value = (Convert.ToInt16(year2) + 1).ToString() });
            yearTo.Add(new SelectListItem { Text = (Convert.ToInt16(year2) + 2).ToString(), Value = (Convert.ToInt16(year2) + 2).ToString() });


            ViewBag.yearFrom = yearFrom;
            ViewBag.yearTo = yearTo;


            ////////////////////////////////////  GET BO and CDBL Charge ///////////

            SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();

            oConnection = new SqlConnection(SqlConn);
            oConnection.Open();

            query = "SELECT [BOCHARGE],[CDBLCHARGE] FROM T_BOANDCDBLCHARGE WHERE BRANCHCODE=(SELECT TOP 1 BRANCHCODE FROM T_BRANCH_LOGIN WHERE BRANCHNAME=@branchname)";

            oCommand = new SqlCommand(query, oConnection);
            oCommand.Parameters.AddWithValue("@branchname", "Roy Shaheb Bazar");

            oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

            foreach (DataRow dr in oTable.Rows)
            {
                ViewBag.BOCHARGE = dr["BOCHARGE"].ToString();
                ViewBag.CDBLCHARGE = dr["CDBLCHARGE"].ToString();
            }

            //////////////////  Check if Previous Rcode found for this tracking no 


            ViewBag.CLIENTCODE = "Client Code Not Yet Created...";

            SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();

            oConnection = new SqlConnection(SqlConn);
            oConnection.Open();

            query = "SELECT * FROM [T_ONLINE_BO_AccountHolderPayment] WHERE TRACKINGNO=@trackingno";

            oCommand = new SqlCommand(query, oConnection);
            oCommand.Parameters.AddWithValue("@trackingno", @Session["Trackingno"].ToString());

            oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

            foreach (DataRow dr in oTable.Rows)
            {
                ViewBag.CLIENTCODE = "Already Paid...";
                //ViewBag.CDBLCHARGE = dr["CDBLCHARGE"].ToString();
            }
            ////////////////// End of Check if Previous Rcode found for this tracking no 


            ////////////////////////////////////  END OF GET BO and CDBL Charge ///////////


            ////////////////////////////////////  GET Full Name of First Applicant ///////////


            oConnection = new SqlConnection(SqlConn);
            oConnection.Open();

            query = "SELECT (fFirstName + ' '+ fLastName) fullname FROM T_ONLINE_BO_AccountHolder WHERE TRACKINGNO=@trackingno";

            oCommand = new SqlCommand(query, oConnection);
            oCommand.Parameters.AddWithValue("@trackingno", Session["Trackingno"].ToString());

            oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

            foreach (DataRow dr in oTable.Rows)
            {
                ViewBag.FULLNAME = dr["fullname"].ToString();
            }

            ////////////////////////////////////  END OF Full Name of First Applicant ///////////

            return View();
        }



        public ActionResult EmailMR(string id)
        {
            string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();

            var incompleteList = new List<OnlineBODetailModel>();
            SqlConnection oConnection = new SqlConnection(SqlConn);
            oConnection.Open();
            string query = "SELECT TOP 1 a.[MR_NO],a.[RCODE],Convert(VARCHAR(10),a.[DATE]) [DATE],a.[fis],a.[tamnt],a.[NAME],b.[fEmail] FROM [T_CDBL_CHARGE] a,T_ONLINE_BO_AccountHolder b where MR_NO=@MR_NO AND b.[TRACKINGNO]=a.MR_NO";

            SqlCommand oCommand = new SqlCommand(query, oConnection);
            oCommand.Parameters.AddWithValue("@MR_NO", id);

            DataTable oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

            foreach (DataRow dr in oTable.Rows)
            {
                try
                {

                            MailMessage mail = new MailMessage();
                            SmtpClient SmtpServer = new SmtpClient("159.21.2.246");
                            //SmtpClient SmtpServer = new SmtpClient("mail.royalcapitalbd.com");
                            mail.From = new MailAddress("bo@royalcapitalbd.com", "Royal Capital-Online BO Account Opening");
                            mail.To.Add(dr["fEmail"].ToString());
                            //mail.CC.Add("rafiq@royalcapitalbd.com");
                            // mail.CC.Add("invicta.corp@gmail.com");
                            //mail.CC.Add("rommel75@yahoo.com");
                            //mail.Bcc.Add("dipankar.bnk@gmail.com");
                            //mail.Bcc.Add("bparvej@yahoo.com");
                            mail.Subject = "BO and CDBL Charge Payment Recept";
                            mail.IsBodyHtml=true;
                            mail.Body ="<div id='boReport' style='line-height:20px; width:690px; border:1px solid;border-radius:10px;'>"+
                                       "<table style=' margin-left:20px;'>"+
                    "<tr style='height:30px;'><td><img id='Image1' src='~/images/logoSmall.jpg' alt='Royal Capital' style='height:70px;width:100px;' /></td><td></td><td></td><td></td></tr>"+
                    "<tr style='height:30px;'><td></td><td><b><u>BO and CDBL Charge Money Receipt</u></b></td><td></td><td></td></tr>"+
                    "<tr style='height:30px;'><td>MR #: <span id='lblMR_NO'>" + dr["MR_NO"].ToString() + "</span></td><td></td><td>Date: <span id='lblDate'>'" +  dr["DATE"].ToString() + "'</span></td><td></td></tr>" +
                    "<tr style='height:30px;'><td>Client Code: </td><td style='text-decoration:underline'><span id='lblClientCode'>"+dr["RCODE"].ToString()+"</span></td><td></td><td></td></tr>"+
                    "<tr style='height:30px;'><td>Received with thanks from</td><td style='text-decoration:underline; font-weight:bold;'><span id='lblName'>"+dr["NAME"]+"</span></td><td></td><td></td></tr>"+
                    "<tr style='height:30px;'><td>For CDBL annual charges for the year </td><td style='text-decoration:underline'><span id='lblyear'>  <b>"+dr["fis"].ToString()+" </b> </span></td><td></td><td></td></tr>"+
                    "<tr style='height:30px;'><td>Taka </td><td style='text-decoration:underline'><span id='lblAmount'>"+dr["tamnt"].ToString()+"</span>/-</td><td>in Cash</td><td></td></tr>"+
                    "</table>"+
                    "<br />"+
                    "* This is a computer generated money receipt, do not require any signature."+
                    "<br /><br />"+
                    "</div>";

                            //mail.Attachments.Add(new Attachment(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\parvej.jpg"));

                            SmtpServer.Port = 25;
                            SmtpServer.Credentials = new System.Net.NetworkCredential("bo@royalcapitalbd.com", "rclbd1234%");
                            //SmtpServer.EnableSsl = true;

                            SmtpServer.Send(mail);

                            ViewBag.Message1 = "Email Sent...";
                            ModelState.Clear();

                    }
                 catch
                    {
                        ViewBag.Message1 = "Email Sent Failed...";
                    }
                }




            string year1 = "", year2 = "";

            var yearFrom = new List<SelectListItem>();
            var yearTo = new List<SelectListItem>();

            if (DateTime.Now.Month <= 6)
            {
                year1 = (DateTime.Now.Year - 1).ToString();
                year2 = (DateTime.Now.Year).ToString();
            }
            if (DateTime.Now.Month > 6)
            {
                year1 = (DateTime.Now.Year).ToString();
                year2 = (DateTime.Now.Year + 1).ToString();
            }

            yearFrom.Add(new SelectListItem { Text = Convert.ToInt16(year1).ToString(), Value = Convert.ToInt16(year1).ToString() });

            yearTo.Add(new SelectListItem { Text = Convert.ToInt16(year2).ToString(), Value = Convert.ToInt16(year2).ToString() });
            yearTo.Add(new SelectListItem { Text = (Convert.ToInt16(year2) + 1).ToString(), Value = (Convert.ToInt16(year2) + 1).ToString() });
            yearTo.Add(new SelectListItem { Text = (Convert.ToInt16(year2) + 2).ToString(), Value = (Convert.ToInt16(year2) + 2).ToString() });


            ViewBag.yearFrom = yearFrom;
            ViewBag.yearTo = yearTo;


            ////////////////////////////////////  GET BO and CDBL Charge ///////////


            oConnection = new SqlConnection(SqlConn);
            oConnection.Open();

            query = "SELECT [BOCHARGE],[CDBLCHARGE] FROM T_BOANDCDBLCHARGE WHERE BRANCHCODE=(SELECT TOP 1 BRANCHCODE FROM T_BRANCH_LOGIN WHERE BRANCHNAME=@branchname)";

            oCommand = new SqlCommand(query, oConnection);
            oCommand.Parameters.AddWithValue("@branchname", "Roy Shaheb Bazar");

            oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

            foreach (DataRow dr in oTable.Rows)
            {
                ViewBag.BOCHARGE = dr["BOCHARGE"].ToString();
                ViewBag.CDBLCHARGE = dr["CDBLCHARGE"].ToString();
            }

            //////////////////  Check if Previous Rcode found for this tracking no 


            ViewBag.CLIENTCODE = "Client Code Not Yet Created...";

            SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();

            oConnection = new SqlConnection(SqlConn);
            oConnection.Open();

            query = "SELECT * FROM [T_ONLINE_BO_AccountHolderPayment] WHERE TRACKINGNO=@trackingno";

            oCommand = new SqlCommand(query, oConnection);
            oCommand.Parameters.AddWithValue("@trackingno", @Session["Trackingno"].ToString());

            oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

            foreach (DataRow dr in oTable.Rows)
            {
                ViewBag.CLIENTCODE = "Already Paid...";
                //ViewBag.CDBLCHARGE = dr["CDBLCHARGE"].ToString();
            }
            ////////////////// End of Check if Previous Rcode found for this tracking no 


            ////////////////////////////////////  END OF GET BO and CDBL Charge ///////////


            ////////////////////////////////////  GET Full Name of First Applicant ///////////


            oConnection = new SqlConnection(SqlConn);
            oConnection.Open();

            query = "SELECT (fFirstName + ' '+ fLastName) fullname FROM T_ONLINE_BO_AccountHolder WHERE TRACKINGNO=@trackingno";

            oCommand = new SqlCommand(query, oConnection);
            oCommand.Parameters.AddWithValue("@trackingno", @Session["Trackingno"].ToString());

            oTable = new DataTable();
            oTable.Load(oCommand.ExecuteReader(CommandBehavior.CloseConnection));

            foreach (DataRow dr in oTable.Rows)
            {
                ViewBag.FULLNAME = dr["fullname"].ToString();
            }

            ////////////////////////////////////  END OF Full Name of First Applicant ///////////

            return View("Payment");
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
                return RedirectToAction("Index", "Branch");
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
            string query = "SELECT b.TRACKINGNO,a.NAME,a.AMOUNT,a.[DATE],b.USERID FROM T_BO_CHARGE a,T_ONLINE_BO_REG b WHERE a.MR_NO=b.TRACKINGNO AND b.USERID='" + @Session["USERID"].ToString() + "' AND a.[DATE] BETWEEN '" +  DateTime.ParseExact(bomodel.fDoB, "dd/MM/yyyy", null) + "' AND '" + bomodel.jDoB + "' ";

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

            string query = "SELECT TOP 10 b.TRACKINGNO,a.NAME,SUM(a.AMOUNT) AMOUNT,a.[DATE],b.USERID FROM T_CDBL_CHARGE a,T_ONLINE_BO_REG b WHERE a.MR_NO=b.TRACKINGNO AND b.USERID='" + @Session["USERID"].ToString() + "' AND a.[DATE] BETWEEN '" + DateTime.ParseExact(bomodel.fDoB, "dd/MM/yyyy", null)  + "' AND '" + bomodel.jDoB + "' GROUP BY b.TRACKINGNO,a.NAME,a.[DATE],b.USERID ORDER BY a.[DATE] DESC ";

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
        public ActionResult Payment()
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
                Session["PaybleAmount"] = "450";
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

        public ActionResult PaymentStatus()
        {
            if (Session["trackingno"] != null)
            {

                string SqlConn = ConfigurationManager.ConnectionStrings["RCLWEB"].ToString();
                SqlConnection oConnection = new SqlConnection(SqlConn);
                oConnection.Open();
                string qry = "SELECT * FROM [T_ONLINE_PAYMENT_RECEIVER] WHERE [tracking_no]='" + Session["trackingno"].ToString() + "'";
                SqlCommand oCommand = new SqlCommand(qry, oConnection);
                DataTable otable = new DataTable();
                otable.Load(oCommand.ExecuteReader());
                oConnection.Close();

                foreach (DataRow dr in otable.Rows)
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


                Session["username"] = "";
                Session["trackingno"] = tracking_no;


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
                    string qry1 = "UPDATE T_ONLINE_BO_REG SET IsAgree=1 WHERE [TRACKINGNO]='" + tracking_no + "'";
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

                    return RedirectToAction("index", "Home");

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
                { "total_amount", Session["PaybleAmount"].ToString()}, //
                { "currency", "BDT"},
                { "tran_id", GenerateUniqueId() },
                { "success_url", baseUrl + "Branch/PaymentStatus" },
                { "fail_url", baseUrl + "Branch/PaymentGatewayCallback" },
                { "cancel_url", baseUrl + "Branch/PaymentGatewayCallback" },
                { "cus_name", "Royal Capital" },
                { "cus_email", "bo@royalcapitalbd.com" },
                { "cus_add1", "Address Line On" },
                { "cus_city", "Dhaka" },
                { "cus_postcode", "1219" },
                { "cus_country", "Bangladesh" },
                { "cus_phone", "01977605080" },
                { "shipping_method", "NO" },
                { "product_name", "ONLINE BO" },
                { "product_category", "Service" },
                { "product_profile", "general" },
                { "value_a", Session["trackingno"].ToString() },
                { "value_b", "BO" },
                { "value_c", get_ip()+ ", Webpage" },
                { "value_d", "01977605080" }
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

    }
}