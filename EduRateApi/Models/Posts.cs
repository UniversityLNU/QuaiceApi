namespace EduRateApi.Models
{
    public class Posts
    {
        public string CreatorFullName { get; set; }
        public string Description { get; set;}

        public long DateOfCreation { get; set; }

        public string FundraisingId { get; set;}

        public List<string> AttachedPhotos { get; set; }   
    }
}
