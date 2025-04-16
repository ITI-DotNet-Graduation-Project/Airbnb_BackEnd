using Airbnb.Application.Services.Abstract;
using Airbnb.Infrastructure.Context;
using Airbnb.Infrastructure.Repos.Abstract;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Airbnb.Application.Services.Implementation
{
    public class PropertyImageService : IPropertyImageService
    {
        private readonly IPropertyImageRepo _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<PropertyImageService> _logger;
        private readonly AppDbContext _context;

        public PropertyImageService(AppDbContext context, IPropertyImageRepo repo, IMapper mapper, ILogger<PropertyImageService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
            _context = context;
        }
        public async Task<bool> DeleteImage(int propertyId, int imageId, string userId)
        {
            var res = await _repo.DeleteImageAsync(propertyId, imageId, userId);
            if (res) return true;
            return false;
        }



    }
}

