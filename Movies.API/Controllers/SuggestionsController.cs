using Microsoft.AspNetCore.Mvc;
using Movies.Application.Abstractions;
using Movies.Application.Models;

namespace Movies.API.Controllers
{
    [Route("api/suggestions")]
    [ApiController]
    public class SuggestionsController : ControllerBase
    {
        private readonly ISuggestionService _suggestionService;

        public SuggestionsController(ISuggestionService suggestionService)
        {
            _suggestionService = suggestionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Suggestion>>> GetSuggestions([FromQuery] string query, CancellationToken cancellationToken = default)
        {
            var suggestions = await _suggestionService.GetSuggestionsAsync(query, cancellationToken);
            return Ok(suggestions);
        }
    }
}
