using EduRateApi.Dtos.Posts;
using EduRateApi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
                var config = JsonConvert.DeserializeObject<FireSharp.Config.FirebaseConfig>(configJson);

                using (var client = new FireSharp.FirebaseClient(config))
                {
                    if (client != null)
                    {

                        var newPostId = Guid.NewGuid().ToString();
                            var setUserPost = await client.SetAsync($"Posts/{post.userId}/{newPostId}/", new Posts
                            {
                                userId = post.userId,
                                creatorFullName = post.creatorFullName,
                                description = post.description,
                                dateOfCreation = post.dateOfCreation,
                                fundraisingId = post.fundraisingId,
                                attachedPhotos = post.attachedPhotos,
                                
                            });
                        var user = GetUserById(post.userId);


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

        private async Task<Models.User> CalculateUserNewPoint(Models.User user)
        {
            const double dailyMult = 0.05;
            const double strickMult = 0.01;



            if (user.dailyCount == 2)
            {

            }
            else
            {
                user.dailyCount += 1;
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
