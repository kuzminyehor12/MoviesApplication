using Microsoft.AspNetCore.Mvc;
using Movies.Application.Abstractions;
using Movies.Application.Models;
using Movies.Application.Requests;
using Movies.Core.Entities;

namespace Movies.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IFuzzySearchService _fuzzySearchService;

        public SearchController(IFuzzySearchService fuzzySearchService)
        {
            _fuzzySearchService = fuzzySearchService;
        }
        
        [HttpGet("fuzzy")]
        public async Task<ActionResult<PaginatedResult<MovieViewModel>>> FuzzySearch(
            [FromQuery] string query, 
            [FromQuery] int pageNumber = 1, 
            CancellationToken cancellationToken = default)
        {
            var request = new FuzzySearchRequest
            {
                Query = query,
                PageNumber = pageNumber
            };
            
            var result = await _fuzzySearchService.SearchAsync(request,  cancellationToken);
            return Ok(result);
        }
    }
}
