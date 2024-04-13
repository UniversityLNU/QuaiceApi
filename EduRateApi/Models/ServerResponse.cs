namespace EduRateApi.Models
{
    public class ServerResponse
    {
        public int statusCode { get; set; }
        public string message { get; set; }
        public ServerResponse(int statusCode , string message)
        {
            this.statusCode = statusCode;
            this.message = message;
        }

    }
}
