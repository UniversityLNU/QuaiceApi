namespace EduRateApi.Dtos.Posts
{
    public class PostDto
    {
        public string userId { get; set; }
        public string creatorFullName { get; set; }
        public string description { get; set; }
        public long dateOfCreation { get; set; }
        public string fundraisingId { get; set; }
        public List<string> attachedPhotos { get; set; }

        public PostDto (string userId, string creatorFullName,
            string description,
            long dateOfCreation,
            string fundraisingId,
            List<string> attachedPhotos
        )
        {
            this.userId = userId;
            this.creatorFullName = creatorFullName;
            this.description = description;
            this.dateOfCreation = dateOfCreation;
            this.attachedPhotos = attachedPhotos;
            this.fundraisingId = fundraisingId;
        }
    }
}
