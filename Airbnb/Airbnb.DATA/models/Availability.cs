using System.ComponentModel.DataAnnotations;

namespace Airbnb.DATA.models
{
    public class Availability
    {
        [Key]
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsBooked { get; set; }
        public int PropertyId { get; set; }
        public virtual Property Property { get; set; }
    }
}
