using Airbnb.Application.DTOs.Booking;
using Airbnb.Application.Services.Abstract;
using Airbnb.DATA.models;
using Airbnb.Infrastructure.UnitOfWorks;
using AutoMapper;

namespace Airbnb.Application.Services.Implementation
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<BookingDTO>> GetUserBookings(int userId)
        {
            var bookings = await _unitOfWork.BookingRepo.GetUserBookings(userId);


            return _mapper.Map<IEnumerable<BookingDTO>>(bookings);
        }
        public async Task<Booking> CreateBookingAsync(BookingDTO bookingDto, int userId)
        {

            if (bookingDto.CheckInDte >= bookingDto.CheckOutDate)
                throw new ArgumentException("Check-out date must be after check-in date");


            var isAvailable = await _unitOfWork.PropertyRepo
                .IsPropertyAvailable(bookingDto.PropertyId, bookingDto.CheckInDte, bookingDto.CheckOutDate);

            if (!isAvailable)
                throw new InvalidOperationException("Property not available for selected dates");


            var property = await _unitOfWork.PropertyRepo.GetByIdAsync(bookingDto.PropertyId);
            if (property == null)
                throw new ArgumentException("Property not found");


            var nights = (bookingDto.CheckOutDate - bookingDto.CheckInDte).Days;
            var totalPrice = property.Price * nights;


            var booking = _mapper.Map<Booking>(bookingDto);
            booking.UserId = userId;
            booking.BookingDate = DateTime.UtcNow;
            booking.TotalPrice = totalPrice;

            await _unitOfWork.BookingRepo.AddAsync(booking);


            return booking;
        }
        public async Task<IEnumerable<string>> GetBookedDatesForPropertyAsync(int propertyId)
        {
            var bookings = await _unitOfWork.BookingRepo.GetConfirmedBookingsForPropertyAsync(propertyId);
            var bookedDates = new HashSet<string>();

            foreach (var booking in bookings)
            {

                if (booking.CheckInDte == default || booking.CheckOutDate == default)
                {
                    continue;
                }


                if (booking.CheckInDte > booking.CheckOutDate)
                {
                    continue;
                }


                int totalDays = (booking.CheckOutDate - booking.CheckInDte).Days + 1;
                if (totalDays > 365 * 2)
                {
                    throw new InvalidOperationException($"Unrealistic booking duration: {totalDays} days");
                }


                for (int i = 0; i < totalDays; i++)
                {
                    var currentDate = booking.CheckInDte.AddDays(i);
                    bookedDates.Add(currentDate.ToString("yyyy-MM-dd"));
                }
            }

            return bookedDates.OrderBy(d => d).ToList();
        }

        public async Task<BookingResponseDTO> GetBookingByIdAsync(int id, int userId)
        {
            var booking = await _unitOfWork.BookingRepo
                .GetBookingAsync(id);

            if (booking == null)
                return null;

            if (booking.UserId != userId)
                throw new UnauthorizedAccessException();

            return _mapper.Map<BookingResponseDTO>(booking);
        }

        public async Task<IEnumerable<BookingResponseDTO>> GetUserBookingsAsync(int userId)
        {
            var bookings = await _unitOfWork.BookingRepo
                .GetUserBookings(userId);

            return _mapper.Map<IEnumerable<BookingResponseDTO>>(bookings);
        }

        public async Task CancelBookingAsync(int id, int userId)
        {
            var booking = await _unitOfWork.BookingRepo.GetByIdAsync(id);

            if (booking == null)
                throw new ArgumentException("Booking not found");

            if (booking.UserId != userId)
                throw new UnauthorizedAccessException();

            if (booking.CheckInDte <= DateTime.UtcNow.AddDays(1))
                throw new ArgumentException("Cannot cancel booking within 24 hours of check-in");

            await _unitOfWork.BookingRepo.DeleteAsync(booking);
            await _unitOfWork.CommitAsync();
        }
    }

}

