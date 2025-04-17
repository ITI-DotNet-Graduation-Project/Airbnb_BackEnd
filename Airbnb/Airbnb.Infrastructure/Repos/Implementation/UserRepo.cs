using Airbnb.DATA.models;
using Airbnb.Infrastructure.Context;
using Airbnb.Infrastructure.Repos.Abstract;

namespace Airbnb.Infrastructure.Repos.Implementation
{
    public class UserRepo : GenericRepo<User>, IUserRepo
    {
        private readonly AppDbContext _context;

        public UserRepo(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
