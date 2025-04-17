using Airbnb.DATA.models;
using Airbnb.Infrastructure.Context;
using Airbnb.Infrastructure.Repos.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.Infrastructure.Repos.Implementation
{
    public class BookingRepo : GenericRepo<Booking>, IBookingRepo
    {
        private readonly AppDbContext _context;

        public BookingRepo(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Booking> GetBookingAsync(int id)
        {
            return await _context.bookings
                 .Include(b => b.Property)
                .ThenInclude(p => p.PropertyImages)
            .FirstOrDefaultAsync(b => b.Id == id);
        }
        public async Task<IEnumerable<Booking>> GetUserBookings(int userId)
        {
            return await _context.bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Property)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task<bool> IsPropertyBooked(int propertyId, DateTime checkIn, DateTime checkOut)
        {
            return await _context.bookings
                .AnyAsync(b => b.PropertyId == propertyId &&
                              b.CheckOutDate > checkIn &&
                              b.CheckInDte < checkOut);
        }
        public async Task<IEnumerable<Booking>> GetConfirmedBookingsForPropertyAsync(int propertyId)
        {
            var today = DateTime.UtcNow.Date;

            return await _context.bookings
                .Where(b => b.PropertyId == propertyId
                            && b.CheckOutDate.Date >= today)
                .ToListAsync();
        }
    }
}
