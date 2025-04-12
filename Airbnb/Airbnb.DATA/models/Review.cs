namespace Airbnb.DATA.models
{
    public class Review
    {
        public int Id { get; set; }
        public double? Rating { get; set; }
        public string Comments { get; set; }
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
        public int PropertyId { get; set; }
        public virtual Property Property { get; set; }
        public int BookingId { get; set; }
        public virtual Booking Booking { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
