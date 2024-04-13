using EduRateApi.Models;

namespace EduRateApi.Dtos.FundraisingDTO
{
    public class FundraisingResponse : ServerResponse
    {
        public Fundraising fundraising { get; set; }
        public FundraisingResponse(Fundraising fundraising, string message, int statusCode) : base(message, statusCode)
        {
            this.fundraising = fundraising;
        }
    }
}
