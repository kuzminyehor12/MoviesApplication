using Microsoft.AspNetCore.Mvc;
using Movies.Application.Abstractions;
using Movies.Application.Models;
using Movies.Application.Requests;

namespace Movies.API.Controllers
{
    [Route("api/search")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IFuzzySearchService _fuzzySearchService;
        private readonly ILexicalSearchService _lexicalSearchService;
        private readonly ISemanticSearchService _semanticSearchService;

        public SearchController(
            IFuzzySearchService fuzzySearchService, 
            ILexicalSearchService lexicalSearchService, 
            ISemanticSearchService semanticSearchService)
        {
            _fuzzySearchService = fuzzySearchService;
            _lexicalSearchService = lexicalSearchService;
            _semanticSearchService = semanticSearchService;
        }
        
        [HttpGet("fuzzy")]
        public async Task<ActionResult<PaginatedResult<MovieViewModel>>> FuzzySearch(
            [FromQuery] string query = "", 
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
        
        [HttpGet("lexical")]
        public async Task<ActionResult<PaginatedResult<MovieViewModel>>> LexicalSearch(
            [FromQuery] string query, 
            [FromQuery] int pageNumber = 1, 
            CancellationToken cancellationToken = default)
        {
            var request = new LexicalSearchRequest
            {
                Query = query,
                PageNumber = pageNumber
            };
            
            var result = await _lexicalSearchService.SearchAsync(request,  cancellationToken);
            return Ok(result);
        }
        
          
        [HttpGet("semantic")]
        public async Task<ActionResult<PaginatedResult<MovieViewModel>>> SemanticSearch(
            [FromQuery] string query, 
            [FromQuery] int pageNumber = 1, 
            CancellationToken cancellationToken = default)
        {
            var request = new SemanticSearchRequest
            {
                Query = query,
                PageNumber = pageNumber
            };
            
            var result = await _semanticSearchService.SearchAsync(request,  cancellationToken);
            return Ok(result);
        }
    }
}
