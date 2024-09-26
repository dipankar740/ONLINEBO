namespace ONLINEBO.Models
{
    public class DataFaceModel
    {
        public string passKyc { get; set; }
        public string errorCode { get; set; }
        public Voter voter { get; set; }
        public class Error
        {
            public string code { get; set; }
            public string message { get; set; }
        }

        public class FaceMatchResult
        {
            public bool matched { get; set; }
            public int percentage { get; set; }
            public Error error { get; set; }
        }

        public class Voter
        {
            public string fatherEn { get; set; }
            public string motherEn { get; set; }
            public string spouseEn { get; set; }
            public string permanentAddressEn { get; set; }
            public string presentAddressEn { get; set; }
            public string name { get; set; }
            public string nameEn { get; set; }
            public string father { get; set; }
            public string mother { get; set; }
            public string gender { get; set; }
            public string profession { get; set; }
            public string spouse { get; set; }
            public string dob { get; set; }
            public string permanentAddress { get; set; }
            public string presentAddress { get; set; }
            public string nationalId { get; set; }
            public string oldNationalId { get; set; }
            public string photo { get; set; }
            public FaceMatchResult faceMatchResult { get; set; }
        }

    }
}
