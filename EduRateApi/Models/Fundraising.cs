using EduRateApi.Dtos.Fundraising;

namespace EduRateApi.Models
{
    public class Fundraising
    {
        public string fundraisingId {  get; set; }

        public string title { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }

        public string fundraisingUrl { get; set; }
        public string description { get; set; }
        public string fundraisingCompany { get; set; }

        public double goal { get; set; }

        public string fundraisingType { get; set; }

        public bool іsApproved { get; set; }
    }
}
