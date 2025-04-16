using Airbnb.Application.DTOs.Property;
namespace Airbnb.Application.Services.Abstract
{
    public interface IPropertyService
    {
        public Task<IEnumerable<PropertyDTO>> GetPropertiesByHost(string hostId);
        public Task UpdatePropertyAsync(UpdatePropertyDTO propertyDto);
        Task CreatePropertyAsync(CreatePropertyDTO propertyDto);
        Task<IEnumerable<PropertyDTO>> GetAllAsync();
        Task<PropertyDTO> GetByIdAsync(int id);
        Task<PropertyDTO> CreateAsync(CreatePropertyDTO dto);
        Task<bool> UpdateAsync(UpdatePropertyDTO dto);
        Task<bool> DeleteAsync(int id, string userId);
    }
}
