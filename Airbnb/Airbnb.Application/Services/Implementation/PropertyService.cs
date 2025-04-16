using Airbnb.Application.DTOs.Property;
using Airbnb.Application.Services.Abstract;
using Airbnb.DATA.models;
using Airbnb.Infrastructure.Repos.Abstract;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Airbnb.Application.Services.Implementation
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepo _propertyRepo;
        private readonly IMapper _mapper; // use AutoMapper or map manually
        private readonly IWebHostEnvironment _environment;

        public PropertyService(IPropertyRepo propertyRepo, IMapper mapper, IWebHostEnvironment environment)
        {
            _propertyRepo = propertyRepo;
            _mapper = mapper;
            _environment = environment;
        }
        public async Task<IEnumerable<PropertyDTO>> GetPropertiesByHost(string hostId)
        {
            var p = await _propertyRepo.GetByIdWithInclude(hostId);
            var res = _mapper.Map<IEnumerable<PropertyDTO>>(p);
            return res;

        }
        public async Task CreatePropertyAsync(CreatePropertyDTO propertyDto)
        {
            var property = new Property
            {
                Title = propertyDto.Title,
                Description = propertyDto.Description,
                Location = propertyDto.Location,
                Price = propertyDto.Price,
                Bedrooms = propertyDto.Bedrooms,
                Bathrooms = propertyDto.Bathrooms,
                CategoryId = int.Parse(propertyDto.PropertyType),
                Amenities = JsonConvert.SerializeObject(propertyDto.Amenities),
                PropertyImages = new List<PropertyImage>(),
                UserId = int.Parse(propertyDto.UserId)
            };


            foreach (var imageFile in propertyDto.Images)
            {
                var imageUrl = await SaveImageAsync(imageFile);
                property.PropertyImages.Add(new PropertyImage { ImageUrl = imageUrl });
            }
            await _propertyRepo.AddAsync(property);

        }
        public async Task UpdatePropertyAsync(UpdatePropertyDTO propertyDto)
        {

            var property = await _propertyRepo.GetByIdWithImagesAsync(propertyDto.Id);

            if (property == null || property.UserId != int.Parse(propertyDto.UserId))
                throw new KeyNotFoundException("Property not found or access denied");


            property.Title = propertyDto.Title;
            property.Description = propertyDto.Description;
            property.Location = propertyDto.Location;
            property.Price = propertyDto.Price;
            property.Bedrooms = propertyDto.Bedrooms;
            property.Bathrooms = propertyDto.Bathrooms;
            property.CategoryId = int.Parse(propertyDto.PropertyType);
            property.Amenities = propertyDto.Amenities;


            await ProcessImageUpdates(property, propertyDto);

            await _propertyRepo.UpdateAsync(property);
        }

        private async Task ProcessImageUpdates(Property property, UpdatePropertyDTO propertyDto)
        {

            if (propertyDto.DeletedImageIds != null && propertyDto.DeletedImageIds.Any())
            {
                var imagesToRemove = property.PropertyImages
                    .Where(img => propertyDto.DeletedImageIds.Contains(img.Id.ToString()))
                    .ToList();

                foreach (var image in imagesToRemove)
                {
                    DeleteImageFile(image.ImageUrl);
                    property.PropertyImages.Remove(image);
                }
            }


            if (propertyDto.NewImages != null && propertyDto.NewImages.Any())
            {
                foreach (var imageFile in propertyDto.NewImages)
                {
                    var imageUrl = await SaveImageAsync(imageFile);
                    property.PropertyImages.Add(new PropertyImage { ImageUrl = imageUrl });
                }
            }
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "properties");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return uniqueFileName;
        }

        private void DeleteImageFile(string imageUrl)
        {
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", imageUrl);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public async Task<IEnumerable<PropertyDTO>> GetAllAsync()
        {
            var properties = await _propertyRepo.GetTableNoTracking().ToListAsync();
            return _mapper.Map<IEnumerable<PropertyDTO>>(properties);
        }

        public async Task<PropertyDTO> GetByIdAsync(int id)
        {
            var property = await _propertyRepo.GetOnePropertyWithInclude(id.ToString());
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

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var property = await _propertyRepo.DeletePropertyOfUser(id, userId);
            if (property) return true;


            return false;
        }
    }

}
