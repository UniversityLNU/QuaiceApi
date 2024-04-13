using EduRateApi.Models;

namespace EduRateApi.Dtos
{
    public class LoginResponse : ServerResponse
    {
        public string jwtToken { get; set; }

        public string userId { get; set; }

        public LoginResponse(int statusCode, string message, string jwtToken, string userId) : base(message, statusCode)
        {
           this.jwtToken = jwtToken;
            this.userId = userId;
        }

        public LoginResponse(int statusCode, string message, string jwtToken) : base(message, statusCode)
        {
            this.jwtToken = jwtToken;
        }

        public LoginResponse(int statusCode, string message) : base(message, statusCode)
        {
        }
    }
}
