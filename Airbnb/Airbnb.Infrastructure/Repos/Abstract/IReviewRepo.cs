using Airbnb.DATA.models;
using Airbnb.Infrastructure.Abstract;

namespace Airbnb.Infrastructure.Repos.Abstract
{
    public interface IReviewRepo : IGenericRepo<Review>
    {
        Task<IEnumerable<Review>> GetReviewsForProperty(int propertyId);
        Task<double> GetAverageRating(int propertyId);
    }
}
