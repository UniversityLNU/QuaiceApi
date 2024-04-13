using EduRateApi.Models;

namespace EduRateApi.Dtos.PostsDTO
{
    public class AllPostsResponse : ServerResponse
    {
        public List<Posts> posts { get; set; }

        public AllPostsResponse(int statusCode, string message, List<Posts> posts) : base(message, statusCode)
        {
            this.posts = posts;
        }
    }
}
