namespace Airbnb.Application.Services.Abstract
{
    public interface IPropertyImageService
    {
        Task<bool> DeleteImage(int propertyId, int imageId, string userId);
        //Task<bool> IsImageOwnerAsync(int propertyId, int imageId, string userId);
        //Task<IEnumerable<PropertyImageDTO>> GetAllAsync();
        //Task<PropertyImageDTO> GetByIdAsync(int id);
        //Task<PropertyImageDTO> CreateAsync(CreatePropertyImageDTO dto);
        //Task UpdateAsync(UpdatePropertyImageDTO dto);
        //Task DeleteAsync(int id);
    }
}
