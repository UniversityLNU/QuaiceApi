using EduRateApi.Models;

namespace EduRateApi.Dtos.PostsDTO
{
    public class AllPostsResponse : ServerResponse
    {
        public List<Posts> postList  { get; set; }

        public AllPostsResponse(int statusCode, string message, List<Posts> postList) : base(message, statusCode)
        {
            this.postList = postList;
        }
    }
}
