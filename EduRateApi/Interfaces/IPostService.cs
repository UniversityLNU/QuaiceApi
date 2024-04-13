using EduRateApi.Dtos.PostsDTO;
using EduRateApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace EduRateApi.Interfaces
{
    public interface IPostService
    {
        public Task<AllPostsResponse> GetAllPosts();
        public Task<PostResponse> GetPostById(string postId);
        public Task<CreatePostResponseDto> UploadUserPost(Posts post);
    }
}
