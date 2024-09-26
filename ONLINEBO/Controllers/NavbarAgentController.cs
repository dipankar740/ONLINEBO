using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ONLINEBO.Domain;

namespace ONLINEBO.Controllers
{
    public class NavbarAgentController : Controller
    {
        //
        // GET: /NavbarAgent/
        public ActionResult Index()
        {
            var data = new DataAgent();
            return PartialView("_NavbarAgent", data.navbarItems().ToList());
        }
	}
}