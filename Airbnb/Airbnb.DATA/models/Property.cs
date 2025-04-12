namespace Airbnb.DATA.models
{
    public class Property
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public decimal Price { get; set; }
        public decimal Distance { get; set; }
        public string Description { get; set; }
        public int Maxgeusts { get; set; }
        public virtual ICollection<PropertyImage> propertyImages { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Availability> Availabilities { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public double? AverageRating => Reviews?.Any() == true ? Reviews.Average(r => r.Rating) : null;
    }
}
