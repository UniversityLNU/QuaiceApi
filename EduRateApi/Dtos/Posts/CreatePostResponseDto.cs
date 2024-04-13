namespace EduRateApi.Dtos.Posts
{
    public class CreatePostResponseDto
    {
        public string postId { get; set; }
        public List<string> photoLinks { get; set; }

        public CreatePostResponseDto(string postId, List<string> photoLinks)
        {
            this.postId = postId;
            this.photoLinks = photoLinks;
        }

        public CreatePostResponseDto() { }
    }
}
