using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airbnb.Application.DTOs.Property
{
    public class UpdatePropertyDTO : CreatePropertyDTO
    {

        public int Id { get; set; }
        
    }
}
