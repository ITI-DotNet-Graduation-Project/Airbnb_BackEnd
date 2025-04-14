using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Airbnb.Application.DTOs.PropertyImage;
using Airbnb.Application.Errors;
using Airbnb.DATA;

using OneOf;
namespace Airbnb.Application.Services.Abstract
{
    public interface IPropertyImageService
    {
        Task<IEnumerable<PropertyImageDTO>> GetAllAsync();
        Task<PropertyImageDTO> GetByIdAsync(int id);
        Task<PropertyImageDTO> CreateAsync(CreatePropertyImageDTO dto);
        Task UpdateAsync(UpdatePropertyImageDTO dto);
        Task DeleteAsync(int id);
    }
}
