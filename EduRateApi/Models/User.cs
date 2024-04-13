namespace EduRateApi.Models
{
    public class User
    {
        public string userId { get; set; }
        public string email { get; set; }

        public string fullName { get; set; }

        public string phoneNumber { get; set; }

        public double numberOfDonatsCoins { get; set; }

        public DateTime lastDonateTime {  get; set; }
        
        public double dailyMultiplier {  get; set; }

        public double overallMultiplier { get; set; }

        public List<ShopItem> purchasedItems {  get; set; }
    }
}
