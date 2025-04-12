using Airbnb.DATA.models.Identity;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.DATA.models
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ImageUrl { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Property> Properties { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }

    }
}
