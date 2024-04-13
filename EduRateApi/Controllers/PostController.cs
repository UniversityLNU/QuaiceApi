
using EduRateApi.Dtos.PostsDTO;
using EduRateApi.Implementation;
using EduRateApi.Interfaces;
using EduRateApi.Models;
using FireSharp.Config;
using FireSharp.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace EduRateApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {

        private readonly IPostService _postService;
        public PostController(IPostService postService)
        {
            _postService = postService;
        }
        [HttpPost("UploadUserPost")]
        public async Task<ActionResult<CreatePostResponseDto>> UploadUserPost([FromBody] Posts post)
        {
            var response = await _postService.UploadUserPost(post);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpGet("GetPostById/{postId}")]
        public async Task<ActionResult<PostResponse>> GetPostById(string postId)
        {
            var response = await _postService.GetPostById(postId);
            return StatusCode((int)response.statusCode, response);
        }


        [HttpGet("GetAllPosts")]
        public async Task<ActionResult<AllPostsResponse>> GetAllPosts()
        {
            var response = await _postService.GetAllPosts();
            return StatusCode((int)response.statusCode, response);
        }

    }


}
