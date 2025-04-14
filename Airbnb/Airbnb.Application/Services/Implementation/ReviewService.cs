using Airbnb.Application.DTOs;
using Airbnb.Application.DTOs.Review;
using Airbnb.Application.Services.Abstract;
using Airbnb.DATA.models;
using Airbnb.Infrastructure.Abstract;
using AutoMapper;

namespace Airbnb.Application.Services.Implementation
{
    public class ReviewService : IReviewService
    {
        private readonly IGenericRepo<Review> _repo;
        private readonly IMapper _mapper;

        public ReviewService(IGenericRepo<Review> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewDTO>> GetAllAsync()
        {
            var reviews = _repo.GetTableNoTracking();
            return _mapper.Map<IEnumerable<ReviewDTO>>(reviews);
        }

        public async Task<ReviewDTO> GetByIdAsync(int id)
        {
            var review = await _repo.GetByIdAsync(id);
            return _mapper.Map<ReviewDTO>(review);
        }

        public async Task<ReviewDTO> CreateAsync(CreateReviewDTO dto)
        {
            var review = _mapper.Map<Review>(dto);
            var created = await _repo.AddAsync(review);
            return _mapper.Map<ReviewDTO>(created);
        }

        public async Task UpdateAsync(UpdateReviewDTO dto)
        {
            var review = await _repo.GetByIdAsync(dto.Id);
            if (review == null) return;

            review.Rating = dto.Rating;
            review.Comments = dto.Comments;
            await _repo.UpdateAsync(review);
        }

        public async Task DeleteAsync(int id)
        {
            var review = await _repo.GetByIdAsync(id);
            if (review != null)
                await _repo.DeleteAsync(review);
        }
    }
}
