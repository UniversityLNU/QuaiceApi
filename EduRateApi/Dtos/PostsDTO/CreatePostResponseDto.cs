using EduRateApi.Models;

namespace EduRateApi.Dtos.PostsDTO
{
    public class CreatePostResponseDto : ServerResponse
    {
        public string postId { get; set; }
        public List<string> photoLinks { get; set; }

        public CreatePostResponseDto(string message , int statusCode, string postId, List<string> photoLinks): base(message , statusCode)
        {
            this.postId = postId;
            this.photoLinks = photoLinks;
        }
    }
}
