namespace Airbnb.Application.DTOs.Booking
{
    public class BookingDTO
    {
        public int? Id { get; set; }
        public DateTime CheckInDte { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int PropertyId { get; set; }
        public int Guests { get; set; }
    }
    public class BookingResponseDTO
    {
        public int Id { get; set; }
        public DateTime CheckInDte { get; set; }
        public DateTime CheckOutDate { get; set; }
        public DateTime BookingDate { get; set; }
        public int Guests { get; set; }
        public int PropertyId { get; set; }
        public string PropertyTitle { get; set; }
        public string PropertyImage { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
