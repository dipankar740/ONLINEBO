using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ONLINEBO.Domain;

namespace ONLINEBO.Controllers
{
    public class NavbarBranchController : Controller
    {
        //
        // GET: /NavbarBranch/
        public ActionResult Index()
        {
            var data = new DataBranch();
            return PartialView("_NavbarBranch", data.navbarItems().ToList());
        }
	}
}