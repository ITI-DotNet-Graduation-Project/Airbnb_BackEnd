using Airbnb.Application.DTOs.Property;
using Airbnb.Application.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;


namespace Airbnb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;
        private readonly ILogger<PropertyController> _logger;

        public PropertyController(IPropertyService propertyService, ILogger<PropertyController> logger)
        {
            _propertyService = propertyService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _propertyService.GetAllAsync();
            return Ok(result);
        }
        [HttpGet("get-one-property/{id}")]
        [Authorize]
        public async Task<IActionResult> GetPropertyToView(int id)
        {
            var res = await _propertyService.GetByIdAsync(id);

            if (res != null)
                return Ok(res);

            return NotFound();
        }

        [HttpGet("host-properties")]
        [Authorize]
        public async Task<IActionResult> GetHostProperties()
        {
            Console.WriteLine($"User identity type: {User.Identity?.GetType().Name}");
            Console.WriteLine($"Is authenticated: {User.Identity?.IsAuthenticated}");

            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return Unauthorized("User not properly authenticated");
            }

            var userId = User.FindFirstValue("sub") ??
                 User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                 User.Identity?.Name;
            if (string.IsNullOrEmpty(userId))
            {
                var claims = User.Claims.Select(c => $"{c.Type}={c.Value}").ToList();
                Console.WriteLine($"Available claims: {string.Join(", ", claims)}");
                return BadRequest("User ID claim (sub) not found in token");
            }
            _logger.LogError(userId);
            var properties = await _propertyService.GetPropertiesByHost(userId);
            return Ok(properties);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdatePropertyDTO propertyDto)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        title = "Validation failed",
                        status = 400,
                        errors = ModelState.ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        )
                    });
                }

                if (id != propertyDto.Id)
                {
                    return BadRequest("ID mismatch between route and body");
                }

                if (string.IsNullOrEmpty(propertyDto.UserId))
                {
                    propertyDto.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                }
                _logger.LogError("Received Availabilities JSON: " + propertyDto.AvailabilitiesJson);
                _logger.LogInformation($"Updted Received MaxGuests: {propertyDto.MaxGuest}");
                if (!string.IsNullOrEmpty(propertyDto.AvailabilitiesJson))
                {
                    try
                    {
                        propertyDto.Availabilities = JsonConvert.DeserializeObject<List<AvailabilityDto>>(propertyDto.AvailabilitiesJson);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error parsing Availabilities JSON", ex);
                        return BadRequest("Invalid Availabilities format");
                    }
                }

                if (propertyDto.Availabilities == null || propertyDto.Availabilities.Count == 0)
                {
                    return BadRequest("Availabilities are required");
                }
                await _propertyService.UpdatePropertyAsync(propertyDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    title = "Server error",
                    status = 500,
                    detail = ex.Message
                });
            }
        }
        [HttpPost("create-property")]
        public async Task<IActionResult> Create([FromForm] CreatePropertyDTO dto)
        {
            _logger.LogError("Received Availabilities JSON: " + dto.AvailabilitiesJson);
            _logger.LogInformation($"Received MaxGuests: {dto.MaxGuest}");
            if (!string.IsNullOrEmpty(dto.AvailabilitiesJson))
            {
                try
                {
                    dto.Availabilities = JsonConvert.DeserializeObject<List<AvailabilityDto>>(dto.AvailabilitiesJson);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error parsing Availabilities JSON", ex);
                    return BadRequest("Invalid Availabilities format");
                }
            }

            if (dto.Availabilities == null || dto.Availabilities.Count == 0)
            {
                return BadRequest("Availabilities are required");
            }
            _logger.LogError(dto.ToString());
            if (string.IsNullOrEmpty(dto.UserId))
            {
                return BadRequest("User ID is required");
            }

            try
            {
                await _propertyService.CreatePropertyAsync(dto);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete("delete-property/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            Console.WriteLine($"User identity type: {User.Identity?.GetType().Name}");
            Console.WriteLine($"Is authenticated: {User.Identity?.IsAuthenticated}");

            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return Unauthorized("User not properly authenticated");
            }

            var userId = User.FindFirstValue("sub") ??
                 User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                 User.Identity?.Name;
            if (string.IsNullOrEmpty(userId))
            {
                var claims = User.Claims.Select(c => $"{c.Type}={c.Value}").ToList();
                Console.WriteLine($"Available claims: {string.Join(", ", claims)}");
                return BadRequest("User ID claim (sub) not found in token");
            }
            _logger.LogError(userId);
            var result = await _propertyService.DeleteAsync(id, userId);
            if (!result) return NotFound();

            return NoContent();
        }
    }

}
