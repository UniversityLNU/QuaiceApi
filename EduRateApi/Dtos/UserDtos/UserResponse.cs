using EduRateApi.Models;
using Microsoft.Identity.Client;

namespace EduRateApi.Dtos.UserDtos
{
    public class UserResponse : ServerResponse
    {
        public User user {  get; set; }
        public UserResponse( User user , string message, int statusCode) : base(message, statusCode)
        {
            this.user = user;
        }
    }
}
