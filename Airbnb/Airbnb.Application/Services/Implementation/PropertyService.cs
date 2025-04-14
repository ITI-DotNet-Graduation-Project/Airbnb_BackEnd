using Airbnb.Application.DTOs.Property;
using Airbnb.Application.Services.Abstract;
using Airbnb.DATA.models;
using Airbnb.DATA;
using Airbnb.Infrastructure.Abstract;

using Microsoft.EntityFrameworkCore.Metadata;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.Application.Services.Implementation
{
    public class PropertyService : IPropertyService
    {
        private readonly IGenericRepo<Property> _propertyRepo;
        private readonly IMapper _mapper; // use AutoMapper or map manually

        public PropertyService(IGenericRepo<Property> propertyRepo, IMapper mapper)
        {
            _propertyRepo = propertyRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PropertyDTO>> GetAllAsync()
        {
            var properties = await _propertyRepo.GetTableNoTracking().ToListAsync();
            return _mapper.Map<IEnumerable<PropertyDTO>>(properties);
        }

        public async Task<PropertyDTO> GetByIdAsync(int id)
        {
            var property = await _propertyRepo.GetByIdAsync(id);
            return _mapper.Map<PropertyDTO>(property);
        }

        public async Task<PropertyDTO> CreateAsync(CreatePropertyDTO dto)
        {
            var property = _mapper.Map<Property>(dto);
            await _propertyRepo.AddAsync(property);
            return _mapper.Map<PropertyDTO>(property);
        }

        public async Task<bool> UpdateAsync(UpdatePropertyDTO dto)
        {
            var property = await _propertyRepo.GetByIdAsync(dto.Id);
            if (property == null) return false;

            _mapper.Map(dto, property);
            await _propertyRepo.UpdateAsync(property);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var property = await _propertyRepo.GetByIdAsync(id);
            if (property == null) return false;

            await _propertyRepo.DeleteAsync(property);
            return true;
        }
    }

}
