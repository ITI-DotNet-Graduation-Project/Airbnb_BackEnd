using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Airbnb.Application.DTOs.Property;
using Airbnb.Application.Errors;
using Airbnb.DATA;

using OneOf;
namespace Airbnb.Application.Services.Abstract
{
    public interface IPropertyService
    {
        Task<IEnumerable<PropertyDTO>> GetAllAsync();
        Task<PropertyDTO> GetByIdAsync(int id);
        Task<PropertyDTO> CreateAsync(CreatePropertyDTO dto);
        Task<bool> UpdateAsync(UpdatePropertyDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
