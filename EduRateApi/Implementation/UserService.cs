using EduRateApi.Dtos.UserDtos;
using EduRateApi.Interfaces;
using EduRateApi.Models;
using Firebase.Auth;
using Newtonsoft.Json;
using System.Net;

namespace EduRateApi.Implementation
{
    public class UserService : IUserService
    {
        private readonly IFirebaseConnectingService _firebaseConnectingService;
        public UserService(IFirebaseConnectingService firebaseConnectingService)
        {
            _firebaseConnectingService = firebaseConnectingService;
        }
        public async Task<ServerResponse> UpdateUser(Models.User user)
        {
            try
            {
                using (var client = _firebaseConnectingService.GetFirebaseClient())
                {
                    if (client != null)
                    {
                        var response = await client.UpdateAsync($"Users/{user.userId}", user);
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            return new ServerResponse("User updated successfully", 200);
                        }
                        else
                        {
                            return new ServerResponse("Failed to update user", (int)response.StatusCode);
                        }
                    }
                    else
                    {
                        return new ServerResponse("Firebase connection failed", 400);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServerResponse("An error occurred while updating user in Firebase: " + ex.Message, 500);
            }
        }

        public async Task<UserResponse> GetUserById(string userId)
        {
            try
            {
                using (var client = _firebaseConnectingService.GetFirebaseClient())
                {
                    if (client != null)
                    {
                        var response = await client.GetAsync($"Users/{userId}");
                        if (response.Body != "null")
                        {
                            var user = response.ResultAs<Models.User>();
                            return new UserResponse( user:user ,"User updated successfully", 200);
                        }
                        else
                        {
                            return new UserResponse(user: null, "Not Found", 200);
                        }
                    }
                    else
                    {
                        return new UserResponse(user: null, "Firebase connection failed", 400);
                    }
                }
            }
            catch (Exception ex)
            {
                return new UserResponse(user: null, "An error occurred while retrieving user: ", 400);
            }
        }
    }
}
