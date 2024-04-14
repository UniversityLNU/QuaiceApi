using FireSharp.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using Firebase.Auth;
using Microsoft.AspNetCore.Authorization;
using FirebaseAdminAuthentication.DependencyInjection.Models;
using Microsoft.AspNetCore.Http.Features;
using System.Net.Http.Headers;
using Azure.Core;
using Microsoft.Extensions.Primitives;
using FirebaseAdmin.Auth;
using EduRateApi.Models;
using FirebaseAuthException = Firebase.Auth.FirebaseAuthException;
using System.Net;
using EduRateApi.Interfaces;

using EduRateApi.Dtos.UserDtos;

using EduRateApi.Dtos.AuthDTO;


namespace EduRateApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
       
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        public UserController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<LoginResponse>> RegisterUserAsync([FromBody] UserRegisterDTO model)
        {
            var response = await _authService.RegisterUserAsync(model);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> LoginUserAsync([FromBody] UserLoginDTO model)
        {
            var response = await _authService.LoginUserAsync(model);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpGet("GetInfoAboutUser")]
        public async Task<ActionResult<ServerResponse>> GetUserInfoAsync(string firebaseToken)
        {
            var response = await _authService.GetUserInfoAsync(firebaseToken);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpGet("GetUser/{userId}")]
        public async Task<ActionResult<UserResponse>> GetUser(string userId)
        {
            var response = await _userService.GetUserById(userId);
            return Ok(response.user);
        }


    }
}
