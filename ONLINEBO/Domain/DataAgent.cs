using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ONLINEBO.Models;

namespace ONLINEBO.Domain
{
    public class DataAgent
    {
        public IEnumerable<NavbarAgent> navbarItems()
        {
            var menu = new List<NavbarAgent>();

            menu.Add(new NavbarAgent { Id = 1, nameOption = "Dashboard", controller = "Agent", action = "Index", imageClass = "fa fa-money", status = true, isParent = false, parentId = 0 });
            menu.Add(new NavbarAgent { Id = 2, nameOption = "Online Bo", imageClass = "glyphicon glyphicon-play", status = true, isParent = true, parentId = 0 });
            menu.Add(new NavbarAgent { Id = 3, nameOption = "Create New BO A/C", controller = "Agent", action = "CreateNewBO", imageClass = "glyphicon glyphicon-plus", status = true, isParent = false, parentId = 2 });
            menu.Add(new NavbarAgent { Id = 5, nameOption = "Add Account Holder(s)", controller = "Agent", action = "AccountHolder", imageClass = "fa fa-male", status = true, isParent = false, parentId = 2 });
            menu.Add(new NavbarAgent { Id = 6, nameOption = "Add Bank A/C Info", controller = "Agent", action = "BankInfo/0", imageClass = "fa fa-bank", status = true, isParent = false, parentId = 2 });
            //menu.Add(new NavbarClient { Id = 7, nameOption = "Add Authorize Info", controller = "Agent", action = "AuthorizeInfo", imageClass = "fa fa-user", status = true, isParent = false, parentId = 2 });
            //menu.Add(new NavbarClient { Id = 8, nameOption = "Add Nominee Info", controller = "Agent", action = "NomineeInfo", imageClass = "fa fa-group", status = true, isParent = false, parentId = 2 });
            menu.Add(new NavbarAgent { Id = 9, nameOption = "Upload Document(s)", controller = "Agent", action = "UploadImages", imageClass = "fa fa-film", status = true, isParent = false, parentId = 2 });
            menu.Add(new NavbarAgent { Id = 10, nameOption = "Payment Option", controller = "Agent", action = "Payment", imageClass = "fa fa-money", status = true, isParent = false, parentId = 2 });

            menu.Add(new NavbarAgent { Id = 11, nameOption = "Search", controller = "Agent", action = "Search", imageClass = "fa fa-money", status = true, isParent = false, parentId = 0 });
            menu.Add(new NavbarAgent { Id = 12, nameOption = "Incomplete BO A/C", controller = "Agent", action = "IncompleteBOAccount", imageClass = "fa fa-money", status = true, isParent = false, parentId = 0 });
            menu.Add(new NavbarAgent { Id = 13, nameOption = "Completed BO A/C", controller = "Agent", action = "completeBOAccount", imageClass = "fa fa-money", status = true, isParent = false, parentId = 0 });
            menu.Add(new NavbarAgent { Id = 14, nameOption = "BO Sale Report", controller = "Agent", action = "BOSaleReport", imageClass = "fa fa-money", status = true, isParent = false, parentId = 0 });
            menu.Add(new NavbarAgent { Id = 15, nameOption = "CDBL Charge Rcv Report", controller = "Agent", action = "CDBLReport", imageClass = "fa fa-money", status = true, isParent = false, parentId = 0 });
            
            ////ade
            menu.Add(new NavbarAgent { Id = 16, nameOption = "Agent Ledger", controller = "Agent", action = "agent_ledger", imageClass = "fa fa-money", status = true, isParent = false, parentId = 0 });
            menu.Add(new NavbarAgent { Id = 17, nameOption = "Fund Withdraw Request", controller = "Agent", action = "fund_withdraw_request", imageClass = "fa fa-money", status = true, isParent = false, parentId = 0 });
            menu.Add(new NavbarAgent { Id = 18, nameOption = "BO Revenue", controller = "Agent", action = "bo_revenue", imageClass = "fa fa-money", status = true, isParent = false, parentId = 0 });
            menu.Add(new NavbarAgent { Id = 19, nameOption = "Share Revenue", controller = "Agent", action = "share_revenue", imageClass = "fa fa-money", status = true, isParent = false, parentId = 0 });



            return menu.ToList();
        }

    }
}