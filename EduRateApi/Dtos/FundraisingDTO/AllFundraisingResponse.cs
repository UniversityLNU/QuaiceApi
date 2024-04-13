using EduRateApi.Models;

namespace EduRateApi.Dtos.FundraisingDTO
{
    public class AllFundraisingResponse : ServerResponse
    {
        public List<Fundraising> fundraisingList { get; set; }

        public AllFundraisingResponse(int statusCode, string message, List<Fundraising> fundraising) : base(message , statusCode)
        {
            this.fundraisingList = fundraising;
        }
    }
}
