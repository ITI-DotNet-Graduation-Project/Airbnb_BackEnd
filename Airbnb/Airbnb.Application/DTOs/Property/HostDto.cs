using Airbnb.Application.DTOs.Property;

namespace Airbnb.Application.Properties
{


    public class HostPropertiesResponse
    {
        public IEnumerable<PropertyDTO> Properties { get; set; }
        public int TotalProperties { get; set; }
        public int AvailableTodayCount { get; set; }
        public int BookedTodayCount { get; set; }
        public double? OverallAverageRating { get; set; }
    }
}
