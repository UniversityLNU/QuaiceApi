namespace EduRateApi.Models
{
    public class User
    {
        public string userId { get; set; }
        public string email { get; set; }

        public string fullName { get; set; }

        public string phoneNumber { get; set; }

        public double NumberOfDonatsCoins { get; set; }

        public DateTime LastDonateTime {  get; set; }
        
        public double DailyMultiplier {  get; set; }

        public double OverallMultiplier { get; set; }
    }
}
