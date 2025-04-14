using Airbnb.Application.DTOs;
using Airbnb.Application.DTOs.PropertyCategory;

namespace Airbnb.Application.Services.Abstract
{
    public interface IPropertyCategoryService
    {
        Task<IEnumerable<PropertyCategoryDTO>> GetAllAsync();
        Task<PropertyCategoryDTO> GetByIdAsync(int id);
        Task<PropertyCategoryDTO> CreateAsync(CreatePropertyCategoryDTO dto);
        Task UpdateAsync(UpdatePropertyCategoryDTO dto);
        Task DeleteAsync(int id);
    }
}

