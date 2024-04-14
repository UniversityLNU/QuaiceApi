using EduRateApi.Dtos.FundraisingDTO;
using EduRateApi.Models;

namespace EduRateApi.Interfaces
{
    public interface IFundraisingService
    {
        Task<ServerResponse> UploadFundrasing(FundraisingDto fundraisingDto);
        Task<FundraisingResponse> GetFundraisingById(string fundraisingId);
        Task<AllFundraisingResponse> GetAllApprovedFundraisings();
        Task<AllFundraisingResponse> GetAllPendingFundraising();
        Task<ServerResponse> ApproveDeclineFundraising(string fundraisingId, FundraisingStatus status);
    }
}
