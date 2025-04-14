using Airbnb.Application.DTOs;
using Airbnb.Application.DTOs.PropertyCategory;
using Airbnb.Application.Services.Abstract;
using Airbnb.DATA.models;
using Airbnb.Infrastructure.Abstract;
using AutoMapper;

namespace Airbnb.Application.Services.Implementation
{
    public class PropertyCategoryService : IPropertyCategoryService
    {
        private readonly IGenericRepo<PropertyCategory> _repo;
        private readonly IMapper _mapper;

        public PropertyCategoryService(IGenericRepo<PropertyCategory> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PropertyCategoryDTO>> GetAllAsync()
        {
            var categories = _repo.GetTableNoTracking();
            return _mapper.Map<IEnumerable<PropertyCategoryDTO>>(categories);
        }

        public async Task<PropertyCategoryDTO> GetByIdAsync(int id)
        {
            var category = await _repo.GetByIdAsync(id);
            return _mapper.Map<PropertyCategoryDTO>(category);
        }

        public async Task<PropertyCategoryDTO> CreateAsync(CreatePropertyCategoryDTO dto)
        {
            var entity = _mapper.Map<PropertyCategory>(dto);
            var created = await _repo.AddAsync(entity);
            return _mapper.Map<PropertyCategoryDTO>(created);
        }

        public async Task UpdateAsync(UpdatePropertyCategoryDTO dto)
        {
            var entity = _mapper.Map<PropertyCategory>(dto);
            await _repo.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            await _repo.DeleteAsync(entity);
        }
    }
}
