namespace EduRateApi.Models
{
    public class Fundraising
    {
        public string FundraisingId {  get; set; }

        public string Title { get; set; }

        public string FundraisingUrl { get; set; }
        public string Description { get; set; }
        public string FundraisingCompany { get; set; }

        public double Goal { get; set; }

        public string FundraisingType { get; set; }
    }
}
