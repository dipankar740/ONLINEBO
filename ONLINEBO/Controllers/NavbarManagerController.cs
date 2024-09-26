using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ONLINEBO.Domain;

namespace ONLINEBO.Controllers
{
    public class NavbarManagerController : Controller
    {
        //
        // GET: /NavbarManager/
        public ActionResult Index()
        {
            var data = new DataManager();
            return PartialView("_NavbarManager", data.navbarItems().ToList());
        }


	}
}