using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ONLINEBO.Models
{
    public class UserModel
    {
      public string SL { get; set; }
      public string BRANCHCODE {get; set;}
      public string BRANCHNAME {get; set;}
      public string USERID {get; set;}
      public string USERTYPE {get; set;}
      public string UESRNAME {get; set;}
      public string BRANCH_PREFIX {get; set;}
      public string AGENT_PREFIX {get; set;}

      [DataType(DataType.Password)]
      public string password {get; set;}
      public string LOCK { get; set; }
    }
}