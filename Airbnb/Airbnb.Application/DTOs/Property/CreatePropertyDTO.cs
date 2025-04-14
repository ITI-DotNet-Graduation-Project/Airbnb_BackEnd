using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airbnb.Application.DTOs.Property
{
    public class CreatePropertyDTO
    {
        public string Title { get; set; }
        public string Location { get; set; }
        public decimal Price { get; set; }
        public decimal Distance { get; set; }
        public string Description { get; set; }
        public int MaxGuests { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
    }
}
