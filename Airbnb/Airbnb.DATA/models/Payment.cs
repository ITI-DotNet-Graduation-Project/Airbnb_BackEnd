using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airbnb.DATA.models
{
    public class Payment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } // e.g., CreditCard, PayPal, etc.
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        // Linking to Booking
        public int BookingId { get; set; }
        public virtual Booking Booking { get; set; }
    }
}
