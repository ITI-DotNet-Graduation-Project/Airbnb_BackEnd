using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Airbnb.Application.DTOs.Property
{
    public class CreatePropertyDTO
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Bedrooms { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Bathrooms { get; set; }

        [Required]
        public string PropertyType { get; set; }
        public string UserId { get; set; }
        public List<string> Amenities { get; set; } = new List<string>();
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }
}
