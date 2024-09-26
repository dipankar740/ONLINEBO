using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ONLINEBO.Models;

namespace ONLINEBO.Domain
{
    public class DataAdmin
    {

        public IEnumerable<NavbarClient> navbarItems()
        {
            var menu = new List<NavbarClient>();

            menu.Add(new NavbarClient { Id = 1, nameOption = "Dashboard", controller = "Admin", action = "Index", imageClass = "fa fa-money", status = true, isParent = false, parentId = 0 });
            menu.Add(new NavbarClient { Id = 2, nameOption = "User List", controller = "Admin", action = "UserList", imageClass = "fa fa-male", status = true, isParent = false, parentId = 0 });
            //menu.Add(new NavbarClient { Id = 3, nameOption = "Create New BO A/C", controller = "Branch", action = "CreateNewBO", imageClass = "glyphicon glyphicon-plus", status = true, isParent = false, parentId = 2 });
            //menu.Add(new NavbarClient { Id = 5, nameOption = "Add Account Holder(s)", controller = "Branch", action = "AccountHolder", imageClass = "fa fa-male", status = true, isParent = false, parentId = 2 });
            //menu.Add(new NavbarClient { Id = 6, nameOption = "Add Bank A/C Info", controller = "Branch", action = "BankInfo", imageClass = "fa fa-bank", status = true, isParent = false, parentId = 2 });
            //menu.Add(new NavbarClient { Id = 7, nameOption = "Add Authorize Info", controller = "Branch", action = "AuthorizeInfo", imageClass = "fa fa-user", status = true, isParent = false, parentId = 2 });
            //menu.Add(new NavbarClient { Id = 8, nameOption = "Add Nominee Info", controller = "Branch", action = "NomineeInfo", imageClass = "fa fa-group", status = true, isParent = false, parentId = 2 });
            //menu.Add(new NavbarClient { Id = 9, nameOption = "Upload Document(s)", controller = "Branch", action = "UploadImages", imageClass = "fa fa-film", status = true, isParent = false, parentId = 2 });
            //menu.Add(new NavbarClient { Id = 10, nameOption = "Payment Option", controller = "Branch", action = "PaymentOption", imageClass = "fa fa-money", status = true, isParent = false, parentId = 2 });

            //menu.Add(new NavbarClient { Id = 11, nameOption = "Incomplete BO A/C", controller = "Branch", action = "PaymentOption", imageClass = "fa fa-money", status = true, isParent = false, parentId = 0 });
            //menu.Add(new NavbarClient { Id = 12, nameOption = "Completed BO A/C", controller = "Branch", action = "PaymentOption", imageClass = "fa fa-money", status = true, isParent = false, parentId = 0 });
            //menu.Add(new NavbarClient { Id = 13, nameOption = "Payment Pending BO A/C", controller = "Branch", action = "PaymentOption", imageClass = "fa fa-money", status = true, isParent = false, parentId = 0 });


            return menu.ToList();
        }

    }
}