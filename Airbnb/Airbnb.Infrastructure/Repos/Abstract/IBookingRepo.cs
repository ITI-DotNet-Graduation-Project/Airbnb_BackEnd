using Airbnb.DATA.models;
using Airbnb.Infrastructure.Abstract;

namespace Airbnb.Infrastructure.Repos.Abstract
{
    public interface IBookingRepo : IGenericRepo<Booking>
    {
        public Task<Booking> GetBookingAsync(int id);
        Task<IEnumerable<Booking>> GetUserBookings(int userId);
        Task<bool> IsPropertyBooked(int propertyId, DateTime checkIn, DateTime checkOut);
        Task<IEnumerable<Booking>> GetConfirmedBookingsForPropertyAsync(int propertyId);

    }
}
