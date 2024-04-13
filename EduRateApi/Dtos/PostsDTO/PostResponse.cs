using EduRateApi.Models;

namespace EduRateApi.Dtos.PostsDTO
{
    public class PostResponse : ServerResponse
    {
        public Posts posts { get; set; }
        public PostResponse(Posts posts ,string message, int statusCode) : base(message, statusCode)
        {
            this.posts = posts;
        }
    }
}
