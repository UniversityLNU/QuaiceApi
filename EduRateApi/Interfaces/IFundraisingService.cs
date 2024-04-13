using EduRateApi.Dtos.FundraisingDTO;
using EduRateApi.Models;

namespace EduRateApi.Interfaces
{
    public interface IFundraisingService
    {
        Task<ServerResponse> UploadFundrasing(FundraisingDto fundraisingDto);
        Task<FundraisingResponse> GetFundraisingById(string fundraisingId);
        Task<AllFundraisingResponse> GetAllApprovedFundraisings();
        Task<AllFundraisingResponse> GetUnapprovedFundraisings();
        Task<ServerResponse> ApproveFundraising(string fundraisingId);
    }
}
