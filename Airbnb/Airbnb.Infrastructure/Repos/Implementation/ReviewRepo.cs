using Airbnb.DATA.models;
using Airbnb.Infrastructure.Context;
using Airbnb.Infrastructure.Repos.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.Infrastructure.Repos.Implementation
{
    public class ReviewRepo : GenericRepo<Review>, IReviewRepo
    {
        private readonly AppDbContext _context;

        public ReviewRepo(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetReviewsForProperty(int propertyId)
        {
            return await _context.reviews
           .Include(r => r.User)
           .Include(r => r.Property)
           .Where(r => r.PropertyId == propertyId)
           .OrderByDescending(r => r.ReviewDate)
           .ToListAsync();
        }

        public async Task<double> GetAverageRating(int propertyId)
        {
            return await _context.reviews
                .Where(r => r.PropertyId == propertyId)
                .AverageAsync(r => r.Rating ?? 0);
        }
    }
}
