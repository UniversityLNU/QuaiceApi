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
        [HttpPost("UploadUserPost")]
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
                        var creationDay = new DateTime(post.dateOfCreation);

                        var response = await client.GetAsync($"Users/{post.userId}");
                        var user = response.ResultAs<User>();

                        if (user.LastDonateTime != creationDay)
                        {
                            user.dailyCount = 0;
                        }

                        if (user.dailyCount != 2)
                        {
                            user.dailyCount += 1;
                        }

                        var strickGap = new DateTime(post.dateOfCreation) - creationDay;

                        if (strickGap.Days > 1)
                        {
                            user.strickCount = 0;
                        }
                        else if (user.LastDonateTime.Day != creationDay.Day)
                        {
                            user.strickCount += 1;
                        }
                        user.LastDonateTime = new DateTime(post.dateOfCreation);
                        var updateUser = await client.UpdateAsync($"Users/{user.userId}", user);
                        var newPostId = Guid.NewGuid().ToString();
                            var setUserPost = await client.SetAsync($"Posts/{newPostId}/", new Posts
                            {
                                userId = post.userId,
                                creatorFullName = post.creatorFullName,
                                description = post.description,
                                dateOfCreation = post.dateOfCreation,
                                fundraisingId = post.fundraisingId,
                                attachedPhotos = post.attachedPhotos,
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


        public User Calculate(User  user)
        {
            const double dailyMult = 0.05;
            const double strickMult = 0.01;

            user.NumberOfDonatsCoins += (user.dailyCount * dailyMult + 1) * (user.strickCount * strickMult + 1) * 2;

            return user;
        }

        [HttpGet("GetPostById/{postId}")]
        public async Task<ActionResult<PostDto>> GetPostById(string postId)
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
                        var response = await client.GetAsync($"Posts/{postId}");
                        if (response.Body != "null") // якщо не null, то запис існує у базі даних
                        {
                            // Якщо запис існує, повертаємо об'єкт PostDto
                            var post = response.ResultAs<Posts>();
                            var postDto = new Posts
                            {
                                userId = post.userId,
                                creatorFullName = post.creatorFullName,
                                description = post.description,
                                dateOfCreation = post.dateOfCreation,
                                fundraisingId = post.fundraisingId,
                                attachedPhotos = post.attachedPhotos
                            };
                            return Ok(postDto);
                        }
                        else
                        {
                            // Якщо запис не знайдено, повертаємо повідомлення про помилку
                            return NotFound("Post not found");
                        }
                    }
                    else
                    {
                        return BadRequest("Firebase connection failed");
                    }
                }
            }
            catch (Exception ex)
            {
                // Повертаємо повідомлення про помилку у разі виникнення виключення
                return StatusCode(500, $"An error occurred while retrieving post: {ex.Message}");
            }
        }


        [HttpGet("GetAllPosts")]
        public async Task<ActionResult<List<Posts>>> GetAllPosts()
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
                        var response = await client.GetAsync("Posts");
                        if (response.Body != "null") // якщо не null, то записи існують у базі даних
                        {
                            // Якщо записи існують, повертаємо список об'єктів PostDto
                            var posts = response.ResultAs<Dictionary<string, Posts>>();
                            var postList = posts.Values.ToList();
                            return Ok(postList);
                        }
                        else
                        {
                            // Якщо записів не знайдено, повертаємо порожній список
                            return Ok(new List<Posts>());
                        }
                    }
                    else
                    {
                        return BadRequest("Firebase connection failed");
                    }
                }
            }
            catch (Exception ex)
            {
                // Повертаємо повідомлення про помилку у разі виникнення виключення
                return StatusCode(500, $"An error occurred while retrieving posts: {ex.Message}");
            }
        }

    }


}
