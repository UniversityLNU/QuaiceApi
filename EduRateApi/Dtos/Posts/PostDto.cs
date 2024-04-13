namespace EduRateApi.Dtos.Posts
{
    public class PostDto
    {
        public string postId { get; set; }
        public string creatorFullName { get; set; }
        public string description { get; set; }
        public long dateOfCreation { get; set; }
        public string fundraisingId { get; set; }
        public List<string> attachedPhotos { get; set; }
    }
}
