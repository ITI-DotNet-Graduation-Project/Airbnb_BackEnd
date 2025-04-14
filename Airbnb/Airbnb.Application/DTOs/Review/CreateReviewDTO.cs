using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airbnb.Application.DTOs.Review
{
    public class CreateReviewDTO
    {
        public double? Rating { get; set; }
        public string Comments { get; set; }
        public int PropertyId { get; set; }
        public int BookingId { get; set; }
        public int UserId { get; set; }
    }
}
