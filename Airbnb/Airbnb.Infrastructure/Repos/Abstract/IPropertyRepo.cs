using Airbnb.DATA.models;
using Airbnb.Infrastructure.Abstract;

namespace Airbnb.Infrastructure.Repos.Abstract
{
    public interface IPropertyRepo : IGenericRepo<Property>
    {
        public Task<IEnumerable<Property>> GetByIdWithInclude(string id);
        public Task<Property> GetOnePropertyWithInclude(string id);
        public Task<bool> DeletePropertyOfUser(int propertyId, string userId);
        Task<Property> GetByIdWithImagesAsync(int id);
        Task UpdateAsync(Property property);
        Task<IEnumerable<Property>> GetAllWithIncludesAsync();
        Task<bool> IsPropertyAvailable(int propertyId, DateTime checkIn, DateTime checkOut);

    }
}
