using System.ComponentModel.DataAnnotations;

namespace Airbnb.Application.DTOs.Property
{
    public class AvailabilityDto
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public bool isBooked { get; set; } = false;
    }

    public class CreateAvailabilityDto
    {
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsBooked { get; set; } = false;

        [Required]
        public int PropertyId { get; set; }
    }

    public class UpdateAvailabilityDto
    {
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsBooked { get; set; }
    }

    public class AvailabilityForPropertyDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsBooked { get; set; }
    }
}
