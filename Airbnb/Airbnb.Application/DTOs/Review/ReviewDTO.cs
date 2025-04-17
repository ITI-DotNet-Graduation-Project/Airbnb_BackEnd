namespace Airbnb.Application.DTOs.Review
{

    public class ReviewDTO
    {
        public int Id { get; set; }
        public double Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PropertyId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
    public class CreateReviewDTO
    {
        public double Rating { get; set; }
        public string Comment { get; set; }
        public int PropertyId { get; set; }
        public int BookingId { get; set; }
    }
}
