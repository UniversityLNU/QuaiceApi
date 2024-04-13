using EduRateApi.Models;

namespace EduRateApi.Dtos.ShopDTO
{
    public class AllShopItemResponse : ServerResponse
    {
        public List<ShopItem> shopItem { get; set; }

        public AllShopItemResponse(int statusCode, string message, List<ShopItem> shopItem) : base(message, statusCode)
        {
            this.shopItem = shopItem;
        }
    }
}
