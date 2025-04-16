using Airbnb.Application.DTOs.Authentication;
using Airbnb.Application.ErrorHandler;
using Airbnb.Application.Errors;
using Airbnb.Application.Services.Abstract;
using Airbnb.DATA.models;
using Airbnb.DATA.models.Identity;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using OneOf;
using System.Security.Cryptography;
using System.Text;

namespace Airbnb.Application.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtProvider _jwtProvider;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly int _refreshTokenExpiryDays = 14;
        public AuthService(UserManager<User> userManager, IJwtProvider jwtProvider, SignInManager<User> signInManager, IEmailSender emailSender, IHttpContextAccessor httpContextAccessor, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _jwtProvider = jwtProvider;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;
            _roleManager = roleManager;
        }
        public async Task<OneOf<AuthResponse?, Error>> GetTokenAsync(string email, string password)
        {

            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return UserErrors.InvalidCredential;
            var result = await _signInManager.PasswordSignInAsync(user, password, false, true);
            if (result.Succeeded)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var (taken, expiresIn) = _jwtProvider.GenerateToken(user, userRoles);

                var refreshToken = GenerateRefreshToken();
                var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);
                user.RefreshTokens.Add(new RefreshToken
                {
                    Token = refreshToken,
                    ExpiredIn = refreshTokenExpiration
                });
                await _userManager.UpdateAsync(user);

                return new AuthResponse((user.Id).ToString(), user.Email, user.FirstName + " " + user.LastName, taken, expiresIn, refreshToken, refreshTokenExpiration, userRoles.FirstOrDefault());
            }

            var error =
                 result.IsLockedOut
                ? UserErrors.UserLockOut
                : result.IsNotAllowed
                ? UserErrors.EmailNotConfirmed
                : UserErrors.InvalidCredential;
            return error;
        }
        public async Task<OneOf<AuthResponse?, Error>> GetRefreshTokenAsync(string token, string refreshToken)
        {
            var userId = _jwtProvider.ValidateTaken(token);
            if (userId is null)
                return UserErrors.InvalidRefreshToken;

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return UserErrors.InvalidRefreshToken;

            if (user.LockoutEnd > DateTime.UtcNow)
                return UserErrors.UserLockOut;

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(u => u.Token == refreshToken);
            if (userRefreshToken is null)
                return UserErrors.InvalidRefreshToken;
            userRefreshToken.RevokedIn = DateTime.UtcNow;

            var userRoles = await _userManager.GetRolesAsync(user);
            var (newToken, expiresIn) = _jwtProvider.GenerateToken(user, userRoles);


            var newRefreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);
            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                ExpiredIn = refreshTokenExpiration
            });
            await _userManager.UpdateAsync(user);

            return new AuthResponse((user.Id).ToString(), user.Email, user.FirstName + " " + user.LastName, newToken, expiresIn, newRefreshToken, refreshTokenExpiration, userRoles.FirstOrDefault());

        }
        public async Task<OneOf<bool, Error>> RevokeRefreshTokenAsync(string token, string refreshToken)
        {
            var userId = _jwtProvider.ValidateTaken(token);
            if (userId is null)
                return UserErrors.InvalidRefreshToken_Token;

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return UserErrors.InvalidRefreshToken_Token;

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(u => u.Token == refreshToken);
            if (userRefreshToken is null)
                return UserErrors.InvalidRefreshToken_Token;
            userRefreshToken.RevokedIn = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            return true;
        }
        public async Task<OneOf<string?, Error>> RegisterAsync(_RegisterRequest request)
        {
            var email = await _userManager.FindByEmailAsync(request.Email);
            if (email is not null)
                return UserErrors.DuplicateUser;
            var user = new User()
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(request.Role))
                {
                    var roleError = result.Errors.First();
                    return new Error(roleError.Code, roleError.Description, StatusCodes.Status400BadRequest);
                }

                var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
                if (!roleResult.Succeeded)
                {
                    var roleError = roleResult.Errors.First();
                    return new Error(roleError.Code, roleError.Description, StatusCodes.Status400BadRequest);
                }
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                await SendEmailConfirmation(user, code);

                return UserErrors.SendEmail;

            }
            var error = result.Errors.First();
            return new Error(error.Code, error.Description, StatusCodes.Status400BadRequest);
        }
        public async Task<OneOf<AuthResponse?, Error>> ConfirmEmailAsync(ConfirmEmailRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user is null)
                return UserErrors.InvalidCode;
            if (user.EmailConfirmed)
                return UserErrors.DuplicateConfirmed;

            var code = request.Code;
            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            }
            catch (Exception)
            {
                return UserErrors.InvalidCode;
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Airbnb.Application.Const.AppRoles.Guest);
                return UserErrors.Success;
            }

            var error = result.Errors.First();
            return new Error(error.Code, error.Description, StatusCodes.Status400BadRequest);
        }
        public async Task<OneOf<string, Error>> ResendConfirmationEmailAsync(_ResendConfirmationEmailRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return UserErrors.ResendEmail;
            if (user.EmailConfirmed)
                return UserErrors.DuplicateConfirmed;

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            await SendEmailConfirmation(user, code);
            return UserErrors.ResendEmail;

        }
        public async Task<OneOf<string, Error>> SendResetPasswordAsync(ForgetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return UserErrors.ResetPassword;
            if (!user.EmailConfirmed)
                return UserErrors.EmailNotConfirmed;
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            await SendResetPassword(user, code);
            return UserErrors.ResetPassword;

        }
        public async Task<OneOf<string, Error>> ResetPasswordAsync(_ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null || !user.EmailConfirmed)
                return UserErrors.InvalidCode;
            IdentityResult result;
            try
            {
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
                result = await _userManager.ResetPasswordAsync(user, code, request.newPassword);
            }
            catch (FormatException)
            {
                result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
            }
            if (result.Succeeded)
                return UserErrors.SuccessResetPassword;

            var error = result.Errors.First();
            return new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized);
        }

        private static string GenerateRefreshToken() =>
            Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        private async Task SendEmailConfirmation(User user, string code)
        {
            var origin = "http://localhost:4200";
            var TempPath = $"{Directory.GetCurrentDirectory()}/Templates/EmailConfirmation.html";
            StreamReader streamReader = new StreamReader(TempPath);
            var body = streamReader.ReadToEnd();
            streamReader.Close();
            body = body
                .Replace("[name]", $"{user.FirstName + " " + user.LastName} ")
                .Replace("[action_url]", $"{origin}/confirm-email?userId={user.Id}&code={code}");

            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Confirm your email", body));
            await Task.CompletedTask;
        }
        private async Task SendResetPassword(User user, string code)
        {
            var origin = "http://localhost:4200";
            var TempPath = $"{Directory.GetCurrentDirectory()}/Templates/ForgetPassword.html";
            StreamReader streamReader = new StreamReader(TempPath);
            var body = streamReader.ReadToEnd();
            streamReader.Close();
            body = body
                .Replace("{{name}}", $"{user.FirstName + " " + user.LastName} ")
                .Replace("{{action_url}}", $"{origin}/forget-password?email={user.Email}&code={code}");

            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Reset your password", body));
            await Task.CompletedTask;
        }
    }
}
