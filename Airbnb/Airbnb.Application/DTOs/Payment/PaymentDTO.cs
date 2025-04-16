using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airbnb.Application.DTOs.Payment
{
        public class PaymentDTO
        {
            public int Id { get; set; }
            public int BookingId { get; set; }
            public decimal Amount { get; set; }
            public string PaymentMethod { get; set; } // e.g., CreditCard, PayPal
            public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        }

       
    
}
