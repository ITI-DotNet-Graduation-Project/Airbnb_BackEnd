using Airbnb.Infrastructure.Context;
using Airbnb.Infrastructure.Repos.Abstract;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace Airbnb.Infrastructure.Repos.Implementation
{
    public class PropertyImageRepo : IPropertyImageRepo
    {
        private readonly AppDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly ILogger<PropertyImageRepo> _logger;

        public PropertyImageRepo(
            AppDbContext context,
            IHostingEnvironment env,
            ILogger<PropertyImageRepo> logger)
        {
            _context = context;
            _env = env as IHostingEnvironment ?? throw new ArgumentNullException(nameof(env));
            _logger = logger;
        }

        public async Task<bool> DeleteImageAsync(int propertyId, int imageId, string userId)
        {
            if (!int.TryParse(userId, out int userIdInt))
            {
                _logger.LogWarning("Invalid user ID format");
                return false;
            }

            var image = await _context.propertyImages
                .FirstOrDefaultAsync(i => i.Id == imageId && i.PropertyId == propertyId);

            if (image == null)
            {
                _logger.LogWarning($"Image {imageId} not found for property {propertyId}");
                return false;
            }

            if (!await IsImageOwnerAsync(propertyId, imageId, userIdInt.ToString()))
            {
                _logger.LogWarning($"User {userId} unauthorized to delete image {imageId}");
                return false;
            }

            try
            {

                var relativePath = image.ImageUrl.Substring(8);
                var filePath = Path.Combine(_env.WebRootPath, "uploads", relativePath);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }


                _context.propertyImages.Remove(image);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting image {imageId} for property {propertyId}");
                return false;
            }
        }

        public async Task<bool> IsImageOwnerAsync(int propertyId, int imageId, string userId)
        {
            return await _context.propertyImages
                .Include(i => i.Property)
                .AnyAsync(i => i.Id == imageId &&
                             i.PropertyId == propertyId &&
                             i.Property.UserId == int.Parse(userId));
        }
    }
}
