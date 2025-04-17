using Airbnb.Application.DTOs.Review;

namespace Airbnb.Application.DTOs.Property
{

    public class PropertyDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public decimal Price { get; set; }
        public decimal Distance { get; set; }
        public string Description { get; set; }

        public int MaxGuest { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }


        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int UserId { get; set; }
        public int CategoryId { get; set; }

        public string CategoryDescription { get; set; }
        public string UserName { get; set; }

        public List<PropertyImageDto> PropertyImages { get; set; }

        public double? AverageRating { get; set; }

        public List<ReviewDTO>? Reviews { get; set; }
        public ICollection<AvailabilityDto> Availabilities { get; set; }
    }

}
