using EduRateApi.Dtos.Posts;
using EduRateApi.Dtos.Shop;
using EduRateApi.Models;
using FireSharp.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EduRateApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {

        //[HttpPost]
        //public async Task UploadItemInShop([FromBody]ShopItemDto item)
        //{
        //    try
        //    {
        //        var firebaseConfigPath = "Config/firebaseConfig.json";
        //        var configJson = System.IO.File.ReadAllText(firebaseConfigPath);
        //        var config = JsonConvert.DeserializeObject<FirebaseConfig>(configJson);

        //        using (var client = new FireSharp.FirebaseClient(config))
        //        {
        //            if (client != null)
        //            {

        //                var newPostId = Guid.NewGuid().ToString();
        //                var setUserPost = await client.SetAsync($"Posts/{newPostId}/", new ShopItem
        //                {
        //                    price = item.price,
        //                    creatorFullName = item.Description,
        //                    description = post.description,
        //                    dateOfCreation = post.dateOfCreation,
        //                    fundraisingId = post.fundraisingId,
        //                    attachedPhotos = post.attachedPhotos,
        //                });
        //                return new CreatePostResponseDto(newPostId, post.attachedPhotos);

        //            }
        //            else
        //            {
        //                Console.WriteLine("Firebase connection failed");
        //                return new CreatePostResponseDto();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("An error occurred while uploading teachers to Firebase: " + ex.Message);
        //        return new CreatePostResponseDto();
        //    }
        //}
    }
}
