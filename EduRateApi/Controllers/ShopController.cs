using EduRateApi.Dtos;
using EduRateApi.Dtos.Posts;
using EduRateApi.Dtos.Shop;
using EduRateApi.Models;
using Firebase.Auth;
using FirebaseAdmin.Auth;
using FireSharp.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace EduRateApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {

        [HttpPost("UploadItemInShop")]
        public async Task<ActionResult<ServerResponse>> UploadItemInShop([FromBody] ShopItemDto item)
        {
            try
            {
                var firebaseConfigPath = "Config/firebaseConfig.json";
                var configJson = System.IO.File.ReadAllText(firebaseConfigPath);
                var config = JsonConvert.DeserializeObject<FireSharp.Config.FirebaseConfig>(configJson);

                using (var client = new FireSharp.FirebaseClient(config))
                {
                    if (client != null)
                    {

                        var newShopItem = Guid.NewGuid().ToString();
                        var setUserPost = await client.SetAsync($"Shop/{newShopItem}/", new ShopItem
                        {
                            itemId = Guid.NewGuid().ToString(),
                            price = item.price,
                            description = item.description,
                            title = item.title,
                        });
                        return StatusCode((int)HttpStatusCode.OK, new ServerResponse(message: "Item Succesfully Uploaded", statusCode: 200));

                    }
                    else
                    {
                        Console.WriteLine("Firebase connection failed");
                        return StatusCode((int)HttpStatusCode.InternalServerError, new ServerResponse(message: "Item Succesfully Uploaded", statusCode: 500));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while uploading teachers to Firebase: " + ex.Message);
                return StatusCode((int)HttpStatusCode.BadRequest, new ServerResponse(message: "Item Succesfully Uploaded", statusCode: 400));
            }
        }

        [HttpGet("GetShopItemById/{itemId}")]
        public async Task<ShopItem> GetShopItemById(string itemId)
        {
            try
            {
                var firebaseConfigPath = "Config/firebaseConfig.json";
                var configJson = System.IO.File.ReadAllText(firebaseConfigPath);
                var config = JsonConvert.DeserializeObject<FireSharp.Config.FirebaseConfig>(configJson);

                using (var client = new FireSharp.FirebaseClient(config))
                {
                    if (client != null)
                    {
                        var response = await client.GetAsync($"Shop/{itemId}");
                        if (response.Body != "null") // якщо не null, то запис існує у базі даних
                        {
                            // Якщо запис існує, повертаємо об'єкт ShopItem
                            var shopItem = response.ResultAs<ShopItem>();
                            return shopItem;
                        }
                        else
                        {
                            // Якщо запис не знайдено, повертаємо повідомлення про помилку
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                // Повертаємо повідомлення про помилку у разі виникнення виключення
                return null;
            }
        }


        [HttpPost("BuyItemInShop")]
        public async Task<ActionResult<ServerResponse>> BuyItemInShop([FromBody] BuyShopItemDto itemDto)
        {
            Models.User currentUser = await GetUserById(itemDto.userId);

            ShopItem shopItem = await GetShopItemById(itemDto.itemId);

            if (currentUser.NumberOfDonatsCoins >= shopItem.price) 
            {
                currentUser.NumberOfDonatsCoins -= shopItem.price;
                currentUser.purchasedItems.Add(shopItem);

                shopItem.itemCount--;
                if (shopItem.itemCount == 0)
                {
                    shopItem.disabled = true;
                }
                return StatusCode((int)HttpStatusCode.OK, new LoginResponse(message: "Succesfully buyed", statusCode: 200));
            }
            else
            {
                return StatusCode((int)HttpStatusCode.OK, new LoginResponse(message: $"I dont have {shopItem.price} ", statusCode: 200));
            }
        }



        private async Task<Models.User> GetUserById(string userId)
        {
            try
            {
                var firebaseConfigPath = "Config/firebaseConfig.json";
                var configJson = System.IO.File.ReadAllText(firebaseConfigPath);
                var config = JsonConvert.DeserializeObject<FireSharp.Config.FirebaseConfig>(configJson);

                using (var client = new FireSharp.FirebaseClient(config))
                {
                    if (client != null)
                    {
                        // Отримуємо дані користувача за його ідентифікатором
                        var response = await client.GetAsync($"Users/{userId}");
                        if (response.Body != "null")
                        {
                            // Якщо знайдено користувача з відповідним ідентифікатором, повертаємо його
                            var user = response.ResultAs<Models.User>();
                            return user;
                        }
                        else
                        {
                            // Якщо користувача з відповідним ідентифікатором не знайдено, повертаємо null
                            return null;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Firebase connection failed");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving user: " + ex.Message);
                return null;
            }
        }
    }
}
