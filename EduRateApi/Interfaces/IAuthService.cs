using EduRateApi.Dtos;
using EduRateApi.Models;

namespace EduRateApi.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> RegisterUserAsync(UserRegisterDTO model);

        Task<LoginResponse> LoginUserAsync(UserLoginDTO model);

        Task<ServerResponse> GetUserInfoAsync(string firebaseToken);
    }
}
