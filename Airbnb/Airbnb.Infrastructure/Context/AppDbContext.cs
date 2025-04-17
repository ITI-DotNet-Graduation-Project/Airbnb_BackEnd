using Airbnb.DATA.models;
using Airbnb.DATA.models.Identity;
using Airbnb.Infrastructure.CategorySeeding;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Airbnb.Infrastructure.Context
{
    public class AppDbContext : IdentityDbContext<User, Role, int>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            builder.Entity<PropertyCategory>().HasData(PropertyCategorySeeder.GetSeedData());
            builder.Entity<Property>()
            .HasMany(p => p.Availabilities)
            .WithOne(a => a.Property)
            .HasForeignKey(a => a.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);
        }
        public DbSet<Availability> availabilities { get; set; }
        public DbSet<Booking> bookings { get; set; }
        public DbSet<Availability> availability { get; set; }
        public DbSet<Property> properties { get; set; }
        public DbSet<PropertyCategory> propertyCategories { get; set; }
        public DbSet<PropertyImage> propertyImages { get; set; }
        public DbSet<Review> reviews { get; set; }
        public DbSet<User> users { get; set; }



    }
}
