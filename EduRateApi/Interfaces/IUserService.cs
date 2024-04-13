using EduRateApi.Dtos.UserDtos;
using EduRateApi.Models;

namespace EduRateApi.Interfaces
{
    public interface IUserService
    {
        public Task<ServerResponse> UpdateUser(Models.User user);
        public Task<UserResponse> GetUserById(string userId);
    }
}
