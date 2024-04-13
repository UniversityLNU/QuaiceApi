using EduRateApi.Models;
using Microsoft.AspNetCore.Identity;

namespace EduRateApi.Dtos.FundraisingDTO
{
    public class AllFundraisingResponse
    {
       
        public List<Fundraising> fundraisingList { get; set; }

        public AllFundraisingResponse(List<Fundraising> fundraising)
        {
            this.fundraisingList = fundraising;
        }
    }


}
