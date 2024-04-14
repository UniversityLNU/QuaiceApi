using System.Globalization;

namespace EduRateApi.Models
{
    public class ShopItem
    {
        public string itemId { get; set; }  
        public string title { get; set;}
        public double price { get; set;}
        public string description { get; set;}

        public string itemImage {  get; set;}

        public int itemCount { get; set; }

        public bool disabled { get; set;}

    }
}
