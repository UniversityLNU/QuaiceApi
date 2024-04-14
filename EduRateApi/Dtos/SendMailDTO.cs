namespace EduRateApi.Dtos
{
    public class SendMailDTO
    {
        public string userName { get; set; }

        public string email { get; set; }

        public string fundraisingTitle {  get; set; }

        public string htmlTemplate { get; set; }


        public SendMailDTO(string userName, string email, string fundraisingTitle, string htmlTemplate)
        {
            this.userName = userName;
            this.email = email;
            this.fundraisingTitle = fundraisingTitle;
            this.htmlTemplate = htmlTemplate;
        }
    }
}
