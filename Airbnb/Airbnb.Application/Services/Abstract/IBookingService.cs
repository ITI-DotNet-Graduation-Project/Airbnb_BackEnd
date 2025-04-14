using Airbnb.Application.DTOs.Booking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airbnb.Application.Services.Abstract
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingDTO>> GetAllAsync();
        Task<BookingDTO> GetByIdAsync(int id);
        Task<BookingDTO> CreateAsync(CreateBookingDTO dto);
        Task UpdateAsync(UpdateBookingDTO dto);
        Task DeleteAsync(int id);
    }
}
