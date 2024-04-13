using EduRateApi.Models;

namespace EduRateApi.Dtos
{
    public class LoginResponse : ServerResponse
    {
        public string jwt { get; set; }

        public string userId { get; set; }

        public LoginResponse(int statusCode, string message, string jwt ,string userId) : base(statusCode, message)
        {
           this.jwt = jwt;
            this.userId = userId;
        }

        public LoginResponse(int statusCode, string message, string jwt) : base(statusCode, message)
        {
            this.jwt = jwt;
        }

        public LoginResponse(int statusCode, string message) : base(statusCode, message)
        {
        }
    }
}
