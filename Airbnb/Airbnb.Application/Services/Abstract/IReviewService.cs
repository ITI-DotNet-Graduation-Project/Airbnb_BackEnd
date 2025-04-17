using Airbnb.Application.DTOs.Review;

namespace Airbnb.Application.Services.Abstract
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDTO>> GetReviewsForProperty(int propertyId);
        Task<ReviewDTO> AddReview(CreateReviewDTO reviewDto, int userId);
        Task<double> GetAverageRating(int propertyId);
    }
}

