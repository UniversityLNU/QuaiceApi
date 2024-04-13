using EduRateApi.Dtos;
using EduRateApi.Interfaces;
using EduRateApi.Models;
using Firebase.Auth;
using FirebaseAdmin.Auth;
using Newtonsoft.Json;

namespace EduRateApi.Implementation
{
    public class AuthService : IAuthService
    {
        private const string API_KEY = "AIzaSyAZKbn_CRFrvs5Mnos_e_URzyROWapLVs8";

        public AuthService()
        {
        }

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
            catch (Firebase.Auth.FirebaseAuthException ex)
            {
                return new ServerResponse(message: "Firebase Authentication error: " + ex.Message, statusCode: 500);
            }
            catch (Exception ex)
            {
                // Обробка інших випадкових винятків
                return new ServerResponse(message: "Error: " + ex.Message, statusCode: 500);
            }
        }

        public async Task<LoginResponse> LoginUserAsync(UserLoginDTO model)
        {
            try
            {
                FirebaseAuthProvider firebaseAuthProvider = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(API_KEY));
                FirebaseAuthLink firebaseAuthLink = await firebaseAuthProvider.SignInWithEmailAndPasswordAsync(model.email, model.password);

                // Отримуємо токен доступу
                string firebaseToken = firebaseAuthLink.FirebaseToken;

                FirebaseToken decodedToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance
                    .VerifyIdTokenAsync(firebaseToken);

                // Додаємо токен доступу до заголовка Authorization у форматі Bearer
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
                // Обробка інших випадкових винятків
                return new LoginResponse(message: "Error: " + ex.Message, statusCode: 500);
            }
        }

        public async Task<LoginResponse> RegisterUserAsync(UserRegisterDTO model)
        {
            try
            {
                FirebaseAuthProvider firebaseAuthProvider = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(API_KEY));

                FirebaseAuthLink firebaseAuthLink = await firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync(model.email, model.password);

                // Створення об'єкта користувача з базовою інформацією
                Models.User user = new Models.User
                {
                    userId = firebaseAuthLink.User.LocalId,
                    email = model.email,
                    fullName = model.fullName,
                    phoneNumber = model.phoneNumber
                };

                // Виклик функції для створення папки користувача
                await CreateNewUserFolder(user);

                // Повернення об'єкту LoginResponse з токеном Firebase
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
                // Обробка інших випадкових винятків
                return new LoginResponse(statusCode: 500, message: "Error: " + ex.Message);
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
                        var setResponse = await client.SetAsync($"Users/{user.userId}", user);
                        if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            Console.WriteLine($"User folder for user with ID {user.userId} created successfully");
                        }
                        else
                        {
                            Console.WriteLine($"Failed to create user folder for user with ID {user.userId}");
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
    }
}
