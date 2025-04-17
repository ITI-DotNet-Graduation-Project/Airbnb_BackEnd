using Airbnb.Application.DTOs.Review;
using Airbnb.Application.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Airbnb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("property/{propertyId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetReviewsForProperty(int propertyId)
        {
            var reviews = await _reviewService.GetReviewsForProperty(propertyId);
            return Ok(reviews);
        }

        [HttpPost]
        public async Task<ActionResult<ReviewDTO>> AddReview([FromBody] CreateReviewDTO reviewDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            try
            {
                var review = await _reviewService.AddReview(reviewDto, userId);
                return CreatedAtAction(nameof(GetReviewsForProperty), new { propertyId = review.PropertyId }, review);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("property/{propertyId}/average-rating")]
        [AllowAnonymous]
        public async Task<ActionResult<double>> GetAverageRating(int propertyId)
        {
            var averageRating = await _reviewService.GetAverageRating(propertyId);
            return Ok(averageRating);
        }
    }
}
