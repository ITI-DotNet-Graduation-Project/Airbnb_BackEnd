using Airbnb.Application.DTOs;
using Airbnb.Application.DTOs.Booking;
using Airbnb.Application.Services.Abstract;
using Airbnb.DATA.models;
using Airbnb.Infrastructure.Abstract;
using AutoMapper;

namespace Airbnb.Application.Services.Implementation
{
    public class BookingService : IBookingService
    {
        private readonly IGenericRepo<Booking> _repo;
        private readonly IMapper _mapper;

        public BookingService(IGenericRepo<Booking> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookingDTO>> GetAllAsync()
        {
            var bookings = _repo.GetTableNoTracking();
            return _mapper.Map<IEnumerable<BookingDTO>>(bookings);
        }

        public async Task<BookingDTO> GetByIdAsync(int id)
        {
            var booking = await _repo.GetByIdAsync(id);
            return _mapper.Map<BookingDTO>(booking);
        }

        public async Task<BookingDTO> CreateAsync(CreateBookingDTO dto)
        {
            var booking = _mapper.Map<Booking>(dto);
            var created = await _repo.AddAsync(booking);
            return _mapper.Map<BookingDTO>(created);
        }

        public async Task UpdateAsync(UpdateBookingDTO dto)
        {
            var booking = await _repo.GetByIdAsync(dto.Id);
            if (booking == null) return;

            booking.CheckInDte = dto.CheckInDte;
            booking.CheckOutDate = dto.CheckOutDate;
            booking.ReviewId = dto.ReviewId;

            await _repo.UpdateAsync(booking);
        }

        public async Task DeleteAsync(int id)
        {
            var booking = await _repo.GetByIdAsync(id);
            if (booking != null)
                await _repo.DeleteAsync(booking);
        }
    }
}

