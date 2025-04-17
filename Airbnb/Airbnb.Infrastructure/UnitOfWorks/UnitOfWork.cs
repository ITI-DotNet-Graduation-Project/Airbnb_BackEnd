using Airbnb.Infrastructure.Context;
using Airbnb.Infrastructure.Repos.Abstract;

namespace Airbnb.Infrastructure.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private IBookingRepo _bookingRepo;
        private IPropertyRepo _propertyRepo;
        private AppDbContext _context;

        public UnitOfWork(IBookingRepo bookingRepo, IPropertyRepo propertyRepo, AppDbContext context)
        {
            _bookingRepo = bookingRepo;
            _propertyRepo = propertyRepo;
            _context = context;
        }
        public IBookingRepo BookingRepo => _bookingRepo;

        public IPropertyRepo PropertyRepo => _propertyRepo;

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
