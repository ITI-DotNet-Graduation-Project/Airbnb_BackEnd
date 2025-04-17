
using Airbnb.Application.DTOs.Authentication;
using Airbnb.Application.Services.Abstract;
using Airbnb.DATA.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MyEductaionManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auhService;
        private readonly UserManager<User> _userService;

        public AuthController(IAuthService authService, UserManager<User> userManager)
        {
            _auhService = authService;
            _userService = userManager;
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] _LoginRequest LoginRequest)
        {
            var authResult = await _auhService.GetTokenAsync(LoginRequest.email, LoginRequest.password);
            return authResult.Match(
               res => Ok(res),
                error => Problem(statusCode: error.StatusCode,
                title: error.Code, detail: error.Description)
            );
        }



        [DisableRateLimiting]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] _RegisterRequest request)
        {
            var Result = await _auhService.RegisterAsync(request);
            return Result.Match(
                Result => Ok(Result),
                error => Problem(statusCode: error.StatusCode,
                title: error.Code, detail: error.Description)
            );
        }

        [HttpGet("confirmation-email")]
        public async Task<IActionResult> ConfirmEmailAsync([FromQuery] ConfirmEmailRequest request)
        {
            var result = await _auhService.ConfirmEmailAsync(request);
            return result.Match(
                result => Ok(result),
                error => Problem(statusCode: error.StatusCode,
                title: error.Code, detail: error.Description)
            );
        }
        [HttpPost("resend-confirmation-email")]
        public async Task<IActionResult> ResendConfirmationEmailAsync([FromBody] _ResendConfirmationEmailRequest request, CancellationToken cancellationToken)
        {
            var result = await _auhService.ResendConfirmationEmailAsync(request);
            return result.Match(
                result => Ok(result),
                error => Problem(statusCode: error.StatusCode,
                title: error.Code, detail: error.Description)
            );
        }
        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request)
        {
            var result = await _auhService.SendResetPasswordAsync(request);
            return result.Match(
                result => Ok(result),
                error => Problem(statusCode: error.StatusCode,
                title: error.Code, detail: error.Description)
            );
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] _ResetPasswordRequest request)
        {
            var result = await _auhService.ResetPasswordAsync(request);
            return result.Match(
                result => Ok(result),
                error => Problem(statusCode: error.StatusCode,
                title: error.Code, detail: error.Description)
            );
        }



    }
}
