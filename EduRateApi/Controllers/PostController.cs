using EduRateApi.Dtos.Posts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace EduRateApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {

        public async Task UploadUserPost([FromBody] PostDto post)
        {
            //try
            //{
            //    var firebaseConfigPath = "Config/firebaseConfig.json";
            //    var configJson = System.IO.File.ReadAllText(firebaseConfigPath);
            //    var config = JsonConvert.DeserializeObject<FirebaseConfig>(configJson);

            //    using (var client = new FireSharp.FirebaseClient(config))
            //    {
            //        if (client != null)
            //        {
            //            var response = await client.GetAsync($"LNU/post/");
            //            if (response.Body != "null") // якщо не null, то запис вже існує у базі даних
            //            {
            //                Console.WriteLine($"Teacher {teacher.name} already exists in the database");
            //            }
            //            else
            //            {
            //                var setResponse = await client.SetAsync($"LNU/teachers/{teacher.faculty}/{teacher.id}", teacher); // Змінено шлях зберігання
            //                if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
            //                {
            //                    Console.WriteLine($"Teacher {teacher.name} uploaded successfully");
            //                }
            //                else
            //                {
            //                    Console.WriteLine($"Failed to upload teacher {teacher.name}");
            //                }
            //            }
            //        }
            //        else
            //        {
            //            Console.WriteLine("Firebase connection failed");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("An error occurred while uploading teachers to Firebase: " + ex.Message);
            //}
        }
    }
}
