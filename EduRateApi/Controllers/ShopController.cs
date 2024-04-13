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
                            itemId = newShopItem,
                            price = item.price,
                            description = item.description,
                            title = item.title,
                            itemCount = item.itemCount,
                            disabled = false
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

        [HttpGet("GetActiveShopItems")]
        public async Task<List<ShopItem>> GetActiveShopItems()
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
                        var response = await client.GetAsync("Shop");
                        if (response.Body != "null") // якщо не null, то є записи у базі даних
                        {
                            // Отримуємо дані у вигляді словника
                            var data = response.ResultAs<Dictionary<string, ShopItem>>();
                            var activeShopItems = new List<ShopItem>();

                            // Перебираємо усі елементи словника
                            foreach (var pair in data)
                            {
                                // Якщо елемент не відключений (disabled = false), додаємо його до списку
                                if (!pair.Value.disabled)
                                {
                                    activeShopItems.Add(pair.Value);
                                }
                            }

                            return activeShopItems;
                        }
                        else
                        {
                            // Якщо записів немає, повертаємо порожній список
                            return new List<ShopItem>();
                        }
                    }
                    else
                    {
                        // Повертаємо помилку, якщо відсутнє з'єднання з Firebase
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
            try
            {
                // Отримати поточного користувача за його ідентифікатором
                Models.User currentUser = await GetUserById(itemDto.userId);

                // Отримати товар магазину за його ідентифікатором
                ShopItem shopItem = await GetShopItemById(itemDto.itemId);

                // Перевірити, чи достатньо монет користувача для покупки товару
                if (currentUser.numberOfDonatsCoins >= shopItem.price)
                {
                    // Зменшити кількість монет користувача та додати товар до його придбаних товарів
                    currentUser.numberOfDonatsCoins -= shopItem.price;
                    if (currentUser.purchasedItems == null)
                    {
                        currentUser.purchasedItems = new List<ShopItem>();
                    }
                    currentUser.purchasedItems.Add(shopItem);

                    // Зменшити кількість товару та відключити його, якщо кількість стає рівною нулю
                    shopItem.itemCount--;
                    if (shopItem.itemCount == 0)
                    {
                        shopItem.disabled = true;
                    }

                    // Оновити дані користувача в базі даних
                    await UpdateUser(currentUser);

                    // Оновити дані товару магазину в базі даних
                    await UpdateShopItem(shopItem);

                    return StatusCode((int)HttpStatusCode.OK, new LoginResponse(message: "Successfully bought", statusCode: 200));
                }
                else
                {
                    // Повернути повідомлення про недостатність монет для покупки товару
                    return StatusCode((int)HttpStatusCode.OK, new LoginResponse(message: $"Insufficient coins to buy {shopItem.price} ", statusCode: 200));
                }
            }
            catch (Exception ex)
            {
                // Повернути повідомлення про помилку у разі виникнення виключення
                return StatusCode((int)HttpStatusCode.InternalServerError, new ServerResponse(message: $"An error occurred while buying item: {ex.Message}", statusCode: 500));
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

        // Метод для оновлення користувача в базі даних
        private async Task UpdateUser(Models.User user)
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
                        // Виконати оновлення об'єкта користувача в базі даних
                        var response = await client.UpdateAsync($"Users/{user.userId}", user);
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            // Оновлення виконано успішно
                            Console.WriteLine($"User {user.userId} updated successfully");
                        }
                        else
                        {
                            // Помилка під час оновлення
                            Console.WriteLine($"Failed to update user {user.userId}");
                        }
                    }
                    else
                    {
                        // Помилка з'єднання з Firebase
                        Console.WriteLine("Firebase connection failed");
                    }
                }
            }
            catch (Exception ex)
            {
                // Обробка винятку
                Console.WriteLine("An error occurred while updating user in Firebase: " + ex.Message);
            }
        }

        // Метод для оновлення товару магазину в базі даних
        private async Task UpdateShopItem(ShopItem shopItem)
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
                        // Виконати оновлення об'єкта товару магазину в базі даних
                        var response = await client.UpdateAsync($"Shop/{shopItem.itemId}", shopItem);
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            // Оновлення виконано успішно
                            Console.WriteLine($"Shop item {shopItem.itemId} updated successfully");
                        }
                        else
                        {
                            // Помилка під час оновлення
                            Console.WriteLine($"Failed to update shop item {shopItem.itemId}");
                        }
                    }
                    else
                    {
                        // Помилка з'єднання з Firebase
                        Console.WriteLine("Firebase connection failed");
                    }
                }
            }
            catch (Exception ex)
            {
                // Обробка винятку
                Console.WriteLine("An error occurred while updating shop item in Firebase: " + ex.Message);
            }
        }

    }
}
