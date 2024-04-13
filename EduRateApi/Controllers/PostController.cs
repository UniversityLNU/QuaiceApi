using EduRateApi.Dtos.Posts;
using EduRateApi.Models;
using FireSharp.Config;
using FireSharp.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;

namespace EduRateApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        [HttpPost]
        public async Task<CreatePostResponseDto> UploadUserPost([FromBody] PostDto post)
        {
            try
            {
                var firebaseConfigPath = "Config/firebaseConfig.json";
                var configJson = System.IO.File.ReadAllText(firebaseConfigPath);
                var config = JsonConvert.DeserializeObject<FirebaseConfig>(configJson);

                using (var client = new FireSharp.FirebaseClient(config))
                {
                    if (client != null)
                    {

                        var newPostId = Guid.NewGuid().ToString();
                            var setUserPost = await client.SetAsync($"Posts/{post.userId}/{newPostId}/", new Posts
                            {
                                CreatorFullName = post.creatorFullName,
                                Description = post.description,
                                DateOfCreation = post.dateOfCreation,
                                FundraisingId = post.fundraisingId,
                                AttachedPhotos = post.attachedPhotos,
                            });
                           return new CreatePostResponseDto(newPostId, post.attachedPhotos);
                        
                    }
                    else
                    {
                        Console.WriteLine("Firebase connection failed");
                        return new CreatePostResponseDto();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while uploading teachers to Firebase: " + ex.Message);
                return new CreatePostResponseDto();
            }
        }
    }
}
