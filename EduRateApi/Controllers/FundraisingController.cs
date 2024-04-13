using EduRateApi.Dtos;
using EduRateApi.Dtos.Fundraising;
using EduRateApi.Models;
using FireSharp.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace EduRateApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FundraisingController : ControllerBase
    {

        [HttpPost("UploadFundrasing")]
        public async Task<ActionResult<ServerResponse>> UploadFundrasing([FromBody] FundraisingDto fundraisingDto)
        {
            try
            {

                string fundraisingId = GenerateNewFundraisingId();
                
                var firebaseConfigPath = "Config/firebaseConfig.json";
                var configJson = System.IO.File.ReadAllText(firebaseConfigPath);
                var config = JsonConvert.DeserializeObject<FirebaseConfig>(configJson);

                using (var client = new FireSharp.FirebaseClient(config))
                {
                    if (client != null)
                    {
                        var response = await client.GetAsync($"Fundraising/{fundraisingId}");

                            Fundraising fundraising = new Fundraising
                            {
                                fundraisingId = fundraisingId,
                                phoneNumber = fundraisingDto.phoneNumber,
                                email = fundraisingDto.email,
                                title = fundraisingDto.title,
                                fundraisingUrl = fundraisingDto.fundraisingUrl,
                                description = fundraisingDto.description,
                                fundraisingCompany = fundraisingDto.fundraisingCompany,
                                goal = fundraisingDto.goal,
                                fundraisingType = fundraisingDto.fundraisingType,
                                іsApproved = false 
                            };

                            var setResponse = await client.SetAsync($"Fundraising/{fundraising.fundraisingId}", fundraising);
                            if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                Console.WriteLine($"Fundraising {fundraising.fundraisingId} uploaded successfully");
                                return StatusCode((int)HttpStatusCode.OK, new ServerResponse(message: "Fundraising uploaded successfully", statusCode: 200));
                            }
                            else
                            {
                                return StatusCode((int)HttpStatusCode.BadRequest, new ServerResponse(message: "Failed to upload fundraising", statusCode: 400));
                            }
                        
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.BadRequest, new ServerResponse(message: "Firebase connection failed", statusCode: 400));
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ServerResponse(message: "Error occurred while uploading fundraising to Firebase", statusCode: 500));
            }
        }


        [HttpGet("GetFundraisingById/{fundraisingId}")]
        public async Task<ActionResult<Fundraising>> GetFundraisingById(string fundraisingId)
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
                        var response = await client.GetAsync($"Fundraising/{fundraisingId}");
                        if (response.Body != "null") // якщо не null, то запис існує у базі даних
                        {
                            // Якщо запис існує, повертаємо об'єкт Fundraising
                            var fundraising = response.ResultAs<Fundraising>();
                            return Ok(fundraising);
                        }
                        else
                        {
                            // Якщо запис не знайдено, повертаємо повідомлення про помилку
                            return NotFound("Fundraising not found");
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
                return StatusCode(500, $"An error occurred while retrieving fundraising: {ex.Message}");
            }
        }

        [HttpGet("GetAllFundraisings")]
        public async Task<ActionResult<List<Fundraising>>> GetAllFundraisings()
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
                        var response = await client.GetAsync("Fundraising");
                        if (response.Body != "null") // якщо не null, то є записи у базі даних
                        {
                            // Якщо записи існують, отримуємо їх та повертаємо список об'єктів Fundraising
                            var data = response.ResultAs<Dictionary<string, Fundraising>>();
                            var fundraisings = data.Values.ToList();
                            return Ok(fundraisings);
                        }
                        else
                        {
                            // Якщо записів немає, повертаємо порожній список
                            return Ok(new List<Fundraising>());
                        }
                    }
                    else
                    {
                        // Повертаємо помилку, якщо відсутнє з'єднання з Firebase
                        return BadRequest("Firebase connection failed");
                    }
                }
            }
            catch (Exception ex)
            {
                // Повертаємо повідомлення про помилку у разі виникнення виключення
                return StatusCode(500, $"An error occurred while retrieving fundraisings: {ex.Message}");
            }
        }

        [HttpGet("GetUnapprovedFundraisings")]
        public async Task<ActionResult<List<Fundraising>>> GetUnapprovedFundraisings()
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
                        var response = await client.GetAsync("Fundraising");
                        if (response.Body != "null") // якщо не null, то є записи у базі даних
                        {
                            // Отримуємо всі Fundraising з бази даних
                            var data = response.ResultAs<Dictionary<string, Fundraising>>();

                            // Фільтруємо тільки ті Fundraising, де isApproved = false
                            var unapprovedFundraisings = data.Values.Where(f => !f.іsApproved).ToList();

                            // Повертаємо список знайдених незатверджених Fundraising
                            return Ok(unapprovedFundraisings);
                        }
                        else
                        {
                            // Якщо записів немає, повертаємо порожній список
                            return Ok(new List<Fundraising>());
                        }
                    }
                    else
                    {
                        // Повертаємо помилку, якщо відсутнє з'єднання з Firebase
                        return BadRequest("Firebase connection failed");
                    }
                }
            }
            catch (Exception ex)
            {
                // Повертаємо повідомлення про помилку у разі виникнення виключення
                return StatusCode(500, $"An error occurred while retrieving unapproved fundraisings: {ex.Message}");
            }
        }


        [HttpPut("ApproveFundraising/{fundraisingId}")]
        public async Task<ActionResult> ApproveFundraising(string fundraisingId)
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
                        var response = await client.GetAsync($"Fundraising/{fundraisingId}");
                        if (response.Body != "null") // якщо не null, то запис існує у базі даних
                        {
                            // Зчитуємо Fundraising з бази даних
                            var fundraising = response.ResultAs<Fundraising>();

                            // Змінюємо isApproved на true
                            fundraising.іsApproved = true;

                            // Оновлюємо Fundraising в базі даних
                            var updateResponse = await client.UpdateAsync($"Fundraising/{fundraisingId}", fundraising);

                            if (updateResponse.StatusCode == HttpStatusCode.OK)
                            {
                                return Ok($"Fundraising {fundraisingId} approved successfully");
                            }
                            else
                            {
                                return BadRequest($"Failed to approve fundraising {fundraisingId}");
                            }
                        }
                        else
                        {
                            return NotFound($"Fundraising {fundraisingId} not found");
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
                return StatusCode(500, $"An error occurred while approving fundraising: {ex.Message}");
            }
        }



        // Метод для генерації нового FundraisingId
        private string GenerateNewFundraisingId()
        {
            return Guid.NewGuid().ToString();
        }

    }
}
