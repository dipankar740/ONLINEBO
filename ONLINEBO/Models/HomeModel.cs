using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ONLINEBO.Models
{
    public class HomeModel
    {
        public string TrackingNo { get; set; }

        //[Required]
        [Display(Name="First Name *")]
        public string FirstName { get; set; }

        //[Required]
        [Display(Name = "Last Name *")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name="Email-ID *")]
        public string Email { get; set; }

        [Display(Name = "Mobile Number:")]
        [Required(ErrorMessage = "Mobile Number is required.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Invalid Mobile Number.")]
        [StringLength(11, ErrorMessage = "Please enter a 11-digit Mobile Number.", MinimumLength = 11)]
        public string Mobile { get; set; }

        public string Password { get; set; }

        public string PromoCode { get; set; }
    }
}