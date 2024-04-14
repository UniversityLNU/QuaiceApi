
using EduRateApi.Dtos;
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
        private readonly IFirebaseConnectingService _firebaseConnectingService;
        private readonly IMailService _MailService;
        public FundraisingService(IFirebaseConnectingService firebaseConnectingService, IMailService mashMailService)
        {
            _firebaseConnectingService = firebaseConnectingService;
            _MailService = mashMailService;
        }
        public async Task<ServerResponse> ApproveDeclineFundraising(ChangeStatusResponse changeStatusResponse)
        {
            try
            {
                using (var client = _firebaseConnectingService.GetFirebaseClient())
                {
                    if (client != null)
                    {
                        var response = await client.GetAsync($"Fundraising/{changeStatusResponse.fundraisingId}");
                        if (response.Body != "null")
                        {
                            var fundraising = response.ResultAs<Fundraising>();

                            if (changeStatusResponse.newsStatus == FundraisingStatus.Approved)
                                fundraising.status = FundraisingStatus.Approved;
                           
                            if (changeStatusResponse.newsStatus == FundraisingStatus.Decline)
                                fundraising.status = FundraisingStatus.Decline;

                            var updateResponse = await client.UpdateAsync($"Fundraising/{changeStatusResponse.fundraisingId}", fundraising);
                            FundraisingResponse response1 = await GetFundraisingById(changeStatusResponse.fundraisingId);
                            if (updateResponse.StatusCode == HttpStatusCode.OK)
                            {
                                if (changeStatusResponse.newsStatus == FundraisingStatus.Approved)
                                {
                                            _MailService.SendMail(new SendMailDTO(
                                         response1.fundraising.userName,
                                         response1.fundraising.email,
                                         response1.fundraising.title,
                                         File.ReadAllText("Templates/confirmationHtml.html")
                                    ));
                                    return new ServerResponse(message: $"Fundraising {changeStatusResponse.fundraisingId} approved successfully", statusCode: 200);
                                }
                                   
                                else if (changeStatusResponse.newsStatus == FundraisingStatus.Decline)
                                {
                                             _MailService.SendMail(new SendMailDTO(
                                         response1.fundraising.userName,
                                         response1.fundraising.email,
                                         response1.fundraising.title,
                                         File.ReadAllText("Templates/rejectionHtml.html")
                                    ));
                                    return new ServerResponse(message: $"Fundraising {changeStatusResponse.fundraisingId} declined successfully", statusCode: 200);
                                }
                                    
                            }
                            else
                            {
                                return new ServerResponse(message: $"Failed to update fundraising {changeStatusResponse.fundraisingId}", statusCode: 400);
                            }
                        }
                        else
                        {
                            return new ServerResponse(message: $"Fundraising {changeStatusResponse.fundraisingId} not found", statusCode: 404);
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
                return new ServerResponse(message: $"An error occurred while approving or declining fundraising: {ex.Message}", statusCode: 500);
            }

            return new ServerResponse(message: "Unhandled error occurred", statusCode: 500);
        }

        public async Task<AllFundraisingResponse> GetAllApprovedFundraisings()
        {
            try
            {
                using (var client = _firebaseConnectingService.GetFirebaseClient())
                {
                    if (client != null)
                    {
                        var response = await client.GetAsync("Fundraising");
                        if (response.Body != "null")
                        {
                            var data = response.ResultAs<Dictionary<string, Fundraising>>();

                            var approvedFundraisings = data.Values.Where(f => f.status == FundraisingStatus.Approved).ToList();

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
                using (var client = _firebaseConnectingService.GetFirebaseClient())
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

        public async Task<AllFundraisingResponse> GetAllPendingFundraising() 
        {
            try
            {
                using (var client = _firebaseConnectingService.GetFirebaseClient())
                {
                    if (client != null)
                    {
                        var response = await client.GetAsync("Fundraising");
                        if (response.Body != "null") 
                        {
 
                            var data = response.ResultAs<Dictionary<string, Fundraising>>();
        
                            var unapprovedFundraisings = data.Values.Where(f => f.status == FundraisingStatus.Pending).ToList();

                            return new AllFundraisingResponse(fundraising: unapprovedFundraisings , message:"OK", statusCode:200);
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

        public async Task<ServerResponse> UploadFundrasing(FundraisingDto fundraisingDto)
        {
            try
            {
                using (var client = _firebaseConnectingService.GetFirebaseClient())
                {
                    if (client != null)
                    {
                        string fundraisingId = Guid.NewGuid().ToString();

                        var response = await client.GetAsync($"Fundraising/{fundraisingId}");

                        Fundraising fundraising = new Fundraising
                        {
                            fundraisingId = fundraisingId,
                            phoneNumber = fundraisingDto.phoneNumber,
                            userName = fundraisingDto.userName,
                            email = fundraisingDto.email,
                            title = fundraisingDto.title,
                            fundraisingUrl = fundraisingDto.fundraisingUrl,
                            description = fundraisingDto.description,
                            fundraisingCompany = fundraisingDto.fundraisingCompany,
                            goal = fundraisingDto.goal,
                            fundraisingType = fundraisingDto.fundraisingType,
                            status = FundraisingStatus.Pending
                        };

                        var setResponse = await client.SetAsync($"Fundraising/{fundraising.fundraisingId}", fundraising);

                        if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
                        {
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
