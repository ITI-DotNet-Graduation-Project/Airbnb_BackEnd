using Airbnb.Application.DTOs.User;
using Airbnb.DATA.models;
namespace Airbnb.Application.Services.Abstract
{
    public interface IUserService
    {
        public Task<User> UpdateUserProfileAsync(int userId, UpdateProfileDto updateProfileDto);
        Task<User> GetUserByIdAsync(int userId);
    }
}
