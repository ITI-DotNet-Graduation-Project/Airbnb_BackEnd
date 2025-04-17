using Airbnb.Application.DTOs.Property;
using Airbnb.Application.Properties;
using Airbnb.Application.Services.Abstract;
using Airbnb.DATA.models;
using Airbnb.Infrastructure.Repos.Abstract;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Airbnb.Application.Services.Implementation
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepo _propertyRepo;
        private readonly IMapper _mapper; // use AutoMapper or map manually
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<PropertyService> _logger;

        public PropertyService(IPropertyRepo propertyRepo, IMapper mapper, IWebHostEnvironment environment, ILogger<PropertyService> logger)
        {
            _propertyRepo = propertyRepo;
            _mapper = mapper;
            _environment = environment;
            _logger = logger;
        }

        public async Task<HostPropertiesResponse> GetPropertiesByHost(string hostId)
        {
            var propertiesWithStatus = await _propertyRepo.GetHostPropertiesWithStatus(hostId);
            var bookedTodayCount = await _propertyRepo.GetTodaysBookedPropertiesCount(hostId);
            var overallAverageRating = await _propertyRepo.GetHostAverageRating(hostId);

            var propertyDtos = _mapper.Map<IEnumerable<PropertyDTO>>(propertiesWithStatus.Select(x => x.Property));

            return new HostPropertiesResponse
            {
                Properties = propertyDtos,
                TotalProperties = propertiesWithStatus.Count(),
                AvailableTodayCount = propertiesWithStatus.Count(p => p.IsAvailableToday),
                BookedTodayCount = bookedTodayCount,
                OverallAverageRating = overallAverageRating
            };
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

                Maxgeusts = propertyDto.MaxGuest,
                PropertyImages = new List<PropertyImage>(),
                UserId = int.Parse(propertyDto.UserId),

            };

            foreach (var imageFile in propertyDto.Images)
            {
                var imageUrl = await SaveImageAsync(imageFile);
                property.PropertyImages.Add(new PropertyImage { ImageUrl = imageUrl });
            }
            _logger.LogError($"Availabilities Count: {propertyDto.Availabilities.Count}");

            if (propertyDto.Availabilities != null && propertyDto.Availabilities.Count > 0)
            {
                property.Availabilities = propertyDto.Availabilities?.Select(a => new Availability
                {
                    StartDate = a.startDate,
                    EndDate = a.endDate,
                    IsBooked = a.isBooked,
                }).ToList() ?? new List<Availability>();

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
            property.Maxgeusts = propertyDto.MaxGuest;


            await _propertyRepo.RemoveAllAvailabilitiesAsync(property.Id);


            if (!string.IsNullOrEmpty(propertyDto.AvailabilitiesJson))
            {
                var availabilities = JsonConvert.DeserializeObject<List<AvailabilityDto>>(propertyDto.AvailabilitiesJson);

                foreach (var availabilityDto in availabilities)
                {



                    if (availabilityDto.endDate <= availabilityDto.startDate)
                    {
                        throw new ArgumentException("End date must be after start date");
                    }

                    property.Availabilities.Add(new Availability
                    {
                        StartDate = availabilityDto.startDate,
                        EndDate = availabilityDto.endDate,
                        IsBooked = availabilityDto.isBooked,
                        PropertyId = property.Id
                    });
                }
            }

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
            var properties = await _propertyRepo.GetAllWithIncludesAsync();
            return _mapper.Map<IEnumerable<PropertyDTO>>(properties);
        }

        public async Task<PropertyDTO> GetByIdAsync(int id)
        {
            var today = DateTime.Today;
            var property = await _propertyRepo.GetOnePropertyWithInclude(id.ToString());

            if (property == null)
                return null;

            var propertyDto = _mapper.Map<PropertyDTO>(property);


            propertyDto.IsBookedToday = property.Bookings.Any(b =>
                b.CheckInDte <= today &&
                b.CheckOutDate >= today);



            return propertyDto;
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
