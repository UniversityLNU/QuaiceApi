using EduRateApi.Dtos.Fundraising;
using EduRateApi.Models;

namespace EduRateApi.Interfaces
{
    public interface IFundraisingService
    {
        Task<ServerResponse> UploadFundrasing(FundraisingDto fundraisingDto);
        Task<Fundraising> GetFundraisingById(string fundraisingId);
        Task<List<Fundraising>> GetAllApprovedFundraisings();
        Task<List<Fundraising>> GetUnapprovedFundraisings();
        Task<bool> ApproveFundraising(string fundraisingId);
    }
}
