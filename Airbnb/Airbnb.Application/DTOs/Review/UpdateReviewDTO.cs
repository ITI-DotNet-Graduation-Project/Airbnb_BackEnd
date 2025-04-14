using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airbnb.Application.DTOs.Review
{
    public class UpdateReviewDTO
    {
        public int Id { get; set; }
        public double? Rating { get; set; }
        public string Comments { get; set; }
    }
}
