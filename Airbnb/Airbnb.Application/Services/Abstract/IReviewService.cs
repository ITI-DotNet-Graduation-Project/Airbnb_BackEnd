using Airbnb.Application.DTOs;
using Airbnb.Application.DTOs.Review;

namespace Airbnb.Application.Services.Abstract
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDTO>> GetAllAsync();
        Task<ReviewDTO> GetByIdAsync(int id);
        Task<ReviewDTO> CreateAsync(CreateReviewDTO dto);
        Task UpdateAsync(UpdateReviewDTO dto);
        Task DeleteAsync(int id);
    }
}

