using ONLINEBO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ONLINEBO.Domain
{
    public class DataClient
    {
        public IEnumerable<NavbarClient> navbarItems()
        {
            var menu = new List<NavbarClient>();

            menu.Add(new NavbarClient { Id = 1, nameOption = "Profile", controller = "Client", action = "Index", imageClass = "fa fa-dashboard", status = true, isParent = false, parentId = 0 });
            menu.Add(new NavbarClient { Id = 2, nameOption = "Add Account Holder(s)", controller = "Client", action = "AccountHolder", imageClass = "fa fa-male", status = true, isParent = false, parentId = 0 });
            menu.Add(new NavbarClient { Id = 3, nameOption = "Add Bank A/C Info", controller = "Client", action = "BankInfo/0", imageClass = "fa fa-bank", status = true, isParent = false, parentId = 0 });
            menu.Add(new NavbarClient { Id = 4, nameOption = "Add Authorize Info", controller = "Client", action = "AuthorizeInfo", imageClass = "fa fa-user", status = true, isParent = false, parentId = 0 });
            menu.Add(new NavbarClient { Id = 5, nameOption = "Add Nominee Info", controller = "Client", action = "NomineeInfo", imageClass = "fa fa-group", status = true, isParent = false, parentId = 0 });
            menu.Add(new NavbarClient { Id = 6, nameOption = "Upload Document(s)", controller = "Client", action = "UploadImages", imageClass = "fa fa-film", status = true, isParent = false, parentId = 0 });
            menu.Add(new NavbarClient { Id = 7, nameOption = "Payment Option", controller = "Client", action = "PaymentOption", imageClass = "fa fa-money", status = true, isParent = false, parentId = 0 });
            menu.Add(new NavbarClient { Id = 8, nameOption = "Payment Status", controller = "Client", action = "PaymentStatus", imageClass = "fa fa-money", status = true, isParent = false, parentId = 0 });

            return menu.ToList();
        }
    }
}