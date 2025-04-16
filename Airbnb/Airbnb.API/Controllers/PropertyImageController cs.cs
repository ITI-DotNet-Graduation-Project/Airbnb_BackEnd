using Airbnb.Application.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Airbnb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyImageController : ControllerBase
    {
        private readonly IPropertyImageService _service;
        private readonly ILogger<PropertyImageController> _logger;

        public PropertyImageController(IPropertyImageService service, ILogger<PropertyImageController> logger)
        {
            _service = service;
            _logger = logger;
        }
        [HttpDelete("deleteImageFromProperty/{propertyId}/{ImageId}")]
        [Authorize]
        public async Task<IActionResult> DeleteImageFromProperty(int propertyId, int ImageId)
        {
            Console.WriteLine($"User identity type: {User.Identity?.GetType().Name}");
            Console.WriteLine($"Is authenticated: {User.Identity?.IsAuthenticated}");

            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return Unauthorized("User not properly authenticated");
            }

            var user = User.FindFirstValue("sub") ??
                 User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                 User.Identity?.Name;
            if (string.IsNullOrEmpty(user))
            {
                var claims = User.Claims.Select(c => $"{c.Type}={c.Value}").ToList();
                Console.WriteLine($"Available claims: {string.Join(", ", claims)}");
                return BadRequest("User ID claim (sub) not found in token");
            }
            _logger.LogError(user);
            var res = await _service.DeleteImage(propertyId, ImageId, user);
            if (res) return Ok();
            return BadRequest();
        }

    }
}
