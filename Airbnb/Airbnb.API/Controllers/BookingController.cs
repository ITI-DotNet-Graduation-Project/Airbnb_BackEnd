using Airbnb.Application.DTOs.Booking;
using Airbnb.Application.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Airbnb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(
            IBookingService bookingService,
            ILogger<BookingsController> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }


        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingDTO bookingDto)
        {
            try
            {

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);


                _logger.LogInformation($"User {userId} attempting to book property {bookingDto.PropertyId}");

                var booking = await _bookingService.CreateBookingAsync(bookingDto, userId);


                return CreatedAtAction(
                    nameof(GetBookingById),
                    new { id = booking.Id },
                    booking);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid booking request");
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Booking conflict");
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                return StatusCode(500, "An error occurred while processing your booking");
            }
        }
        [HttpGet("{propertyId}/booked-dates")]
        public async Task<ActionResult<IEnumerable<string>>> GetBookedDates(int propertyId)
        {
            try
            {
                var bookedDates = await _bookingService.GetBookedDatesForPropertyAsync(propertyId);
                return Ok(bookedDates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving booked dates for property {PropertyId}", propertyId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var booking = await _bookingService.GetBookingByIdAsync(id, userId);

                if (booking == null)
                {
                    return NotFound();
                }

                return Ok(booking);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting booking {id}");
                return StatusCode(500, "An error occurred while retrieving booking");
            }
        }


        [HttpGet("user")]
        public async Task<IActionResult> GetUserBookings()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var bookings = await _bookingService.GetUserBookingsAsync(userId);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user bookings");
                return StatusCode(500, "An error occurred while retrieving bookings");
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                await _bookingService.CancelBookingAsync(id, userId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error canceling booking {id}");
                return StatusCode(500, "An error occurred while canceling booking");
            }
        }
    }
}