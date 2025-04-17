using Airbnb.Application.DTOs.Review;
using Airbnb.Application.Services.Abstract;
using Airbnb.DATA.models;
using Airbnb.Infrastructure.Repos.Abstract;
using AutoMapper;

namespace Airbnb.Application.Services.Implementation
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepo _reviewRepository;
        private readonly IUserRepo _userRepository;
        private readonly IPropertyRepo _propertyRepository;
        private readonly IBookingRepo _bookingRepository;
        private readonly IMapper _mapper;

        public ReviewService(
            IReviewRepo reviewRepository,
            IUserRepo userRepository,
            IPropertyRepo propertyRepository,
            IBookingRepo bookingRepository,
            IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _userRepository = userRepository;
            _propertyRepository = propertyRepository;
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewDTO>> GetReviewsForProperty(int propertyId)
        {
            var reviews = await _reviewRepository.GetReviewsForProperty(propertyId);
            return _mapper.Map<IEnumerable<ReviewDTO>>(reviews);
        }

        public async Task<ReviewDTO> AddReview(CreateReviewDTO reviewDto, int userId)
        {
            var booking = await _bookingRepository.GetByIdAsync(reviewDto.BookingId);
            if (booking == null || booking.UserId != userId || booking.PropertyId != reviewDto.PropertyId)
            {
                throw new UnauthorizedAccessException("Invalid booking for review");
            }

            var review = new Review
            {
                Rating = reviewDto.Rating,
                Comments = reviewDto.Comment,
                PropertyId = reviewDto.PropertyId,
                BookingId = reviewDto.BookingId,
                UserId = userId,
                ReviewDate = DateTime.UtcNow
            };

            var createdReview = await _reviewRepository.AddAsync(review);
            await _reviewRepository.SaveChangesAsync();


            var user = await _userRepository.GetByIdAsync(userId);

            return new ReviewDTO
            {
                Id = createdReview.Id,
                Rating = createdReview.Rating.Value,
                Comment = createdReview.Comments,
                CreatedAt = createdReview.ReviewDate,
                PropertyId = createdReview.PropertyId,
                UserId = userId,
                UserName = $"{user.FirstName} {user.LastName}"
            };
        }

        public async Task<double> GetAverageRating(int propertyId)
        {
            return await _reviewRepository.GetAverageRating(propertyId);
        }
    }

}
