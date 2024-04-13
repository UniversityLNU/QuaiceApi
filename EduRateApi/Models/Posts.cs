namespace EduRateApi.Models
{
    public class Posts
    {
        public string creatorFullName { get; set; }
        public string description { get; set;}
        public string userId { get; set; }
        public long dateOfCreation { get; set; }

        public string fundraisingId { get; set;}

        public List<string> attachedPhotos { get; set; }

       
    }
}
