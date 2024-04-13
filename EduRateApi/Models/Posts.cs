namespace EduRateApi.Models
{
    public class Posts
    {
        public string PostId { get; set; }

        public string CreatorFullName { get; set; }
        public string Description { get; set;}

        public string DateOfCreation { get; set; }

        public string FundraisingId { get; set;}

        public List<string> AttachedPhotos { get; set; }   
    }
}
