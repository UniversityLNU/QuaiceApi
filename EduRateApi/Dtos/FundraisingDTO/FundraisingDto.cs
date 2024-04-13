using Microsoft.AspNetCore.Identity;
using System.Runtime.Serialization;

namespace EduRateApi.Dtos.FundraisingDTO
{
    public class FundraisingDto
    {
        public string title { get; set; }
        public string fundraisingUrl { get; set; }
        public string description { get; set; }
        public string fundraisingCompany { get; set; }
        public double goal { get; set; }
        public string fundraisingType { get; set; }
        public string phoneNumber {  get; set; }
        public string email {  get; set; }


    }


}
