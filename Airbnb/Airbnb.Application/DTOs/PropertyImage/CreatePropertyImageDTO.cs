using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airbnb.Application.DTOs.PropertyImage
{
    public class CreatePropertyImageDTO
    {
        public string ImageUrl { get; set; }
        public int PropertyId { get; set; }
    }
}
