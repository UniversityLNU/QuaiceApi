using EduRateApi.Models;

namespace EduRateApi.Dtos
{
    public class LoginResponse : ServerResponse
    {
        public string JWTToken { get; set; }

        public LoginResponse(int statusCode, string message, string Jwt) : base(statusCode, message)
        {
            JWTToken = Jwt;
        }

        public LoginResponse(int statusCode, string message) : base(statusCode, message)
        {
        }
    }
}
