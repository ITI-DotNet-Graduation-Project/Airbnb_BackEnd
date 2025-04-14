using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airbnb.Application.DTOs.Booking
{
    public class UpdateBookingDTO
    {
        public int Id { get; set; }
        public DateTime CheckInDte { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int? ReviewId { get; set; }
    }
}
