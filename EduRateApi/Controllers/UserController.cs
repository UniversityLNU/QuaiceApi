using EduRateApi.Dtos;
using FireSharp.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using Firebase.Auth;
using Microsoft.AspNetCore.Authorization;
using FirebaseAdminAuthentication.DependencyInjection.Extensions;
using FirebaseAdminAuthentication.DependencyInjection.Models;
using Microsoft.AspNetCore.Http.Features;
using System.Net.Http.Headers;
using Azure.Core;
using Microsoft.Extensions.Primitives;
using FirebaseAdmin.Auth;
using EduRateApi.Models;
using FirebaseAuthException = Firebase.Auth.FirebaseAuthException;

namespace EduRateApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private const string API_KEY = "AIzaSyAZKbn_CRFrvs5Mnos_e_URzyROWapLVs8";

        public UserController()
        {

        }

        [HttpPost("register")]
        public async Task<ServerResponse> RegisterUserAsync([FromBody] UserRegisterDTO model)
        {
            try
            {
                FirebaseAuthProvider firebaseAuthProvider = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(API_KEY));

                FirebaseAuthLink firebaseAuthLink = await firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync(model.userEmail, model.password);

                Console.WriteLine(firebaseAuthLink.FirebaseToken);

                // Створення об'єкта користувача з базовою інформацією
                Models.User user = new Models.User
                {
                    UserId = firebaseAuthLink.User.LocalId,
                    Email = model.userEmail
                    // Додайте інші необхідні поля тут
                };

                // Виклик функції для створення папки користувача
                await CreateNewUserFolder(user);

                return new ServerResponse(message: firebaseAuthLink.FirebaseToken, statusCode: 200);
            }
            catch (FirebaseAuthException ex)
            {
                if (ex.Reason == AuthErrorReason.EmailExists)
                {
                    return new ServerResponse(message: "Email already exists.", statusCode: 400);
                }
                else if (ex.Reason == AuthErrorReason.InvalidEmailAddress)
                {
                    return new ServerResponse(message: "Invalid email format.", statusCode: 400);
                }
                else if (ex.Reason == AuthErrorReason.WeakPassword)
                {
                    return new ServerResponse(message: "Password is too weak.", statusCode: 400);
                }
                else
                {
                    return new ServerResponse(message: "Firebase Authentication error: " + ex.Message, statusCode: 500);
                }
            }
            catch (Exception ex)
            {
                // Обробка інших випадкових винятків
                return new ServerResponse(message: "Error: " + ex.Message, statusCode: 500);
            }
        }



        [HttpPost("login")]
        public async Task<ServerResponse> LoginUserAsync([FromBody] UserRegisterDTO model)
        {
            try
            {
                FirebaseAuthProvider firebaseAuthProvider = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(API_KEY));
                FirebaseAuthLink firebaseAuthLink = await firebaseAuthProvider.SignInWithEmailAndPasswordAsync(model.userEmail, model.password);

                // Отримуємо токен доступу
                string firebaseToken = firebaseAuthLink.FirebaseToken;

                FirebaseToken decodedToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance
                .VerifyIdTokenAsync(firebaseToken);

                // Додаємо токен доступу до заголовка Authorization у форматі Bearer

                return new ServerResponse(message: firebaseToken, statusCode: 200);
            }
            catch (FirebaseAuthException ex)
            {
                if (ex.Reason == AuthErrorReason.UserNotFound || ex.Reason == AuthErrorReason.WrongPassword)
                {
                    return new ServerResponse(message: "Invalid email or password.", statusCode: 400);
                }
                else
                {
                    return new ServerResponse(message: "Firebase Authentication error: " + ex.Message, statusCode: 500);
                }
            }
            catch (Exception ex)
            {
                // Обробка інших випадкових винятків
                return new ServerResponse(message: "Error: " + ex.Message, statusCode: 500);
            }
        }

        private async Task CreateNewUserFolder(Models.User user)
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
                        // Створення нової папки користувача в базі даних
                        var setResponse = await client.SetAsync($"Users/{user.UserId}", user);
                        if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            Console.WriteLine($"User folder for user with ID {user.UserId} created successfully");
                        }
                        else
                        {
                            Console.WriteLine($"Failed to create user folder for user with ID {user.UserId}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Firebase connection failed");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while creating user folder in Firebase: " + ex.Message);
            }
        }



        [HttpGet("GetInfoAboutUser")]
        public async Task<ServerResponse> GetUserInfoAsync(string firebaseToken)
        {
            try
            {
                FirebaseToken decodedToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance
                    .VerifyIdTokenAsync(firebaseToken);

                string uid = decodedToken.Uid;

                // Повернути тільки Uid користувача
                return new ServerResponse(message: $"{uid}", statusCode: 200);
            }
            catch (FirebaseAuthException ex)
            {
                return new ServerResponse(message: "Firebase Authentication error: " + ex.Message, statusCode: 500);
            }
            catch (Exception ex)
            {
                // Обробка інших випадкових винятків
                return new ServerResponse(message: "Error: " + ex.Message, statusCode: 500);
            }
        }


    }
}
