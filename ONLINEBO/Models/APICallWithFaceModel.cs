using System.ComponentModel.DataAnnotations;

namespace ONLINEBO.Models
{
    public class APICallWithFaceModel
    {
        public string national_id { get; set; }
        public string team_tx_id { get; set; }
        public bool english_output { get; set; } = true;
        public string person_dob { get; set; }
        public string person_photo { get; set; }
    }
}
