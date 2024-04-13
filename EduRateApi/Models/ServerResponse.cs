namespace EduRateApi.Models
{
    public class ServerResponse
    {
        public int statusCode { get; set; }
        public string message { get; set; }

        public ServerResponse(string message , int statusCode)
        {
            this.statusCode = statusCode;
            this.message = message;
        }
    }
}
