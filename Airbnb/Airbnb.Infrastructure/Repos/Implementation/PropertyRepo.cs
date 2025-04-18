﻿using Airbnb.DATA.models;
using Airbnb.Infrastructure.Context;
using Airbnb.Infrastructure.Repos.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Airbnb.Infrastructure.Repos.Implementation
{
    public class PropertyWithStatusDto
    {
        public Property Property { get; set; }
        public bool IsAvailableToday { get; set; }
        public double? AverageRating { get; set; }
    }
    public class PropertyRepo : GenericRepo<Property>, IPropertyRepo
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PropertyRepo> _logger;

        public PropertyRepo(AppDbContext context, ILogger<PropertyRepo> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<IEnumerable<PropertyWithStatusDto>> GetHostPropertiesWithStatus(string hostId)
        {
            var today = DateTime.Today;

            return await _context.properties
                .Where(p => p.UserId == int.Parse(hostId))
                .Include(p => p.PropertyImages)
                .Include(p => p.Category)
                .Include(p => p.Availabilities)
                .Include(p => p.Bookings)
                .Include(p => p.Reviews)
                .Select(p => new PropertyWithStatusDto
                {
                    Property = p,
                    IsAvailableToday =
                                     !p.Bookings.Any(b => b.CheckInDte <= today && b.CheckOutDate >= today),
                    AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : (double?)null
                })
                .ToListAsync();
        }

        public async Task<int> GetTodaysBookedPropertiesCount(string hostId)
        {
            var today = DateTime.Today;

            return await _context.bookings
                .CountAsync(b => b.Property.UserId == int.Parse(hostId) &&
                               b.CheckInDte <= today &&
                               b.CheckOutDate >= today);
        }

        public async Task<double?> GetHostAverageRating(string hostId)
        {
            return await _context.properties
                .Where(p => p.UserId == int.Parse(hostId))
                .SelectMany(p => p.Reviews)
                .AverageAsync(r => (double?)r.Rating);
        }
        public async Task<bool> DeletePropertyOfUser(int propertyId, string userId)
        {
            try
            {

                if (!await IsOwnerAsync(propertyId, userId))
                {
                    _logger.LogWarning($"User {userId} attempted to delete property {propertyId} they don't own");
                    return false;
                }

                var property = await _context.properties
                    .Include(p => p.PropertyImages)
                    .FirstOrDefaultAsync(p => p.Id == propertyId);

                if (property == null)
                {
                    _logger.LogWarning($"Property {propertyId} not found for deletion");
                    return false;
                }


                if (property.PropertyImages.Any())
                {
                    _context.propertyImages.RemoveRange(property.PropertyImages);
                }

                _context.properties.Remove(property);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Property {propertyId} deleted by user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting property {propertyId} by user {userId}");
                return false;
            }
        }
        public async Task<bool> IsOwnerAsync(int propertyId, string userId)
        {
            return await _context.properties
                .AnyAsync(p => p.Id == propertyId && p.UserId == int.Parse(userId));
        }
        public async Task<IEnumerable<Property>> GetByIdWithInclude(string id)
        {
            return await _context.properties
            .Where(p => p.UserId == int.Parse(id))
            .Include(p => p.PropertyImages)
            .Include(p => p.Category)
            .Include(p => p.Availabilities)
            .ToListAsync();


        }

        public async Task<Property> GetOnePropertyWithInclude(string id)
        {
            var today = DateTime.Today;

            return await _context.properties
                .Include(p => p.PropertyImages)
                .Include(p => p.Category)
                .Include(p => p.Availabilities)
                .Include(p => p.Reviews)
                .Include(p => p.Bookings)
                .FirstOrDefaultAsync(p => p.Id == int.Parse(id));

        }
        public async Task<IEnumerable<Property>> GetAllWithIncludesAsync()
        {
            return await _context.properties
              .Include(p => p.PropertyImages)
              .Include(p => p.Category)
              .Include(p => p.Availabilities).ToListAsync();
        }
        public async Task RemoveAllAvailabilitiesAsync(int propertyId)
        {
            var availabilities = await _context.availabilities
                .Where(a => a.PropertyId == propertyId)
                .ToListAsync();

            _context.availabilities.RemoveRange(availabilities);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> IsPropertyAvailable(int propertyId, DateTime checkIn, DateTime checkOut)
        {
            return !await _context.bookings
                .AnyAsync(b => b.PropertyId == propertyId &&
                              b.CheckOutDate > checkIn &&
                              b.CheckInDte < checkOut);
        }
        public async Task<Property> GetByIdWithImagesAsync(int id)
        {
            return await _context.properties
                .Include(p => p.PropertyImages)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateAsync(Property property)
        {
            _context.properties.Update(property);
            await _context.SaveChangesAsync();
        }
    }
}
