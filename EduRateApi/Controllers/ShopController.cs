using EduRateApi.Dtos;
using EduRateApi.Dtos.ShopDTO;
using EduRateApi.Interfaces;
using EduRateApi.Models;
using Firebase.Auth;
using FirebaseAdmin.Auth;
using FireSharp.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Reflection;

namespace EduRateApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {

        [HttpPost("UploadItemInShop")]
        public async Task<ActionResult<ServerResponse>> UploadItemInShop([FromBody] ShopItemDto item)
        {
            var response = await _shopService.UploadItemInShop(item);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpGet("GetShopItemById/{itemId}")]
        public async Task<ShopItem> GetShopItemById(string itemId)
        {
            var response = await _shopService.GetShopItemById(itemId);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpGet("GetActiveShopItems")]
        public async Task<ActionResult<AllShopItemResponse>> GetActiveShopItems()
        {
            var response = await _shopService.GetActiveShopItems();
            return StatusCode((int)response.statusCode, response);
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
            var response = await _shopService.BuyItemInShop(itemDto);
            return StatusCode((int)response.statusCode, response);
        }
    }
}
