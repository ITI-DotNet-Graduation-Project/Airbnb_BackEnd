using Microsoft.EntityFrameworkCore;

namespace Airbnb.DATA.models.Identity
{
    [Owned]
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime CreatedIn { get; set; } = DateTime.Now;
        public DateTime ExpiredIn { get; set; }
        public DateTime? RevokedIn { get; set; }
        public bool IsExpired => ExpiredIn < DateTime.Now;
        public bool IsUsed => !IsExpired && RevokedIn is null;
    }
}
