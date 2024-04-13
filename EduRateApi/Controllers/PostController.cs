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
                        
                        var getUserPosts = await client.GetAsync($"Posts/{post.userId}");
                        if (getUserPosts == null)
                        {
                            var setUserPost = await client.SetAsync($"Posts/{post.userId}/1/", new Posts { 
                                CreatorFullName = post.creatorFullName,
                                Description = post.description,
                                DateOfCreation = post.dateOfCreation,
                                FundraisingId = post.fundraisingId,
                                AttachedPhotos = post.attachedPhotos,
                            });
                            return new CreatePostResponseDto("1", post.attachedPhotos);
                        }
                        else
                        {
                            var responseContent = getUserPosts.Body.Count();
                            var setUserPost = await client.SetAsync($"Posts/{post.userId}/{responseContent + 1}/", new Posts
                            {
                                CreatorFullName = post.creatorFullName,
                                Description = post.description,
                                DateOfCreation = post.dateOfCreation,
                                FundraisingId = post.fundraisingId,
                                AttachedPhotos = post.attachedPhotos,
                            });
                            return new CreatePostResponseDto((responseContent + 1).ToString(), post.attachedPhotos);
                        }
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
