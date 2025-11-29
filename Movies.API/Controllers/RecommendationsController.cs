using Microsoft.AspNetCore.Mvc;
using Movies.Application.Abstractions;
using Movies.Application.Models;

namespace Movies.API.Controllers
{
    [Route("api/recommendations")]
    [ApiController]
    public class RecommendationsController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;

        public RecommendationsController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieViewModel>>> GetRecommendations([FromQuery] int movieId, CancellationToken cancellationToken = default)
        {
            var recommendations = await _recommendationService.GetRecommendationsAsync(movieId, cancellationToken);
            return Ok(recommendations);
        }
    }
}