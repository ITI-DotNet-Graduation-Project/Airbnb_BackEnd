using Airbnb.Application.DTOs.Booking;
using Airbnb.DATA.models;

namespace Airbnb.Application.Services.Abstract
{
    public interface IBookingService
    {
        Task<Booking> CreateBookingAsync(BookingDTO bookingDto, int userId);
        Task<BookingResponseDTO> GetBookingByIdAsync(int id, int userId);
        Task<IEnumerable<BookingResponseDTO>> GetUserBookingsAsync(int userId);
        Task CancelBookingAsync(int id, int userId);
        Task<IEnumerable<string>> GetBookedDatesForPropertyAsync(int propertyId);
        public Task<IEnumerable<BookingDTO>> GetUserBookings(int userId);

    }
}
