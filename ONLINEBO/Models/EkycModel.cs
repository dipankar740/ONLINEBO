using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ONLINEBO.Models
{
    public class EkycModel
    {
        public string national_id { get; set; }
        public string person_dob { get; set; }
        public string person_photo { get; set; }
        public string mobile_otp { get; set; }
        public HttpPostedFileBase File { set; get; }

    }
}