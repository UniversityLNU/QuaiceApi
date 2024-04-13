using EduRateApi.Dtos.ShopDTO;
using EduRateApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace EduRateApi.Interfaces
{
    public interface IShopService
    {
        public Task<ServerResponse> UploadItemInShop(ShopItemDto item);
        public Task<ShopResponse> GetShopItemById(string itemId);
        public Task<ServerResponse> BuyItemInShop(BuyShopItemDto itemDto);
        public Task<AllShopItemResponse> GetActiveShopItems();
        public Task<ServerResponse> UpdateShopItem(ShopItem shopItem);
    }
}
