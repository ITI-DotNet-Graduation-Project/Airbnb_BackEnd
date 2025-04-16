using Airbnb.Application.DTOs.User;
using Airbnb.Application.Services.Abstract;
using Airbnb.DATA.models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.Application.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<User> _userManager;

        public UserService(
            IWebHostEnvironment environment,
            UserManager<User> userManager)
        {
            _environment = environment;
            _userManager = userManager;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<User> UpdateUserProfileAsync(int userId, UpdateProfileDto updateProfileDto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new Exception("User not found");
            }

            user.FirstName = updateProfileDto.FirstName;
            user.LastName = updateProfileDto.LastName;
            user.Email = updateProfileDto.Email;
            user.UserName = updateProfileDto.Email;


            if (updateProfileDto.ProfileImage != null)
            {
                user.ImageUrl = await SaveProfileImageAsync(updateProfileDto.ProfileImage);
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return user;
        }

        private async Task<string> SaveProfileImageAsync(IFormFile imageFile)
        {

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }


            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);


            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return $"/uploads/{uniqueFileName}";
        }

        public async Task<string> GetUserFullNameAsync(int userId)
        {
            var user = await GetUserByIdAsync(userId);
            return $"{user.FirstName} {user.LastName}";
        }
    }
}