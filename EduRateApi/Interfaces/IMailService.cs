using EduRateApi.Dtos;

namespace EduRateApi.Interfaces
{
    public interface IMailService
    {
        public void SendMail(SendMailDTO sendMail);
    }
}
