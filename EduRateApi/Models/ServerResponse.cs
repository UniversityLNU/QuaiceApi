namespace EduRateApi.Models
{
    public abstract class ServerResponse
    {
        public int statusCode { get; set; }
        public string message { get; set; }

        protected ServerResponse(int statusCode, string message)
        {
            this.statusCode = statusCode;
            this.message = message;
        }
    }
}
