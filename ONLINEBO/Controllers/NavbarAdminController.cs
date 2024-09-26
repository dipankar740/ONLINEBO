using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ONLINEBO.Domain;

namespace ONLINEBO.Controllers
{
    public class NavbarAdminController : Controller
    {
        //
        // GET: /NavbarAdmin/
        public ActionResult Index()
        {
            var data = new DataAdmin();
            return PartialView("_NavbarAdmin", data.navbarItems().ToList());
        }
	}
}