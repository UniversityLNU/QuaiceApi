using EduRateApi.Models;

namespace EduRateApi.Dtos.FundraisingDTO
{
    public class ChangeStatusResponse
    {
        public string fundraisingId { get; set; }
        public FundraisingStatus newsStatus {  get; set; }
    }
}
