using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airbnb.Application.DTOs.Booking
{
    public class CreateBookingDTO
    {
        public DateTime CheckInDte { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int UserId { get; set; }
        public int PropertyId { get; set; }
        public int? ReviewId { get; set; }
    }
}
