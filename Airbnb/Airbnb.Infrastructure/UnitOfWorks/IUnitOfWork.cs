using Airbnb.Infrastructure.Repos.Abstract;

namespace Airbnb.Infrastructure.UnitOfWorks
{
    public interface IUnitOfWork
    {
        public IBookingRepo BookingRepo { get; }
        public IPropertyRepo PropertyRepo { get; }
        public Task CommitAsync();
    }
}
