using System.Security.Claims;
using Backend.DTOs.Recommendation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationController : ControllerBase
    {
        private readonly RecommendationService _recommendationService;

        public RecommendationController(RecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<RecommendationResponseDto>> GetRecommendation()
        {
            var tokenUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(tokenUserId)) return Unauthorized();

            try
            {
                var result = await _recommendationService.GetRecommendationAsync(tokenUserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Kunde inte hämta rekommendationen", details = ex.Message });
            }
        }
    }
}
