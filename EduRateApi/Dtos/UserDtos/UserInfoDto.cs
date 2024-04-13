namespace EduRateApi.Dtos.UserDtos
{
    public class UserInfoDto
    {
        public string userId { get; set; }
        public string fullName { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
        public string numberOfDonatsCoins { get; set; }
        public long lastDonateTime { get; set; }
        public double dailyMultiplier { get; set; }
        public double overallMultiplier { get; set; }
    }
}
