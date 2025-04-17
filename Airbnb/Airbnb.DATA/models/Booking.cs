using System.Text.Json.Serialization;

namespace Airbnb.DATA.models
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime CheckInDte { get; set; }
        public DateTime CheckOutDate { get; set; }
        public DateTime? BookingDate { get; set; } = DateTime.UtcNow;
        public int Guests { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public decimal TotalPrice { get; set; }
        public int PropertyId { get; set; }
        [JsonIgnore]
        public virtual Property Property { get; set; }

        public int? ReviewId { get; set; }
        public virtual Review Review { get; set; }
    }
}
