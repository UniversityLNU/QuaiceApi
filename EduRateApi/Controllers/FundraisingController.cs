﻿using EduRateApi.Dtos;
using EduRateApi.Dtos.Fundraising;
using EduRateApi.Models;
using FireSharp.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;

namespace EduRateApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FundraisingController : ControllerBase
    {

        [HttpPost("UploadFundrasing")]
        public async Task<ServerResponse> UploadFundrasing([FromBody] FundraisingDto fundraisingDto)
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
                        if (response.Body != "null") // якщо не null, то запис вже існує у базі даних
                        {
                            return new LoginResponse(message: "Already Exist", statusCode: 500);
                        }
                        else
                        {
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
                                return new LoginResponse(message: "Fundraising uploaded successfully", statusCode: 200);
                            }
                            else
                            {
                                return new LoginResponse(message: "Failed to upload fundraising", statusCode: 400);
                            }
                        }
                    }
                    else
                    {
                        return new LoginResponse(message: "Firebase connection failed", statusCode: 400);
                    }
                }
            }
            catch (Exception ex)
            {
                return new LoginResponse(message: "Error occurred while uploading fundraising to Firebase", statusCode: 500);
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


        // Метод для генерації нового FundraisingId
        private string GenerateNewFundraisingId()
        {
            return Guid.NewGuid().ToString();
        }
  
    }
}
