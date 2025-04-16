namespace Airbnb.Application.DTOs.Review
{

    public class ReviewDTO
    {
        public int Id { get; set; }
        public double? Rating { get; set; }
        public string Comments { get; set; }
        public DateTime ReviewDate { get; set; }
        public int PropertyId { get; set; }
        public int BookingId { get; set; }
        public int UserId { get; set; }
    }
}
