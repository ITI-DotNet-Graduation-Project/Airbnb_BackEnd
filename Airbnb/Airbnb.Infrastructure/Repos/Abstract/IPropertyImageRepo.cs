namespace Airbnb.Infrastructure.Repos.Abstract
{
    public interface IPropertyImageRepo
    {
        Task<bool> DeleteImageAsync(int propertyId, int imageId, string userId);
        Task<bool> IsImageOwnerAsync(int propertyId, int imageId, string userId);
    }
}
