using EduRateApi.Dtos.AuthDTO;
using EduRateApi.Interfaces;
using EduRateApi.Models;
using Firebase.Auth;
using FirebaseAdmin.Auth;
using Newtonsoft.Json;

namespace EduRateApi.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IFirebaseConnectingService _connectingService;
        private readonly string API_KEY;
        public AuthService(IFirebaseConnectingService _connectingService)
        {
            this._connectingService = _connectingService;
            API_KEY = _connectingService.GetApiKey();
        }

        public async Task<ServerResponse> GetUserInfoAsync(string firebaseToken)
        {
            try
            {
                FirebaseToken decodedToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance
                    .VerifyIdTokenAsync(firebaseToken);

                string uid = decodedToken.Uid;

                return new ServerResponse(message: $"{uid}", statusCode: 200);
            }
            catch (Firebase.Auth.FirebaseAuthException ex)
            {
                return new ServerResponse(message: "Firebase Authentication error: " + ex.Message, statusCode: 500);
            }
            catch (Exception ex)
            {
                return new ServerResponse(message: "Error: " + ex.Message, statusCode: 500);
            }
        }

        public async Task<LoginResponse> LoginUserAsync(UserLoginDTO model)
        {
            try
            {
                FirebaseAuthProvider firebaseAuthProvider = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(API_KEY));
                FirebaseAuthLink firebaseAuthLink = await firebaseAuthProvider.SignInWithEmailAndPasswordAsync(model.email, model.password);

                string firebaseToken = firebaseAuthLink.FirebaseToken;

                FirebaseToken decodedToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance
                    .VerifyIdTokenAsync(firebaseToken);

                return new LoginResponse(message: "Succesfully logined", statusCode: 200, jwtToken: firebaseToken, userId: firebaseAuthLink.User.LocalId);
            }
            catch (Firebase.Auth.FirebaseAuthException ex)
            {
                if (ex.Reason == AuthErrorReason.UserNotFound || ex.Reason == AuthErrorReason.WrongPassword)
                {
                    return new LoginResponse(message: "Invalid email or password.", statusCode: 400);
                }
                else
                {
                    return new LoginResponse(message: "Firebase Authentication error: " + ex.Message, statusCode: 500);
                }
            }
            catch (Exception ex)
            {
                return new LoginResponse(message: "Error: " + ex.Message, statusCode: 500);
            }
        }

        public async Task<LoginResponse> RegisterUserAsync(UserRegisterDTO model)
        {
            try
            {
                FirebaseAuthProvider firebaseAuthProvider = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(API_KEY));

                FirebaseAuthLink firebaseAuthLink = await firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync(model.email, model.password);

                Models.User user = new Models.User
                {
                    userId = firebaseAuthLink.User.LocalId,
                    email = model.email,
                    fullName = model.fullName,
                    phoneNumber = model.phoneNumber
                };

                await CreateNewUserFolder(user);

                return new LoginResponse(statusCode: 200, message: "Succesfully registered", jwtToken: firebaseAuthLink.FirebaseToken, userId: firebaseAuthLink.User.LocalId);
            }
            catch (Firebase.Auth.FirebaseAuthException ex)
            {
                if (ex.Reason == AuthErrorReason.EmailExists)
                {
                    return new LoginResponse(statusCode: 400, message: "Email already exists.");
                }
                else if (ex.Reason == AuthErrorReason.InvalidEmailAddress)
                {
                    return new LoginResponse(statusCode: 400, message: "Invalid email format.");
                }
                else if (ex.Reason == AuthErrorReason.WeakPassword)
                {
                    return new LoginResponse(statusCode: 400, message: "Password is too weak.");
                }
                else
                {
                    return new LoginResponse(statusCode: 500, message: "Firebase Authentication error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                return new LoginResponse(statusCode: 500, message: "Error: " + ex.Message);
            }
        }

        private async Task<ServerResponse> CreateNewUserFolder(Models.User user)
        {
            try
            {
                using (var client = _connectingService.GetFirebaseClient())
                {
                    if (client != null)
                    {
                        var setResponse = await client.SetAsync($"Users/{user.userId}", user);
                        if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            return new ServerResponse($"User folder for user with ID {user.userId} created successfully", 200);
                        }
                        else
                        {
                            return new ServerResponse($"Failed to create user folder for user with ID {user.userId}", 500);
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
                return new ServerResponse("An error occurred while creating user folder in Firebase: " + ex.Message, 500);
            }
        }
    }
}
