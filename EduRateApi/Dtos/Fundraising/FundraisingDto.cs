using Microsoft.AspNetCore.Identity;

namespace EduRateApi.Dtos.Fundraising
{
    public class FundraisingDto
    {
        public string title { get; set; }
        public string fundraisingUrl { get; set; }
        public string description { get; set; }
        public string fundraisingCompany { get; set; }
        public double goal { get; set; }
        public FundraisingType fundraisingType { get; set; }
        public string phoneNumber {  get; set; }
        public string email {  get; set; }


    }

    public enum FundraisingType
    {
        Military , Humanitarian
    }
}
