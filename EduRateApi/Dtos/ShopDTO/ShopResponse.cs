using EduRateApi.Models;

namespace EduRateApi.Dtos.ShopDTO
{
    public class ShopResponse : ServerResponse
    {
        public ShopItem shopItem { get; set; }
        public ShopResponse(ShopItem shopItem, string message, int statusCode) : base(message, statusCode)
        {
            this.shopItem = shopItem;
        }
    }
}
