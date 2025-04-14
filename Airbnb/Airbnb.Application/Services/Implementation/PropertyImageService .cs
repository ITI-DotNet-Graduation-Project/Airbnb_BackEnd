using Airbnb.Application.DTOs;
using Airbnb.Application.DTOs.PropertyImage;
using Airbnb.Application.Services.Abstract;
using Airbnb.DATA.models;
using Airbnb.Infrastructure.Abstract;
using AutoMapper;

namespace Airbnb.Application.Services.Implementation
{
    public class PropertyImageService : IPropertyImageService
    {
        private readonly IGenericRepo<PropertyImage> _repo;
        private readonly IMapper _mapper;

        public PropertyImageService(IGenericRepo<PropertyImage> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PropertyImageDTO>> GetAllAsync()
        {
            var images = _repo.GetTableNoTracking();
            return _mapper.Map<IEnumerable<PropertyImageDTO>>(images);
        }

        public async Task<PropertyImageDTO> GetByIdAsync(int id)
        {
            var image = await _repo.GetByIdAsync(id);
            return _mapper.Map<PropertyImageDTO>(image);
        }

        public async Task<PropertyImageDTO> CreateAsync(CreatePropertyImageDTO dto)
        {
            var entity = _mapper.Map<PropertyImage>(dto);
            var created = await _repo.AddAsync(entity);
            return _mapper.Map<PropertyImageDTO>(created);
        }

        public async Task UpdateAsync(UpdatePropertyImageDTO dto)
        {
            var entity = _mapper.Map<PropertyImage>(dto);
            await _repo.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            await _repo.DeleteAsync(entity);
        }
    }
}

