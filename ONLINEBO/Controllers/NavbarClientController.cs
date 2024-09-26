using ONLINEBO.Domain;
using ONLINEBO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ONLINEBO.Controllers
{
    public class NavbarClientController : Controller
    {
        // GET: Navbar
        public ActionResult Index()
        {
                //@Session["username"].ToString();
                //@Session["trackingno"].ToString();

                OnlineBODetailModel DBMODEL = new OnlineBODetailModel();
                DBMODEL.TRACKINGNO = @Session["trackingno"].ToString();

                BankInfoModel bankBOmodel = new BankInfoModel();
                bankBOmodel.TrackingNo = @Session["trackingno"].ToString();
                var userName = @Session["username"].ToString();

            Session["trackingno"] = bankBOmodel.TrackingNo;
            Session["username"] = userName;

            var data = new DataClient();
            return PartialView("_NavbarClient", data.navbarItems().ToList());
        }
    }
}