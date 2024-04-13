
using EduRateApi.Dtos.FundraisingDTO;
using EduRateApi.Interfaces;
using EduRateApi.Models;
using FireSharp.Config;
using Newtonsoft.Json;
using System.Net;

namespace EduRateApi.Implementation
{
    public class FundraisingService : IFundraisingService
    {
        public async Task<ServerResponse> ApproveFundraising(string fundraisingId)
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
                                return new ServerResponse(message: $"Fundraising {fundraisingId} approved successfully", statusCode: 200);
                            }
                            else
                            {
                                return new ServerResponse(message: $"Failed to approve fundraising {fundraisingId}", statusCode: 400);
                            }
                        }
                        else
                        {
                            return new ServerResponse(message: $"Fundraising {fundraisingId} not found", statusCode: 404);
                        }
                    }
                    else
                    {
                        return new ServerResponse(message: "Firebase connection failed", statusCode: 400);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServerResponse(message: $"An error occurred while approving fundraising: {ex.Message}", statusCode: 500);
            }
        }

        public async Task<AllFundraisingResponse> GetAllApprovedFundraisings()
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
                        if (response.Body != "null")
                        {
                            var data = response.ResultAs<Dictionary<string, Fundraising>>();

                            var approvedFundraisings = data.Values.Where(f => f.іsApproved).ToList();

                            return new AllFundraisingResponse(fundraising: approvedFundraisings, message: "OK", statusCode: 200);
                        }
                        else
                        {
                            return new AllFundraisingResponse(fundraising: new List<Fundraising>(), message: "OK , but 0 object", statusCode: 200);
                        }
                    }
                    else
                    {
                        return new AllFundraisingResponse(fundraising: new List<Fundraising>(), message: "BAD", statusCode: 400);
                    }
                }
            }
            catch (Exception ex)
            {
                return new AllFundraisingResponse(fundraising: new List<Fundraising>(), message: "BAD", statusCode: 400);
            }
        }

        public async Task<FundraisingResponse> GetFundraisingById(string fundraisingId)
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
                        if (response.Body != "null")
                        {
                            var fundraising = response.ResultAs<Fundraising>();
                            return new FundraisingResponse(fundraising, message: "Fundraising found", statusCode: 200);
                        }
                        else
                        {
                            return new FundraisingResponse(new Fundraising(), message: "Fundraising not found", statusCode: 404);
                        }
                    }
                    else
                    {
                        return new FundraisingResponse(new Fundraising(), message: "Firebase connection failed", statusCode: 400);
                    }
                }
            }
            catch (Exception ex)
            {
                return new FundraisingResponse(new Fundraising(), message: $"An error occurred while retrieving fundraising: {ex.Message}", statusCode: 500);
            }
        }

        public async Task<AllFundraisingResponse> GetUnapprovedFundraisings()
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
                        if (response.Body != "null") 
                        {
 
                            var data = response.ResultAs<Dictionary<string, Fundraising>>();
        
                            var unapprovedFundraisings = data.Values.Where(f => !f.іsApproved).ToList();

                            return new AllFundraisingResponse(fundraising: unapprovedFundraisings , message:"OK", statusCode:200);
                        }
                        else
                        {
                            // Якщо записів немає, повертаємо порожній список
                            return new AllFundraisingResponse(fundraising: new List<Fundraising>(), message: "OK , but 0 object", statusCode: 200);
                        }
                    }
                    else
                    {
                        // Повертаємо помилку, якщо відсутнє з'єднання з Firebase
                        return new AllFundraisingResponse(fundraising: new List<Fundraising>(), message: "BAD", statusCode: 400);
                    }
                }
            }
            catch (Exception ex)
            {
                // Перехоплення та повернення винятку
                return new AllFundraisingResponse(fundraising: new List<Fundraising>(), message: "BAD", statusCode: 400);
            }
        }

        public async Task<ServerResponse> UploadFundrasing(FundraisingDto fundraisingDto)
        {
            try
            {
                var firebaseConfigPath = "Config/firebaseConfig.json";
                var configJson = System.IO.File.ReadAllText(firebaseConfigPath);
                var config = JsonConvert.DeserializeObject<FirebaseConfig>(configJson);

                using (var _client = new FireSharp.FirebaseClient(config))
                {
                    if (_client != null)
                    {
                        string fundraisingId = Guid.NewGuid().ToString();

                        var response = await _client.GetAsync($"Fundraising/{fundraisingId}");

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

                        var setResponse = await _client.SetAsync($"Fundraising/{fundraising.fundraisingId}", fundraising);

                        if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            Console.WriteLine($"Fundraising {fundraising.fundraisingId} uploaded successfully");
                            return new ServerResponse(message: "Fundraising uploaded successfully", statusCode: 200);
                        }
                        else
                        {
                            return new ServerResponse(message: "Failed to upload fundraising", statusCode: 400);
                        }
                    }
                    else
                    {
                        return new ServerResponse(message: "Failed to upload fundraising", statusCode: 400);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServerResponse(message: "Error occurred while uploading fundraising to Firebase", statusCode: 500);
            }
        }
    }
}
